enum UserRole {
  administrador('Administrador'),
  paciente('Paciente'),
  profesional('ProfesionalSalud'),
  familiar('Familiar');

  const UserRole(this.label);

  final String label;
}
