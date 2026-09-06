import 'package:flutter/material.dart';

import '../../app_theme.dart';

import '../../../models/nexo_models.dart';
import '../../shared_widgets.dart';

Widget reminderCard(
  Reminder reminder, {
  VoidCallback? onComplete,
  VoidCallback? onEdit,
}) {
  final color = reminder.status.toLowerCase() == 'completado'
      ? AppTheme.tertiary
      : AppTheme.primary;
  return Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: InfoCard(
      leading: Icon(Icons.alarm_outlined, color: color),
      title: reminder.title,
      subtitle:
          '${reminder.type} · ${formatDateTime(reminder.scheduledAt)}\n${reminder.description}',
      accent: color,
      trailing: onComplete == null
          ? Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                StatusPill(label: reminder.status, color: color),
                if (onEdit != null)
                  IconButton(
                    tooltip: 'Editar',
                    onPressed: onEdit,
                    icon: const Icon(Icons.edit_outlined, size: 20),
                  ),
              ],
            )
          : FilledButton.icon(
              onPressed: onComplete,
              icon: const Icon(Icons.check_circle_outline),
              label: const Text('Tomé'),
            ),
    ),
  );
}

Widget indicatorCard(HealthIndicator indicator) {
  return Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: InfoCard(
      leading: Icon(
        Icons.monitor_heart_outlined,
        color: indicatorColor(indicator),
      ),
      title: indicator.name,
      subtitle:
          '${indicator.displayValue} · ${formatDateTime(indicator.measuredAt)}\n${indicator.notes.isEmpty ? indicator.source : indicator.notes}',
      accent: indicatorColor(indicator),
      trailing: Text(
        indicator.source,
        style: const TextStyle(
          fontWeight: FontWeight.w700,
          color: AppTheme.inkMuted,
        ),
      ),
    ),
  );
}

Widget appointmentCard(Appointment appointment) {
  return Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: InfoCard(
      leading: const Icon(
        Icons.event_available_outlined,
        color: AppTheme.tertiary,
      ),
      title: appointment.type,
      subtitle:
          '${formatDateTime(appointment.startsAt)} · ${appointment.modality}\n${appointment.reason}\n${appointment.place}',
      accent: AppTheme.tertiary,
      trailing: StatusPill(label: appointment.status, color: AppTheme.tertiary),
    ),
  );
}

Widget historyCard(ClinicalEvent event) {
  return Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: InfoCard(
      leading: const Icon(
        Icons.history_edu_outlined,
        color: AppTheme.historyAccent,
      ),
      title: event.title,
      subtitle:
          '${event.type} · ${formatDate(event.eventAt)}\n${event.description}',
      accent: AppTheme.historyAccent,
    ),
  );
}

Widget alertCard(CareAlert alert) {
  final color = switch (alert.priority.toLowerCase()) {
    'alta' => const Color(0xFFB42318),
    'media' => AppTheme.secondary,
    _ => AppTheme.tertiary,
  };
  return Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: InfoCard(
      leading: Icon(
        alert.attended ? Icons.task_alt : Icons.warning_amber_outlined,
        color: color,
      ),
      title: alert.title,
      subtitle:
          '${alert.type} · ${formatDateTime(alert.createdAt)}\n${alert.message}',
      accent: color,
      trailing: StatusPill(
        label: alert.attended ? 'Atendida' : alert.priority,
        color: color,
      ),
    ),
  );
}

Color indicatorColor(HealthIndicator indicator) {
  final pressureHigh =
      indicator.typeId == 1 &&
      (indicator.value >= 140 ||
          (indicator.secondaryValue != null &&
              indicator.secondaryValue! >= 90));
  final glucoseHigh = indicator.typeId == 2 && indicator.value >= 180;
  final oxygenLow = indicator.typeId == 4 && indicator.value < 92;
  return pressureHigh || glucoseHigh || oxygenLow
      ? AppTheme.secondary
      : AppTheme.primary;
}

class IndicatorSummary extends StatelessWidget {
  const IndicatorSummary({super.key, required this.indicators});

  final List<HealthIndicator> indicators;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Tendencia reciente',
              style: Theme.of(
                context,
              ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w800),
            ),
            const SizedBox(height: 14),
            for (final indicator in indicators) ...[
              Row(
                children: [
                  SizedBox(
                    width: 132,
                    child: Text(
                      indicator.name,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  Expanded(
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: LinearProgressIndicator(
                        value: (indicator.value / _scaleFor(indicator)).clamp(
                          0.04,
                          1,
                        ),
                        minHeight: 12,
                        color: indicatorColor(indicator),
                        backgroundColor: Theme.of(
                          context,
                        ).colorScheme.surfaceContainerHighest,
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  SizedBox(
                    width: 86,
                    child: Text(
                      indicator.displayValue,
                      textAlign: TextAlign.right,
                      style: const TextStyle(fontWeight: FontWeight.w700),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
            ],
          ],
        ),
      ),
    );
  }

  double _scaleFor(HealthIndicator indicator) {
    return switch (indicator.typeId) {
      1 => 180,
      2 => 240,
      3 => 140,
      4 => 100,
      _ => 200,
    };
  }
}

class TwoColumn extends StatelessWidget {
  const TwoColumn({super.key, required this.left, required this.right});

  final List<Widget> left;
  final List<Widget> right;

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        if (constraints.maxWidth < 760) {
          return Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [...left, const SizedBox(height: 18), ...right],
          );
        }
        return Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: left,
              ),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: right,
              ),
            ),
          ],
        );
      },
    );
  }
}
