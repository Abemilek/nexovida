import 'package:flutter/material.dart';

import '../app_session.dart';
import '../models/nexo_models.dart';
import '../services/nexo_repository.dart';
import 'security_dialog.dart';
import 'widgets/destination.dart';
import 'pages/admin/admin_config_page.dart';
import 'pages/admin/admin_metrics_page.dart';
import 'pages/admin/admin_users_page.dart';
import 'pages/patients_list_page.dart';
import 'pages/patient/alerts_page.dart';
import 'pages/patient/appointments_page.dart';
import 'pages/patient/dashboard_page.dart';
import 'pages/patient/history_page.dart';
import 'pages/patient/indicators_page.dart';
import 'pages/patient/reminders_page.dart';

class HomeShell extends StatefulWidget {
  const HomeShell({super.key, required this.session});

  final AppSession session;

  @override
  State<HomeShell> createState() => _HomeShellState();
}

class _HomeShellState extends State<HomeShell> {
  int _index = 0;

  List<Destination> _getDestinations(UserRole role) {
    if (role == UserRole.administrador) {
      return const [
        Destination('Gestión Usuarios', Icons.people_outline),
        Destination('Métricas', Icons.bar_chart_outlined),
        Destination('Configuración', Icons.settings_outlined),
      ];
    }
    if (role == UserRole.profesional || role == UserRole.familiar) {
      return const [
        Destination('Pacientes', Icons.people_outline),
        Destination('Alertas', Icons.warning_amber_outlined),
      ];
    }
    return const [
      Destination('Inicio', Icons.dashboard_outlined),
      Destination('Recordatorios', Icons.alarm_outlined),
      Destination('Indicadores', Icons.monitor_heart_outlined),
      Destination('Citas', Icons.event_available_outlined),
      Destination('Historial', Icons.history_edu_outlined),
      Destination('Alertas', Icons.warning_amber_outlined),
    ];
  }

  @override
  Widget build(BuildContext context) {
    final user = widget.session.user!;
    final destinations = _getDestinations(user.role);
    // El chrome de navegación (AppBar, NavigationRail/Bar) no depende de los
    // datos de `repository` — solo de `_index` y del rol, que ya disparan
    // rebuild por su cuenta vía setState/build. Antes todo esto vivía
    // dentro del mismo AnimatedBuilder que la página activa, así que
    // marcar un recordatorio o refrescar reconstruía también la barra de
    // navegación entera en cada notificación. Ahora solo escucha lo que
    // realmente necesita los datos: el ícono de refresco y el contenido.
    return LayoutBuilder(
      builder: (context, constraints) {
        final wide = constraints.maxWidth >= 900;
        final page = AnimatedBuilder(
          animation: Listenable.merge([
            widget.session,
            widget.session.repository,
          ]),
          builder: (context, _) =>
              _buildPage(widget.session.repository, user, destinations),
        );
        return Scaffold(
          appBar: AppBar(
            title: Text(
              destinations.length > _index ? destinations[_index].label : '',
            ),
            actions: [
              AnimatedBuilder(
                animation: widget.session.repository,
                builder: (context, _) {
                  final repo = widget.session.repository;
                  return IconButton(
                    tooltip: 'Actualizar',
                    onPressed: repo.isLoading ? null : repo.refresh,
                    icon: repo.isLoading
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Icon(Icons.refresh),
                  );
                },
              ),
              IconButton(
                tooltip: 'Seguridad · 2FA',
                onPressed: () {
                  showDialog<void>(
                    context: context,
                    builder: (_) =>
                        SecurityDialog(apiClient: widget.session.apiClient),
                  );
                },
                icon: const Icon(Icons.shield_outlined),
              ),
              IconButton(
                tooltip: 'Cerrar sesión',
                onPressed: widget.session.logout,
                icon: const Icon(Icons.logout),
              ),
            ],
          ),
          body: wide
              ? Row(
                  children: [
                    NavigationRail(
                      selectedIndex: _index,
                      onDestinationSelected: (value) =>
                          setState(() => _index = value),
                      labelType: NavigationRailLabelType.all,
                      leading: Padding(
                        padding: const EdgeInsets.only(bottom: 16),
                        child: CircleAvatar(
                          backgroundColor: Theme.of(
                            context,
                          ).colorScheme.primary,
                          child: const Icon(
                            Icons.health_and_safety_outlined,
                            color: Colors.white,
                          ),
                        ),
                      ),
                      destinations: [
                        for (final destination in destinations)
                          NavigationRailDestination(
                            icon: Icon(destination.icon),
                            selectedIcon: Icon(destination.icon, fill: 1),
                            label: Text(destination.label),
                          ),
                      ],
                    ),
                    const VerticalDivider(width: 1),
                    Expanded(child: page),
                  ],
                )
              : page,
          bottomNavigationBar: wide
              ? null
              : NavigationBar(
                  selectedIndex: _index,
                  onDestinationSelected: (value) =>
                      setState(() => _index = value),
                  destinations: [
                    for (final destination in destinations)
                      NavigationDestination(
                        icon: Icon(destination.icon),
                        label: destination.label,
                      ),
                  ],
                ),
        );
      },
    );
  }

  Widget _buildPage(
    NexoRepository repo,
    AppUser user,
    List<Destination> destinations,
  ) {
    if (user.role == UserRole.administrador) {
      return switch (_index) {
        0 => AdminUsersPage(apiClient: widget.session.apiClient),
        1 => AdminMetricsPage(apiClient: widget.session.apiClient),
        _ => AdminConfigPage(apiClient: widget.session.apiClient),
      };
    }
    if (user.role == UserRole.profesional || user.role == UserRole.familiar) {
      return switch (_index) {
        0 => PatientsListPage(
          apiClient: widget.session.apiClient,
          session: widget.session,
        ),
        _ => AlertsPage(repository: repo, user: user),
      };
    }
    return switch (_index) {
      0 => DashboardPage(repository: repo, user: user),
      1 => RemindersPage(repository: repo, user: user),
      2 => IndicatorsPage(repository: repo, user: user),
      3 => AppointmentsPage(repository: repo, user: user),
      4 => HistoryPage(repository: repo, user: user),
      _ => AlertsPage(repository: repo, user: user),
    };
  }
}
