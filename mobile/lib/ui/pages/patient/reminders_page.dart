import 'package:flutter/material.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../shared_widgets.dart';
import 'patient_cards.dart';
import 'patient_data_helpers.dart';
import 'patient_dialogs.dart';

class RemindersPage extends StatelessWidget {
  const RemindersPage({
    super.key,
    required this.repository,
    required this.user,
  });

  final NexoRepository repository;
  final AppUser user;

  @override
  Widget build(BuildContext context) {
    final reminders = remindersFor(repository, user);
    return PageScaffold(
      title: 'Recordatorios',
      subtitle: 'Toma de medicamentos, controles y avisos programados',
      action: user.role == UserRole.profesional
          ? FilledButton.icon(
              onPressed: () => showReminderDialog(context, repository, user),
              icon: const Icon(Icons.add_alarm),
              label: const Text('Nuevo'),
            )
          : null,
      children: reminders.isEmpty
          ? const [
              EmptyState(
                icon: Icons.alarm_off_outlined,
                title: 'No hay recordatorios',
              ),
            ]
          : reminders
                .map(
                  (reminder) => reminderCard(
                    reminder,
                    onComplete:
                        user.role == UserRole.paciente &&
                            reminder.status.toLowerCase() != 'completado'
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
                )
                .toList(),
    );
  }
}
