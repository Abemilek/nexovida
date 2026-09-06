import 'user_role.dart';

class AppUser {
  const AppUser({
    required this.name,
    required this.email,
    required this.role,
    this.patientId = 1,
    this.userId = 1,
    this.familiarId,
    this.profesionalId,
  });

  final String name;
  final String email;
  final UserRole role;
  final int patientId;
  final int userId;
  final int? familiarId;
  final int? profesionalId;

  AppUser copyWith({
    String? name,
    String? email,
    UserRole? role,
    int? patientId,
    int? userId,
    int? familiarId,
    int? profesionalId,
  }) {
    return AppUser(
      name: name ?? this.name,
      email: email ?? this.email,
      role: role ?? this.role,
      patientId: patientId ?? this.patientId,
      userId: userId ?? this.userId,
      familiarId: familiarId ?? this.familiarId,
      profesionalId: profesionalId ?? this.profesionalId,
    );
  }
}
