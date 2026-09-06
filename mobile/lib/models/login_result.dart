import 'model_helpers.dart';

class LoginResult {
  const LoginResult({
    this.accessToken,
    this.refreshToken,
    required this.expiresInSeconds,
    required this.requiresTwoFactor,
  });

  factory LoginResult.fromJson(Map<String, dynamic> json) {
    return LoginResult(
      accessToken: json['accessToken'] as String?,
      refreshToken: json['refreshToken'] as String?,
      expiresInSeconds: asInt(json['expiresInSeconds']) ?? 0,
      requiresTwoFactor: json['requiresTwoFactor'] == true,
    );
  }

  final String? accessToken;
  final String? refreshToken;
  final int expiresInSeconds;
  final bool requiresTwoFactor;
}
