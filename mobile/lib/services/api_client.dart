import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart' as http;

import '../config/environment.dart';
import '../models/login_result.dart';

class ApiException implements Exception {
  const ApiException(
    this.message, {
    this.statusCode,
    this.tokenExpired = false,
  });

  final String message;
  final int? statusCode;
  final bool tokenExpired;

  @override
  String toString() => message;
}

class ApiClient {
  ApiClient({http.Client? httpClient, this.baseUrl = Environment.apiBaseUrl})
    : _httpClient = httpClient;

  http.Client? _httpClient;
  String baseUrl;
  String? accessToken;
  String? refreshToken;

  http.Client get _client => _httpClient ??= http.Client();

  Future<LoginResult> login({
    required String email,
    required String password,
    String? totpCode,
  }) async {
    final response = await _send(
      'POST',
      '/api/auth/login',
      body: {
        'correo': email,
        'password': password,
        if (totpCode != null && totpCode.isNotEmpty) 'totpCode': totpCode,
      },
      authenticated: false,
    );

    final result = LoginResult.fromJson(_asMap(response));

    accessToken = result.accessToken;
    refreshToken = result.refreshToken;

    return result;
  }

  Future<void> register({
    required String username,
    required String email,
    required String password,
  }) async {
    await _send(
      'POST',
      '/api/Usuario',
      body: {'nombreUsuario': username, 'correo': email, 'password': password},
      authenticated: false,
    );
  }

  Future<void> logout() async {
    final token = refreshToken;

    if (token == null || token.isEmpty) return;

    await _send(
      'POST',
      '/api/auth/logout',
      body: {'refreshToken': token},
      authenticated: false,
    );

    accessToken = null;
    refreshToken = null;
  }

  Future<Map<String, dynamic>> setupTwoFactor() async {
    return _asMap(await _send('POST', '/api/auth/2fa/setup'));
  }

  Future<void> verifyTwoFactor(String code) async {
    await _send('POST', '/api/auth/2fa/verify', body: {'code': code});
  }

  Future<void> disableTwoFactor() async {
    await _send('POST', '/api/auth/2fa/disable');
  }

  Future<Map<String, dynamic>> get(String path) async {
    return _asMap(await _send('GET', path));
  }

  Future<List<Map<String, dynamic>>> getCollection(String path) async {
    final response = await _send('GET', path);

    if (response is List) {
      return response
          .whereType<Map>()
          .map((item) => Map<String, dynamic>.from(item))
          .toList();
    }

    if (response is Map<String, dynamic>) {
      for (final key in const ['data', 'items', 'result', 'value']) {
        final value = response[key];

        if (value is List) {
          return value
              .whereType<Map>()
              .map((item) => Map<String, dynamic>.from(item))
              .toList();
        }
      }
    }
    return const [];
  }

  Future<Map<String, dynamic>> post(
    String path,
    Map<String, dynamic> body,
  ) async {
    return _asMap(await _send('POST', path, body: body));
  }

  Future<Map<String, dynamic>> put(
    String path,
    Map<String, dynamic> body,
  ) async {
    return _asMap(await _send('PUT', path, body: body));
  }

  Future<dynamic> _send(
    String method,
    String path, {
    Map<String, dynamic>? body,
    bool authenticated = true,
    bool retryingAfterRefresh = false,
  }) async {
    final uri = Uri.parse('$baseUrl$path');
    final headers = <String, String>{
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      if (authenticated && accessToken != null)
        'Authorization': 'Bearer $accessToken',
    };

    final encodedBody = body == null ? null : jsonEncode(body);
    final http.Response response;
    try {
      response = await switch (method) {
        'GET' =>
          _client
              .get(uri, headers: headers)
              .timeout(const Duration(seconds: 8)),
        'POST' =>
          _client
              .post(uri, headers: headers, body: encodedBody)
              .timeout(const Duration(seconds: 8)),
        'PUT' =>
          _client
              .put(uri, headers: headers, body: encodedBody)
              .timeout(const Duration(seconds: 8)),
        _ => throw ApiException('Método HTTP no soportado: $method'),
      };
    } on TimeoutException {
      throw const ApiException('El backend tardó demasiado en responder.');
    } on Object catch (error) {
      throw ApiException('No se pudo conectar con el backend. $error');
    }

    final decoded = _decode(response.body);

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return decoded;
    }

    final error = _errorFromResponse(response.statusCode, decoded);

    if (error.tokenExpired && !retryingAfterRefresh && refreshToken != null) {
      final refreshed = await _refresh();

      if (refreshed) {
        return _send(
          method,
          path,
          body: body,
          authenticated: authenticated,
          retryingAfterRefresh: true,
        );
      }
    }
    throw error;
  }

  Future<bool> _refresh() async {
    final token = refreshToken;

    if (token == null || token.isEmpty) {
      return false;
    }

    try {
      final response = await _send(
        'POST',
        '/api/auth/refresh',
        body: {'refreshToken': token},
        authenticated: false,
        retryingAfterRefresh: true,
      );

      final result = LoginResult.fromJson(_asMap(response));

      accessToken = result.accessToken;
      refreshToken = result.refreshToken;

      return accessToken != null;
    } on Object {
      return false;
    }
  }

  dynamic _decode(String body) {
    if (body.trim().isEmpty) {
      return <String, dynamic>{};
    }

    final decoded = jsonDecode(body);

    if (decoded is Map) {
      return Map<String, dynamic>.from(decoded);
    }

    return decoded;
  }

  Map<String, dynamic> _asMap(dynamic value) {
    if (value is Map<String, dynamic>) {
      return value;
    }

    if (value is Map) {
      return Map<String, dynamic>.from(value);
    }

    return <String, dynamic>{};
  }

  ApiException _errorFromResponse(int statusCode, dynamic decoded) {
    if (statusCode == 429) {
      return const ApiException(
        'Límite de peticiones alcanzado. Intenta de nuevo en un minuto.',
        statusCode: 429,
      );
    }

    if (decoded is Map<String, dynamic>) {
      final message = decoded['message']?.toString();

      return ApiException(
        message == null || message.isEmpty
            ? 'Error HTTP $statusCode.'
            : message,
        statusCode: statusCode,
        tokenExpired: decoded['tokenExpired'] == true,
      );
    }

    return ApiException('Error HTTP $statusCode.', statusCode: statusCode);
  }
}
