import 'package:flutter/material.dart';

import '../../app_session.dart';
import '../../models/nexo_models.dart';
import '../shared_widgets.dart';
import '../widgets/form_helpers.dart';
import 'patient_detail_screen.dart';

class PatientsListPage extends StatefulWidget {
  final dynamic apiClient;
  final AppSession session;
  const PatientsListPage({
    super.key,
    required this.apiClient,
    required this.session,
  });
  @override
  State<PatientsListPage> createState() => _PatientsListPageState();
}

class _PatientsListPageState extends State<PatientsListPage> {
  List<Map<String, dynamic>> _patients = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _fetchPatients();
  }

  Future<void> _fetchPatients() async {
    try {
      final res = await widget.apiClient.getCollection('/api/Paciente');
      if (mounted) {
        setState(() {
          _patients = res;
          _error = null;
          _loading = false;
        });
      }
    } catch (_) {
      if (mounted) {
        setState(() {
          _patients = [];
          _error = 'No se pudo cargar la lista de pacientes.';
          _loading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final role = widget.session.user!.role;
    return PageScaffold(
      title: role == UserRole.familiar
          ? 'Pacientes bajo cuidado'
          : 'Pacientes a cargo',
      subtitle: role == UserRole.familiar
          ? 'Seguimiento compartido de las personas que acompañas'
          : 'Panel de seguimiento clínico por paciente',
      children: [
        if (_loading) const Center(child: CircularProgressIndicator()),
        if (_error != null) SyncNotice(message: _error!),
        if (!_loading && _patients.isEmpty)
          const EmptyState(
            icon: Icons.people_outline,
            title: 'No hay pacientes',
          ),
        if (!_loading)
          for (final p in _patients)
            Card(
              margin: const EdgeInsets.only(bottom: 8),
              child: ListTile(
                leading: const CircleAvatar(child: Icon(Icons.person)),
                title: Text(
                  textValue(p, const [
                    'nombre',
                    'nombreUsuario',
                  ], 'Paciente ${p['idPaciente'] ?? p['id'] ?? 1}'),
                ),
                subtitle: Text('Estado: ${p['estadoPaciente'] ?? 'Activo'}'),
                trailing: const Icon(Icons.chevron_right),
                onTap: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => PatientDetailScreen(
                        session: widget.session,
                        patientId: p['idPaciente'] ?? p['id'] ?? 1,
                        patientName: textValue(p, const [
                          'nombre',
                          'nombreUsuario',
                        ], 'Paciente ${p['idPaciente'] ?? p['id'] ?? 1}'),
                      ),
                    ),
                  );
                },
              ),
            ),
      ],
    );
  }
}
