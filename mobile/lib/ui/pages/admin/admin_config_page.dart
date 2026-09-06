import 'package:flutter/material.dart';

import '../../shared_widgets.dart';

class AdminConfigPage extends StatefulWidget {
  final dynamic apiClient;
  const AdminConfigPage({super.key, required this.apiClient});
  @override
  State<AdminConfigPage> createState() => _AdminConfigPageState();
}

class _AdminConfigPageState extends State<AdminConfigPage> {
  Map<String, dynamic>? _config;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _fetchConfig();
  }

  Future<void> _fetchConfig() async {
    try {
      final res = await widget.apiClient.get('/api/Configuracion');
      if (mounted) {
        setState(() {
          _config = res;
          _error = null;
          _loading = false;
        });
      }
    } catch (_) {
      if (mounted) {
        setState(() {
          _config = null;
          _error = 'No se pudo cargar la configuración del sistema.';
          _loading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return PageScaffold(
      title: 'Configuración',
      subtitle: 'Ajustes globales del sistema',
      children: [
        if (_loading) const Center(child: CircularProgressIndicator()),
        if (_error != null) SyncNotice(message: _error!),
        if (_config != null) ...[
          Card(
            child: Column(
              children: [
                SwitchListTile(
                  title: const Text('Notificaciones push globales'),
                  value: _config!['notificacionesPush'] == true,
                  onChanged: null,
                ),
                ListTile(
                  title: const Text('Frecuencia por defecto'),
                  subtitle: Text('${_config!['frecuenciaRecordatorios']}'),
                ),
                SwitchListTile(
                  title: const Text('Modo mantenimiento'),
                  value: _config!['mantenimientoActivo'] == true,
                  onChanged: null,
                ),
                ListTile(
                  title: const Text('Versión del sistema'),
                  trailing: Text('${_config!['versionSistema']}'),
                ),
              ],
            ),
          ),
        ],
      ],
    );
  }
}
