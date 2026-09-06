import 'package:flutter/material.dart';

import '../../app_session.dart';
import '../widgets/destination.dart';
import 'patient/alerts_page.dart';
import 'patient/appointments_page.dart';
import 'patient/dashboard_page.dart';
import 'patient/history_page.dart';
import 'patient/indicators_page.dart';
import 'patient/reminders_page.dart';

class PatientDetailScreen extends StatefulWidget {
  final AppSession session;
  final int patientId;
  final String patientName;
  const PatientDetailScreen({
    super.key,
    required this.session,
    required this.patientId,
    required this.patientName,
  });

  @override
  State<PatientDetailScreen> createState() => _PatientDetailScreenState();
}

class _PatientDetailScreenState extends State<PatientDetailScreen> {
  int _index = 0;

  static const _destinations = [
    Destination('Inicio', Icons.dashboard_outlined),
    Destination('Recordatorios', Icons.alarm_outlined),
    Destination('Indicadores', Icons.monitor_heart_outlined),
    Destination('Citas', Icons.event_available_outlined),
    Destination('Historial', Icons.history_edu_outlined),
    Destination('Alertas', Icons.warning_amber_outlined),
  ];

  @override
  Widget build(BuildContext context) {
    final user = widget.session.user!.copyWith(patientId: widget.patientId);
    final repo = widget.session.repository;

    Widget page = switch (_index) {
      0 => DashboardPage(repository: repo, user: user),
      1 => RemindersPage(repository: repo, user: user),
      2 => IndicatorsPage(repository: repo, user: user),
      3 => AppointmentsPage(repository: repo, user: user),
      4 => HistoryPage(repository: repo, user: user),
      _ => AlertsPage(repository: repo, user: user),
    };

    return Scaffold(
      appBar: AppBar(
        title: Text('${widget.patientName} · ${_destinations[_index].label}'),
      ),
      body: Row(
        children: [
          NavigationRail(
            selectedIndex: _index,
            onDestinationSelected: (val) => setState(() => _index = val),
            labelType: NavigationRailLabelType.all,
            destinations: [
              for (final d in _destinations)
                NavigationRailDestination(
                  icon: Icon(d.icon),
                  selectedIcon: Icon(d.icon, fill: 1),
                  label: Text(d.label),
                ),
            ],
          ),
          const VerticalDivider(width: 1),
          Expanded(child: page),
        ],
      ),
    );
  }
}
