import 'package:flutter/material.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../shared_widgets.dart';
import 'patient_cards.dart';
import 'patient_data_helpers.dart';
import 'patient_dialogs.dart';

class IndicatorsPage extends StatelessWidget {
  const IndicatorsPage({
    super.key,
    required this.repository,
    required this.user,
  });

  final NexoRepository repository;
  final AppUser user;

  @override
  Widget build(BuildContext context) {
    final indicators = indicatorsFor(repository, user);
    return PageScaffold(
      title: 'Indicadores de salud',
      subtitle:
          'Registro sistemático de signos vitales y mediciones relevantes',
      action: user.role == UserRole.paciente
          ? FilledButton.icon(
              onPressed: () => showIndicatorDialog(context, repository, user),
              icon: const Icon(Icons.add_chart_outlined),
              label: const Text('Registrar'),
            )
          : null,
      children: [
        if (indicators.isEmpty)
          const EmptyState(
            icon: Icons.monitor_heart_outlined,
            title: 'Sin indicadores registrados',
          )
        else ...[
          IndicatorSummary(indicators: indicators.take(6).toList()),
          const SizedBox(height: 14),
          ...indicators.map(indicatorCard),
        ],
      ],
    );
  }
}

