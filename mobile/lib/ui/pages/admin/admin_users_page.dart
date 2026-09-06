import 'package:flutter/material.dart';

import '../../app_theme.dart';

import '../../../services/api_client.dart';
import '../../shared_widgets.dart';
import '../../widgets/form_helpers.dart';

class AdminUsersPage extends StatefulWidget {
  final dynamic apiClient;
  const AdminUsersPage({super.key, required this.apiClient});
  @override
  State<AdminUsersPage> createState() => _AdminUsersPageState();
}

class _AdminUsersPageState extends State<AdminUsersPage> {
  List<Map<String, dynamic>> _users = [];
  bool _loading = true;
  bool _busy = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _fetchUsers();
  }

  Future<void> _fetchUsers() async {
    try {
      final res = await widget.apiClient.getCollection('/api/Usuario');
      if (mounted) {
        setState(() {
          _users = res;
          _error = null;
          _loading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _users = [];
          _error =
              'No se pudo cargar la lista de usuarios.'
              ' Verifica la conexión y tu rol.';
          _loading = false;
        });
      }
    }
  }

  void _notify(String message) {
    if (!mounted) return;
    ScaffoldMessenger.of(context)
      ..clearSnackBars()
      ..showSnackBar(SnackBar(content: Text(message)));
  }

  Future<void> _toggleActivo(Map<String, dynamic> usuario) async {
    final id = usuario['idUsuario'];
    final nuevoEstado = usuario['activo'] == false;
    setState(() => _busy = true);
    try {
      await widget.apiClient.put('/api/Usuario/$id', {
        'nombreUsuario': textValue(usuario, const [
          'nombreUsuario',
          'nombre',
        ], ''),
        'correo': usuario['correo']?.toString() ?? '',
        'activo': nuevoEstado,
      });
      if (mounted) {
        setState(() {
          _users = [
            for (final u in _users)
              if (u['idUsuario'] == id) {...u, 'activo': nuevoEstado} else u,
          ];
        });
        _notify(nuevoEstado ? 'Cuenta activada.' : 'Cuenta desactivada.');
      }
    } catch (_) {
      if (mounted) {
        _notify('No se pudo cambiar el estado de la cuenta.');
      }
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _openCreateDialog() async {
    await showDialog<void>(
      context: context,
      builder: (context) => _AdminCreateUserDialog(
        apiClient: widget.apiClient,
        onCreated: () {
          Navigator.pop(context);
          _fetchUsers();
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return PageScaffold(
      title: 'Gestión Usuarios',
      subtitle: 'Panel administrativo de cuentas',
      action: FilledButton.icon(
        onPressed: _busy ? null : _openCreateDialog,
        icon: const Icon(Icons.person_add_alt_outlined),
        label: const Text('Nuevo usuario'),
      ),
      children: [
        if (_loading) const Center(child: CircularProgressIndicator()),
        if (_error != null) SyncNotice(message: _error!),
        if (!_loading && _users.isEmpty && _error == null)
          const EmptyState(
            icon: Icons.people_outline,
            title: 'No hay usuarios registrados',
          ),
        if (!_loading)
          for (final u in _users)
            Card(
              margin: const EdgeInsets.only(bottom: 8),
              child: ListTile(
                enabled: !_busy,
                leading: const CircleAvatar(child: Icon(Icons.person)),
                title: Text(
                  textValue(u, const [
                    'nombre',
                    'nombreUsuario',
                    'correo',
                  ], 'Usuario'),
                ),
                subtitle: Text(textValue(u, const ['rol', 'correo'], '')),
                trailing: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    StatusPill(
                      label: u['activo'] == false ? 'Inactivo' : 'Activo',
                      color: u['activo'] == false
                          ? const Color(0xFFB42318)
                          : AppTheme.tertiary,
                    ),
                    IconButton(
                      tooltip: u['activo'] == false
                          ? 'Activar cuenta'
                          : 'Desactivar cuenta',
                      onPressed: () => _toggleActivo(u),
                      icon: Icon(
                        u['activo'] == false
                            ? Icons.power_settings_new
                            : Icons.power_off,
                        color: u['activo'] == false
                            ? AppTheme.tertiary
                            : const Color(0xFFB42318),
                      ),
                    ),
                  ],
                ),
              ),
            ),
      ],
    );
  }
}

class _AdminCreateUserDialog extends StatefulWidget {
  const _AdminCreateUserDialog({
    required this.apiClient,
    required this.onCreated,
  });

  final dynamic apiClient;
  final VoidCallback onCreated;

  @override
  State<_AdminCreateUserDialog> createState() => _AdminCreateUserDialogState();
}

class _AdminCreateUserDialogState extends State<_AdminCreateUserDialog> {
  static const _roles = <String, int>{
    'Paciente': 2,
    'Familiar': 3,
    'ProfesionalSalud': 4,
    'Administrador': 1,
  };

  static final _passwordPattern = RegExp(
    r'^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$',
  );

  final _formKey = GlobalKey<FormState>();
  final _nombre = TextEditingController();
  final _correo = TextEditingController();
  final _password = TextEditingController();
  final _tipoPaciente = TextEditingController(text: 'Crónico');
  final _especialidad = TextEditingController();
  final _licencia = TextEditingController();
  final _parentesco = TextEditingController();

  String _rol = 'Paciente';
  bool _saving = false;
  String? _error;

  @override
  void dispose() {
    _nombre.dispose();
    _correo.dispose();
    _password.dispose();
    _tipoPaciente.dispose();
    _especialidad.dispose();
    _licencia.dispose();
    _parentesco.dispose();
    super.dispose();
  }

  Future<void> _guardar() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() {
      _saving = true;
      _error = null;
    });
    try {
      final creado = await widget.apiClient.post('/api/Usuario', {
        'nombreUsuario': _nombre.text.trim(),
        'correo': _correo.text.trim(),
        'password': _password.text,
      });
      final idUsuario = creado['idUsuario'];
      await widget.apiClient.post('/api/UsuarioRol', {
        'idUsuario': idUsuario,
        'idRol': _roles[_rol],
      });
      switch (_rol) {
        case 'Paciente':
          await widget.apiClient.post('/api/Paciente', {
            'idUsuario': idUsuario,
            'tipoPaciente': _tipoPaciente.text.trim(),
            'estadoPaciente': 'Activo',
          });
        case 'ProfesionalSalud':
          await widget.apiClient.post('/api/ProfesionalSalud', {
            'idUsuario': idUsuario,
            'especialidad': _especialidad.text.trim(),
            'numeroLicencia': _licencia.text.trim(),
          });
        case 'Familiar':
          await widget.apiClient.post('/api/Familiar', {
            'idUsuario': idUsuario,
            'parentesco': _parentesco.text.trim(),
          });
        default:
          break;
      }
      if (mounted) widget.onCreated();
    } catch (e) {
      if (mounted) {
        setState(() => _error = e is ApiException ? e.message : e.toString());
      }
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Nuevo usuario'),
      content: SizedBox(
        width: 460,
        child: SingleChildScrollView(
          child: Form(
            key: _formKey,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextFormField(
                  controller: _nombre,
                  decoration: const InputDecoration(
                    labelText: 'Nombre de usuario',
                    prefixIcon: Icon(Icons.badge_outlined),
                  ),
                  validator: requiredField,
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _correo,
                  keyboardType: TextInputType.emailAddress,
                  decoration: const InputDecoration(
                    labelText: 'Correo electrónico',
                    prefixIcon: Icon(Icons.mail_outline),
                  ),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Campo requerido';
                    }
                    final email = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$');
                    return email.hasMatch(value.trim())
                        ? null
                        : 'Correo inválido';
                  },
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _password,
                  obscureText: true,
                  decoration: const InputDecoration(
                    labelText: 'Contraseña',
                    prefixIcon: Icon(Icons.lock_outline),
                  ),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Campo requerido';
                    }
                    return _passwordPattern.hasMatch(value)
                        ? null
                        : 'Mínimo 8 caracteres con mayúscula, minúscula, número y símbolo';
                  },
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  initialValue: _rol,
                  decoration: const InputDecoration(labelText: 'Rol'),
                  items: _roles.keys.map(menuItem).toList(),
                  onChanged: (value) => setState(() => _rol = value ?? _rol),
                ),
                const SizedBox(height: 8),
                if (_rol == 'Paciente')
                  TextFormField(
                    controller: _tipoPaciente,
                    decoration: const InputDecoration(
                      labelText: 'Tipo de paciente',
                      prefixIcon: Icon(Icons.favorite_outline),
                    ),
                    validator: requiredField,
                  ),
                if (_rol == 'ProfesionalSalud') ...[
                  TextFormField(
                    controller: _especialidad,
                    decoration: const InputDecoration(
                      labelText: 'Especialidad',
                      prefixIcon: Icon(Icons.medical_services_outlined),
                    ),
                    validator: requiredField,
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _licencia,
                    decoration: const InputDecoration(
                      labelText: 'Número de licencia',
                      prefixIcon: Icon(Icons.assignment_ind_outlined),
                    ),
                    validator: requiredField,
                  ),
                ],
                if (_rol == 'Familiar')
                  TextFormField(
                    controller: _parentesco,
                    decoration: const InputDecoration(
                      labelText: 'Parentesco',
                      prefixIcon: Icon(Icons.family_restroom),
                    ),
                    validator: requiredField,
                  ),
                if (_error != null) ...[
                  const SizedBox(height: 12),
                  Text(_error!, style: const TextStyle(color: Colors.red)),
                ],
              ],
            ),
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: _saving ? null : () => Navigator.pop(context),
          child: const Text('Cancelar'),
        ),
        FilledButton.icon(
          onPressed: _saving ? null : _guardar,
          icon: _saving
              ? const SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Icon(Icons.save_outlined),
          label: const Text('Crear'),
        ),
      ],
    );
  }
}
