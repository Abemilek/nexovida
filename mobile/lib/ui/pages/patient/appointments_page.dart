import 'package:flutter/material.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../shared_widgets.dart';
import 'patient_cards.dart';
import 'patient_data_helpers.dart';
import 'patient_dialogs.dart';

class AppointmentsPage extends StatelessWidget {
  const AppointmentsPage({
    super.key,
    required this.repository,
    required this.user,
  });

  final NexoRepository repository;
  final AppUser user;

  @override
  Widget build(BuildContext context) {
    final appointments = appointmentsFor(repository, user);
    return PageScaffold(
      title: 'Citas médicas',
      subtitle: 'Agenda de controles, teleconsultas y visitas presenciales',
      action: user.role == UserRole.paciente
          ? FilledButton.icon(
              onPressed: () => showAppointmentDialog(context, repository, user),
              icon: const Icon(Icons.add_circle_outline),
              label: const Text('Agendar'),
            )
          : null,
      children: appointments.isEmpty
          ? const [
              EmptyState(
                icon: Icons.event_busy_outlined,
                title: 'No hay citas registradas',
              ),
            ]
          : appointments.map(appointmentCard).toList(),
    );
  }
}
