import 'package:flutter/material.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../shared_widgets.dart';
import 'patient_cards.dart';
import 'patient_data_helpers.dart';
import 'patient_dialogs.dart';

class HistoryPage extends StatelessWidget {
  const HistoryPage({super.key, required this.repository, required this.user});

  final NexoRepository repository;
  final AppUser user;

  @override
  Widget build(BuildContext context) {
    final history = historyFor(repository, user);
    return PageScaffold(
      title: 'Historial clínico',
      subtitle: 'Eventos relevantes integrados al seguimiento del paciente',
      action: user.role == UserRole.profesional
          ? FilledButton.icon(
              onPressed: () =>
                  showClinicalEventDialog(context, repository, user),
              icon: const Icon(Icons.post_add_outlined),
              label: const Text('Evento'),
            )
          : null,
      children: history.isEmpty
          ? const [
              EmptyState(
                icon: Icons.history_toggle_off_outlined,
                title: 'Sin historial registrado',
              ),
            ]
          : history.map(historyCard).toList(),
    );
  }
}

