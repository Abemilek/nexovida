import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:qr_flutter/qr_flutter.dart';

import '../services/api_client.dart';
import 'app_theme.dart';

class SecurityDialog extends StatefulWidget {
  const SecurityDialog({super.key, required this.apiClient});

  final dynamic apiClient;

  @override
  State<SecurityDialog> createState() => _SecurityDialogState();
}

enum _Step { loading, disabled, setup, ready }

class _SecurityDialogState extends State<SecurityDialog> {
  bool _enabled = false;
  _Step _step = _Step.loading;
  bool _busy = false;
  String? _error;
  String _secret = '';
  String _uri = '';
  final _codeController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadStatus();
  }

  @override
  void dispose() {
    _codeController.dispose();
    super.dispose();
  }

  Future<void> _loadStatus() async {
    try {
      final me = await widget.apiClient.get('/api/auth/me');
      _apply(() {
        _enabled = me['twoFactorEnabled'] == true;
        _step = _enabled ? _Step.ready : _Step.disabled;
      });
    } on ApiException catch (error) {
      _apply(() {
        _error = error.message;
        _step = _Step.disabled;
      });
    }
  }

  Future<void> _startSetup() async {
    _apply(() {
      _busy = true;
      _error = null;
    });
    try {
      final setup = await widget.apiClient.setupTwoFactor();
      _apply(() {
        _secret = setup['secret'] ?? '';
        _uri = setup['provisioningUri'] ?? '';
        _step = _Step.setup;
      });
    } on ApiException catch (error) {
      _apply(() => _error = error.message);
    } finally {
      _apply(() => _busy = false);
    }
  }

  Future<void> _confirmSetup() async {
    final code = _codeController.text.trim();
    if (code.length != 6) {
      _apply(() => _error = 'Ingresa el código de 6 dígitos de tu app.');
      return;
    }
    _apply(() {
      _busy = true;
      _error = null;
    });
    try {
      await widget.apiClient.verifyTwoFactor(code);
      _apply(() {
        _enabled = true;
        _step = _Step.ready;
      });
    } on ApiException catch (error) {
      _apply(() => _error = error.message);
    } finally {
      _apply(() => _busy = false);
    }
  }

  Future<void> _disable() async {
    _apply(() {
      _busy = true;
      _error = null;
    });
    try {
      await widget.apiClient.disableTwoFactor();
      _apply(() {
        _enabled = false;
        _step = _Step.disabled;
      });
    } on ApiException catch (error) {
      _apply(() => _error = error.message);
    } finally {
      _apply(() => _busy = false);
    }
  }

  void _apply(VoidCallback change) {
    if (!mounted) return;
    setState(change);
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Row(
        children: [
          Icon(Icons.shield_outlined),
          SizedBox(width: 10),
          Text('Seguridad · 2FA'),
        ],
      ),
      content: SizedBox(
        width: 380,
        child: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              if (_error != null)
                Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: _MessageBox(error: _error!),
                ),
              switch (_step) {
                _Step.loading => const Padding(
                  padding: EdgeInsets.symmetric(vertical: 24),
                  child: Center(child: CircularProgressIndicator()),
                ),
                _Step.disabled => _disabledContent(),
                _Step.setup => _setupContent(context),
                _Step.ready => _enabledContent(),
              },
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Cerrar'),
        ),
        if (_step == _Step.disabled)
          FilledButton(
            onPressed: _busy ? null : _startSetup,
            child: _busy
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Text('Activar 2FA'),
          ),
        if (_step == _Step.setup)
          FilledButton(
            onPressed: _busy ? null : _confirmSetup,
            child: _busy
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Text('Verificar y activar'),
          ),
        if (_step == _Step.ready)
          OutlinedButton.icon(
            onPressed: _busy ? null : _disable,
            icon: const Icon(Icons.lock_open_outlined),
            label: _busy
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Text('Desactivar 2FA'),
          ),
      ],
    );
  }

  Widget _disabledContent() {
    return const Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'El doble factor de autenticación está desactivado.',
          style: TextStyle(fontSize: 14, height: 1.4),
        ),
        SizedBox(height: 12),
        Text(
          'Al activarlo, además de tu contraseña se te pedirá un código de 6 dígitos '
          'que genera una app autenticadora (Google Authenticator o Authy). Todo '
          'funciona localmente: el código se calcula en tu celular sin internet.',
          style: TextStyle(fontSize: 13, color: AppTheme.inkMuted, height: 1.4),
        ),
      ],
    );
  }

  Widget _setupContent(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Text(
          'Escanea el código con tu app autenticadora:',
          style: TextStyle(fontSize: 14, height: 1.4),
        ),
        const SizedBox(height: 14),
        Center(
          child: DecoratedBox(
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.black12),
            ),
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: QrImageView(data: _uri, size: 210),
            ),
          ),
        ),
        const SizedBox(height: 14),
        const Text(
          '¿No puedes escanearlo? Ingresa este secreto manualmente:',
          style: TextStyle(fontSize: 13, color: AppTheme.inkMuted),
        ),
        const SizedBox(height: 6),
        Row(
          children: [
            Expanded(
              child: SelectableText(
                _secret,
                style: const TextStyle(
                  fontFamily: 'monospace',
                  fontSize: 14,
                  letterSpacing: 2,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
            IconButton(
              tooltip: 'Copiar secreto',
              icon: const Icon(Icons.copy, size: 18),
              onPressed: () => Clipboard.setData(ClipboardData(text: _secret)),
            ),
          ],
        ),
        const SizedBox(height: 16),
        TextFormField(
          controller: _codeController,
          keyboardType: TextInputType.number,
          maxLength: 6,
          autofocus: true,
          decoration: const InputDecoration(
            labelText: 'Código de 6 dígitos',
            prefixIcon: Icon(Icons.pin_outlined),
            counterText: '',
          ),
          onChanged: (_) => _apply(() => _error = null),
        ),
      ],
    );
  }

  Widget _enabledContent() {
    return const Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          children: [
            Icon(Icons.verified_user_outlined, color: Colors.green),
            SizedBox(width: 10),
            Expanded(
              child: Text(
                'El doble factor está activo.',
                style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600),
              ),
            ),
          ],
        ),
        SizedBox(height: 12),
        Text(
          'En cada inicio de sesión se te pedirá el código de 6 dígitos de tu app '
          'autenticadora. Para cambiarlo, desactívalo y vuelve a configurarlo.',
          style: TextStyle(fontSize: 13, color: AppTheme.inkMuted, height: 1.4),
        ),
      ],
    );
  }
}

class _MessageBox extends StatelessWidget {
  const _MessageBox({required this.error});

  final String error;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Theme.of(context).colorScheme.errorContainer,
      borderRadius: BorderRadius.circular(8),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            Icon(
              Icons.error_outline,
              color: Theme.of(context).colorScheme.error,
            ),
            const SizedBox(width: 10),
            Expanded(child: Text(error)),
          ],
        ),
      ),
    );
  }
}
