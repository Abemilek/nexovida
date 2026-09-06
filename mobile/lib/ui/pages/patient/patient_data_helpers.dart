import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';

List<Reminder> remindersFor(NexoRepository repository, AppUser user) {
  final items = repository.reminders
      .where(
        (item) =>
            user.role == UserRole.administrador ||
            item.patientId == user.patientId,
      )
      .toList();
  items.sort((a, b) => a.scheduledAt.compareTo(b.scheduledAt));
  return items;
}

List<HealthIndicator> indicatorsFor(NexoRepository repository, AppUser user) {
  final items = repository.indicators
      .where(
        (item) =>
            user.role == UserRole.administrador ||
            item.patientId == user.patientId,
      )
      .toList();
  items.sort((a, b) => b.measuredAt.compareTo(a.measuredAt));
  return items;
}

List<Appointment> appointmentsFor(NexoRepository repository, AppUser user) {
  final items = repository.appointments
      .where(
        (item) =>
            user.role == UserRole.administrador ||
            item.patientId == user.patientId,
      )
      .toList();
  items.sort((a, b) => a.startsAt.compareTo(b.startsAt));
  return items;
}

List<ClinicalEvent> historyFor(NexoRepository repository, AppUser user) {
  final items = repository.history
      .where(
        (item) =>
            user.role == UserRole.administrador ||
            item.patientId == user.patientId,
      )
      .toList();
  items.sort((a, b) => b.eventAt.compareTo(a.eventAt));
  return items;
}

List<CareAlert> alertsFor(NexoRepository repository, AppUser user) {
  final items = repository.alerts
      .where(
        (item) =>
            user.role == UserRole.profesional ||
            user.role == UserRole.administrador ||
            item.patientId == user.patientId,
      )
      .toList();
  items.sort((a, b) => b.createdAt.compareTo(a.createdAt));
  return items;
}
