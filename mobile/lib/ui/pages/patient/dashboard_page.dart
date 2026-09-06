import 'package:flutter/material.dart';

import '../../app_theme.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../shared_widgets.dart';
import 'patient_cards.dart';
import 'patient_data_helpers.dart';
import 'patient_dialogs.dart';

class DashboardPage extends StatelessWidget {
  const DashboardPage({
    super.key,
    required this.repository,
    required this.user,
  });

  final NexoRepository repository;
  final AppUser user;

  @override
  Widget build(BuildContext context) {
    final reminders = remindersFor(repository, user);
    final indicators = indicatorsFor(repository, user);
    final appointments = appointmentsFor(repository, user);
    final alerts = alertsFor(repository, user);
    final latestIndicators = indicators.take(4).toList();
    final pendingReminders =
        reminders
            .where((item) => item.status.toLowerCase() != 'completado')
            .toList()
          ..sort((a, b) => a.scheduledAt.compareTo(b.scheduledAt));
    final upcomingAppointments =
        appointments
            .where(
              (item) => item.startsAt.isAfter(
                DateTime.now().subtract(const Duration(days: 1)),
              ),
            )
            .toList()
          ..sort((a, b) => a.startsAt.compareTo(b.startsAt));
    final activeAlerts = alerts.where((item) => !item.attended).toList();
    final completedReminders = reminders
        .where((item) => item.status.toLowerCase() == 'completado')
        .length;
    final adherence = reminders.isEmpty
        ? 0
        : ((completedReminders / reminders.length) * 100).round();

    return PageScaffold(
      title: 'Hola, ${user.name}',
      subtitle: '${user.role.label} · seguimiento integral de salud',
      children: [
        if (repository.lastError != null)
          SyncNotice(message: repository.lastError!),
        ResponsiveGrid(
          children: [
            MetricCard(
              icon: Icons.alarm_on_outlined,
              label: 'Recordatorios pendientes',
              value: '${pendingReminders.length}',
              detail: 'Medicamentos, controles y citas',
              color: AppTheme.primary,
            ),
            MetricCard(
              icon: Icons.monitor_heart_outlined,
              label: 'Últimas mediciones',
              value: '${indicators.length}',
              detail: latestIndicators.isEmpty
                  ? 'Sin registros'
                  : latestIndicators.first.displayValue,
              color: AppTheme.secondary,
            ),
            MetricCard(
              icon: Icons.event_available_outlined,
              label: 'Próximas citas',
              value: '${upcomingAppointments.length}',
              detail: upcomingAppointments.isEmpty
                  ? 'Agenda libre'
                  : formatDate(upcomingAppointments.first.startsAt),
              color: AppTheme.tertiary,
            ),
            MetricCard(
              icon: Icons.verified_outlined,
              label: 'Adherencia',
              value: '$adherence%',
              detail: 'Según recordatorios completados',
              color: AppTheme.historyAccent,
            ),
          ],
        ),
        const SizedBox(height: 20),
        TwoColumn(
          left: [
            const SectionHeader(title: 'Hoy'),
            if (pendingReminders.isEmpty)
              const EmptyState(
                icon: Icons.check_circle_outline,
                title: 'Sin recordatorios pendientes',
              )
            else
              ...pendingReminders
                  .take(3)
                  .map(
                    (reminder) => reminderCard(
                      reminder,
                      onComplete: user.role == UserRole.paciente
                          ? () => repository.completeReminder(reminder)
                          : null,
                      onEdit: user.role == UserRole.profesional
                          ? () => showReminderDialog(
                              context,
                              repository,
                              user,
                              existing: reminder,
                            )
                          : null,
                    ),
                  ),
          ],
          right: [
            const SectionHeader(title: 'Indicadores recientes'),
            if (latestIndicators.isEmpty)
              const EmptyState(
                icon: Icons.monitor_heart_outlined,
                title: 'Sin mediciones registradas',
              )
            else
              IndicatorSummary(indicators: latestIndicators),
          ],
        ),
        const SizedBox(height: 20),
        const SectionHeader(title: 'Alertas preventivas'),
        if (activeAlerts.isEmpty)
          const EmptyState(
            icon: Icons.shield_outlined,
            title: 'No hay alertas activas',
          )
        else
          ...activeAlerts.take(3).map(alertCard),
      ],
    );
  }
}
