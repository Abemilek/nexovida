import 'package:flutter/material.dart';

import '../../app_theme.dart';

import '../../shared_widgets.dart';
import '../../widgets/form_helpers.dart';

class AdminMetricsPage extends StatefulWidget {
  final dynamic apiClient;
  const AdminMetricsPage({super.key, required this.apiClient});
  @override
  State<AdminMetricsPage> createState() => _AdminMetricsPageState();
}

class _AdminMetricsPageState extends State<AdminMetricsPage> {
  Map<String, dynamic>? _metrics;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _fetchMetrics();
  }

  Future<void> _fetchMetrics() async {
    try {
      final res = await widget.apiClient.get('/api/Metrica');
      if (mounted) {
        setState(() {
          _metrics = res;
          _error = null;
          _loading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _metrics = null;
          _error = 'No se pudieron cargar las métricas globales.';
          _loading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return PageScaffold(
      title: 'Métricas Globales',
      subtitle: 'Estadísticas en vivo del sistema',
      children: [
        if (_loading) const Center(child: CircularProgressIndicator()),
        if (_error != null) SyncNotice(message: _error!),
        if (_metrics != null)
          ResponsiveGrid(
            children: [
              MetricCard(
                icon: Icons.people,
                label: 'Pacientes',
                value: '${metricValue(_metrics!, 'totalPacientes')}',
                detail: 'Total registrados',
                color: AppTheme.primary,
              ),
              MetricCard(
                icon: Icons.medical_services,
                label: 'Profesionales',
                value: '${metricValue(_metrics!, 'totalProfesionales')}',
                detail: 'En el sistema',
                color: AppTheme.tertiary,
              ),
              MetricCard(
                icon: Icons.alarm,
                label: 'Recordatorios',
                value: '${metricValue(_metrics!, 'totalRecordatorios')}',
                detail: 'Programados',
                color: AppTheme.secondary,
              ),
              MetricCard(
                icon: Icons.monitor_heart,
                label: 'Indicadores',
                value: '${metricValue(_metrics!, 'totalIndicadores')}',
                detail: 'Mediciones guardadas',
                color: AppTheme.historyAccent,
              ),
            ],
          ),
      ],
    );
  }
}
