import 'package:flutter/material.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../shared_widgets.dart';
import 'patient_cards.dart';
import 'patient_data_helpers.dart';

class AlertsPage extends StatelessWidget {
  const AlertsPage({super.key, required this.repository, required this.user});

  final NexoRepository repository;
  final AppUser user;

  @override
  Widget build(BuildContext context) {
    final alerts = alertsFor(repository, user);
    return PageScaffold(
      title: 'Alertas',
      subtitle: 'Prioridades preventivas para atención oportuna',
      children: alerts.isEmpty
          ? const [
              EmptyState(
                icon: Icons.notifications_off_outlined,
                title: 'No hay alertas',
              ),
            ]
          : alerts.map(alertCard).toList(),
    );
  }
}

