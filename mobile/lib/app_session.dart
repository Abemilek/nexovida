import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:nexovida_app/models/app_user.dart';
import 'package:nexovida_app/models/user_role.dart';
import 'config/environment.dart';
import 'services/api_client.dart';
import 'services/nexo_repository.dart';

class AppSession extends ChangeNotifier {
  AppSession(this.apiClient) : repository = NexoRepository(apiClient);

  final ApiClient apiClient;
  final NexoRepository repository;

  AppUser? user;
  bool isBusy = false;
  bool requiresTwoFactor = false;
  String? lastError;
  String _pendingEmail = '';
  String _pendingPassword = '';

  bool get isAuthenticated => user != null;

  Future<bool> login({
    required String email,
    required String password,
    String? totpCode,
    String? baseUrl,
  }) async {
    isBusy = true;
    lastError = null;
    requiresTwoFactor = false;
    apiClient.baseUrl = baseUrl ?? Environment.apiBaseUrl;
    notifyListeners();

    try {
      final result = await apiClient.login(
        email: email,
        password: password,
        totpCode: totpCode,
      );
      if (result.requiresTwoFactor) {
        requiresTwoFactor = true;
        _pendingEmail = email;
        _pendingPassword = password;
        isBusy = false;
        notifyListeners();
        return false;
      }

      user = await _resolveUser(result.accessToken!, email);
      await repository.refresh();
      return true;
    } on ApiException catch (error) {
      lastError = error.message;
      return false;
    } finally {
      isBusy = false;
      notifyListeners();
    }
  }

  Future<bool> confirmTwoFactor(String code) {
    return login(
      email: _pendingEmail,
      password: _pendingPassword,
      totpCode: code,
    );
  }

  Future<bool> register({
    required String username,
    required String email,
    required String password,
    String? baseUrl,
  }) async {
    isBusy = true;
    lastError = null;
    apiClient.baseUrl = baseUrl ?? Environment.apiBaseUrl;
    notifyListeners();
    try {
      await apiClient.register(
        username: username,
        email: email,
        password: password,
      );
      return true;
    } on ApiException catch (error) {
      lastError = error.message;
      return false;
    } finally {
      isBusy = false;
      notifyListeners();
    }
  }

  Future<void> logout() async {
    try {
      await apiClient.logout();
    } on Object {}
    user = null;
    requiresTwoFactor = false;
    _pendingEmail = '';
    _pendingPassword = '';
    apiClient.accessToken = null;
    apiClient.refreshToken = null;
    repository.clearLocalData();
    notifyListeners();
  }

  void resetAuthState() {
    requiresTwoFactor = false;
    lastError = null;
    _pendingEmail = '';
    _pendingPassword = '';
    notifyListeners();
  }

  Future<AppUser> _resolveUser(String token, String email) async {
    final fromToken = _userFromToken(token, email);
    try {
      final me = await apiClient.get('/api/auth/me');
      final idPaciente = _asInt(me['idPaciente']);
      final idFamiliar = _asInt(me['idFamiliar']);
      final idProfesional = _asInt(me['idProfesional']);
      final nombre = me['nombreUsuario']?.toString().trim();
      final correo = me['correo']?.toString().trim();
      return AppUser(
        name: (nombre == null || nombre.isEmpty) ? fromToken.name : nombre,
        email: (correo == null || correo.isEmpty) ? email : correo,
        role: fromToken.role,
        patientId: idPaciente ?? fromToken.patientId,
        userId: fromToken.userId,
        familiarId: idFamiliar,
        profesionalId: idProfesional,
      );
    } on Object {
      return fromToken;
    }
  }

  int? _asInt(Object? value) {
    if (value is int) return value;
    if (value is num) return value.toInt();
    if (value is String) return int.tryParse(value);
    return null;
  }

  AppUser _userFromToken(String token, String email) {
    final parts = token.split('.');
    if (parts.length == 3) {
      try {
        final payload = parts[1];
        final normalized = base64Url.normalize(payload);
        final decoded = utf8.decode(base64Url.decode(normalized));
        final map = json.decode(decoded) as Map<String, dynamic>;

        final roleStr =
            map['role'] ??
            map['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        final roleValue = (roleStr is List)
            ? roleStr.first.toString()
            : roleStr?.toString();
        final userId =
            int.tryParse(
              (map['nameid'] ??
                      map['sub'] ??
                      map['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
                      '1')
                  .toString(),
            ) ??
            1;

        UserRole parsedRole = UserRole.paciente;
        if (roleValue == 'Administrador') parsedRole = UserRole.administrador;
        if (roleValue == 'ProfesionalSalud') parsedRole = UserRole.profesional;
        if (roleValue == 'Familiar') parsedRole = UserRole.familiar;

        return AppUser(
          name: email.split('@').first,
          email: email,
          role: parsedRole,
          patientId: parsedRole == UserRole.paciente ? userId : 1,
          userId: userId,
        );
      } catch (_) {}
    }
    return _userFromEmail(email);
  }

  AppUser _userFromEmail(String email) {
    return AppUser(
      name: email.split('@').first,
      email: email,
      role: UserRole.paciente,
      patientId: 0,
      userId: 0,
    );
  }
}
