import 'package:flutter/material.dart';

import '../app_session.dart';
import 'app_theme.dart';

class AuthScreen extends StatefulWidget {
  const AuthScreen({super.key, required this.session});

  final AppSession session;

  @override
  State<AuthScreen> createState() => _AuthScreenState();
}

class _AuthScreenState extends State<AuthScreen> with TickerProviderStateMixin {
  late final TabController _tabController;
  late final AnimationController _introController;
  late final Animation<double> _introFade;
  late final Animation<Offset> _introSlide;
  final _loginFormKey = GlobalKey<FormState>();
  final _registerFormKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _totpController = TextEditingController();
  final _usernameController = TextEditingController();
  final _registerEmailController = TextEditingController();
  final _registerPasswordController = TextEditingController();
  bool _obscurePassword = true;
  bool _obscureRegisterPassword = true;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _tabController.addListener(_onTabChanged);
    widget.session.addListener(_onSessionChanged);

    _introController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 420),
    );
    _introFade = CurvedAnimation(
      parent: _introController,
      curve: Curves.easeOut,
    );
    _introSlide = Tween<Offset>(
      begin: const Offset(0, 0.03),
      end: Offset.zero,
    ).animate(_introFade);
    _introController.forward();
  }

  @override
  void dispose() {
    widget.session.removeListener(_onSessionChanged);
    _tabController.removeListener(_onTabChanged);
    _tabController.dispose();
    _introController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _totpController.dispose();
    _usernameController.dispose();
    _registerEmailController.dispose();
    _registerPasswordController.dispose();
    super.dispose();
  }

  void _onSessionChanged() => setState(() {});

  void _onTabChanged() {
    if (!_tabController.indexIsChanging) {
      setState(() {});
    }
  }

  @override
  Widget build(BuildContext context) {
    final wide = MediaQuery.sizeOf(context).width >= 840;
    return Theme(
      data: AppTheme.light(),
      child: Scaffold(
        body: Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [Colors.white, AppTheme.surfaceTint],
            ),
          ),
          child: SafeArea(
            child: Center(
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 1080),
                child: Padding(
                  padding: const EdgeInsets.all(18),
                  child: FadeTransition(
                    opacity: _introFade,
                    child: SlideTransition(
                      position: _introSlide,
                      child: wide
                          ? Row(
                              children: [
                                const Expanded(child: _BrandPanel()),
                                const SizedBox(width: 28),
                                SizedBox(width: 420, child: _authCard()),
                              ],
                            )
                          : ListView(
                              children: [
                                const _BrandPanel(compact: true),
                                const SizedBox(height: 18),
                                _authCard(),
                              ],
                            ),
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _authCard() {
    return Container(
      padding: const EdgeInsets.all(26),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppTheme.radius + 6),
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryDark.withValues(alpha: 0.06),
            blurRadius: 24,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            'Acceso',
            style: Theme.of(
              context,
            ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w800),
          ),
          const SizedBox(height: 4),
          Text(
            'Ingresa con tu cuenta institucional.',
            style: Theme.of(
              context,
            ).textTheme.bodyMedium?.copyWith(color: AppTheme.inkMuted),
          ),
          const SizedBox(height: 20),
          TabBar(
            controller: _tabController,
            tabs: const [
              Tab(text: 'Ingresar'),
              Tab(text: 'Registro'),
            ],
          ),
          const SizedBox(height: 18),
          AnimatedSize(
            duration: const Duration(milliseconds: 180),
            curve: Curves.easeOut,
            child: _tabController.index == 0 ? _loginForm() : _registerForm(),
          ),
        ],
      ),
    );
  }

  Widget _loginForm() {
    return Form(
      key: _loginFormKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (widget.session.lastError != null)
            _ErrorBanner(message: widget.session.lastError!),
          TextFormField(
            controller: _emailController,
            keyboardType: TextInputType.emailAddress,
            autofillHints: const [AutofillHints.email],
            decoration: const InputDecoration(
              labelText: 'Correo',
              prefixIcon: Icon(Icons.mail_outline),
            ),
            validator: _emailValidator,
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: _passwordController,
            obscureText: _obscurePassword,
            autofillHints: const [AutofillHints.password],
            decoration: InputDecoration(
              labelText: 'Contraseña',
              prefixIcon: const Icon(Icons.lock_outline),
              suffixIcon: IconButton(
                tooltip: _obscurePassword ? 'Mostrar' : 'Ocultar',
                onPressed: () =>
                    setState(() => _obscurePassword = !_obscurePassword),
                icon: Icon(
                  _obscurePassword
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                ),
              ),
            ),
            validator: (value) => (value == null || value.isEmpty)
                ? 'Ingresa tu contraseña'
                : null,
          ),
          if (widget.session.requiresTwoFactor) ...[
            const SizedBox(height: 12),
            TextFormField(
              controller: _totpController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Código 2FA',
                prefixIcon: Icon(Icons.password_outlined),
              ),
              validator: (value) {
                if (!widget.session.requiresTwoFactor) {
                  return null;
                }
                if (value == null || value.length != 6) {
                  return 'El código debe tener 6 dígitos';
                }
                return null;
              },
            ),
          ],
          const SizedBox(height: 16),
          FilledButton.icon(
            onPressed: widget.session.isBusy ? null : _submitLogin,
            style: FilledButton.styleFrom(
              minimumSize: const Size.fromHeight(52),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(AppTheme.radius),
              ),
              textStyle: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w700,
              ),
            ),
            icon: widget.session.isBusy
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.login),
            label: Text(
              widget.session.requiresTwoFactor
                  ? 'Verificar e ingresar'
                  : 'Ingresar',
            ),
          ),
        ],
      ),
    );
  }

  Widget _registerForm() {
    return Form(
      key: _registerFormKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (widget.session.lastError != null)
            _ErrorBanner(message: widget.session.lastError!),
          TextFormField(
            controller: _usernameController,
            decoration: const InputDecoration(
              labelText: 'Nombre de usuario',
              prefixIcon: Icon(Icons.badge_outlined),
            ),
            validator: (value) => (value == null || value.trim().length < 3)
                ? 'Mínimo 3 caracteres'
                : null,
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: _registerEmailController,
            keyboardType: TextInputType.emailAddress,
            decoration: const InputDecoration(
              labelText: 'Correo',
              prefixIcon: Icon(Icons.mail_outline),
            ),
            validator: _emailValidator,
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: _registerPasswordController,
            obscureText: _obscureRegisterPassword,
            autofillHints: const [AutofillHints.newPassword],
            decoration: InputDecoration(
              labelText: 'Contraseña segura',
              prefixIcon: const Icon(Icons.lock_outline),
              suffixIcon: IconButton(
                tooltip: _obscureRegisterPassword ? 'Mostrar' : 'Ocultar',
                onPressed: () => setState(
                  () => _obscureRegisterPassword = !_obscureRegisterPassword,
                ),
                icon: Icon(
                  _obscureRegisterPassword
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                ),
              ),
            ),
            validator: _passwordValidator,
          ),
          const SizedBox(height: 16),
          FilledButton.icon(
            onPressed: widget.session.isBusy ? null : _submitRegister,
            style: FilledButton.styleFrom(
              minimumSize: const Size.fromHeight(52),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(AppTheme.radius),
              ),
              textStyle: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w700,
              ),
            ),
            icon: const Icon(Icons.person_add_alt_1),
            label: const Text('Crear cuenta'),
          ),
        ],
      ),
    );
  }

  Future<void> _submitLogin() async {
    if (!_loginFormKey.currentState!.validate()) return;
    final ok = widget.session.requiresTwoFactor
        ? await widget.session.confirmTwoFactor(_totpController.text.trim())
        : await widget.session.login(
            email: _emailController.text.trim(),
            password: _passwordController.text,
          );
    if (!ok && mounted && widget.session.requiresTwoFactor) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Ingresa el código de 6 dígitos.')),
      );
    }
  }

  Future<void> _submitRegister() async {
    if (!_registerFormKey.currentState!.validate()) return;
    final ok = await widget.session.register(
      username: _usernameController.text.trim(),
      email: _registerEmailController.text.trim(),
      password: _registerPasswordController.text,
    );
    if (!mounted) return;
    if (ok) {
      _emailController.text = _registerEmailController.text.trim();
      _passwordController.text = _registerPasswordController.text;
      _tabController.animateTo(0);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Cuenta creada. Puedes ingresar.')),
      );
    }
  }

  String? _emailValidator(String? value) {
    final text = value?.trim() ?? '';
    if (!RegExp(r'^[^@]+@[^@]+\.[^@]+$').hasMatch(text)) {
      return 'Ingresa un correo válido';
    }
    return null;
  }

  String? _passwordValidator(String? value) {
    final text = value ?? '';
    final valid = RegExp(
      r'^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$',
    ).hasMatch(text);
    if (!valid) {
      return 'Mínimo 8 caracteres, mayúscula, minúscula, número y especial';
    }
    return null;
  }
}

class _BrandPanel extends StatelessWidget {
  const _BrandPanel({this.compact = false});

  final bool compact;

  @override
  Widget build(BuildContext context) {
    return Container(
      constraints: BoxConstraints(minHeight: compact ? 210 : 520),
      padding: EdgeInsets.all(compact ? 28 : 36),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppTheme.radius + 6),
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryDark.withValues(
              alpha: compact ? 0.02 : 0.05,
            ),
            blurRadius: 30,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        mainAxisAlignment: MainAxisAlignment.center,
        children: compact ? _buildCompact(context) : _buildWide(context),
      ),
    );
  }

  List<Widget> _buildCompact(BuildContext context) {
    return [
      Image.asset('assets/images/nexovida-logo.png', width: 64),
      const SizedBox(height: 14),
      const _BrandWordmark(size: 28),
      const SizedBox(height: 4),
      const Text(
        'MONITOREO INTELIGENTE',
        style: TextStyle(
          color: AppTheme.inkMuted,
          fontSize: 11,
          letterSpacing: 4,
          fontWeight: FontWeight.w600,
        ),
      ),
      const SizedBox(height: 10),
      const Text(
        'Seguimiento continuo para pacientes crónicos, cuidadores y personal de salud.',
        textAlign: TextAlign.center,
        style: TextStyle(color: AppTheme.inkMuted, height: 1.4, fontSize: 14),
      ),
    ];
  }

  List<Widget> _buildWide(BuildContext context) {
    return [
      Image.asset('assets/images/nexovida-logo.png', width: 340),
      const SizedBox(height: 18),
      const Text(
        'Seguimiento continuo para pacientes crónicos, cuidadores y personal de salud.',
        textAlign: TextAlign.center,
        style: TextStyle(
          color: AppTheme.inkMuted,
          fontSize: 16,
          height: 1.45,
          fontWeight: FontWeight.w400,
        ),
      ),
      const SizedBox(height: 26),
      const Wrap(
        alignment: WrapAlignment.center,
        spacing: 10,
        runSpacing: 10,
        children: [
          _FeatureChip(icon: Icons.alarm_outlined, label: 'Recordatorios'),
          _FeatureChip(
            icon: Icons.monitor_heart_outlined,
            label: 'Indicadores',
          ),
          _FeatureChip(icon: Icons.event_available_outlined, label: 'Citas'),
          _FeatureChip(icon: Icons.warning_amber_outlined, label: 'Alertas'),
        ],
      ),
    ];
  }
}

class _BrandWordmark extends StatelessWidget {
  const _BrandWordmark({required this.size});

  final double size;

  @override
  Widget build(BuildContext context) {
    return Text.rich(
      TextSpan(
        style: TextStyle(
          color: AppTheme.ink,
          fontSize: size,
          fontWeight: FontWeight.w800,
          letterSpacing: 2,
        ),
        children: const [
          TextSpan(text: 'Nexo'),
          TextSpan(
            text: 'Vida',
            style: TextStyle(color: AppTheme.primary),
          ),
        ],
      ),
    );
  }
}

class _FeatureChip extends StatelessWidget {
  const _FeatureChip({required this.icon, required this.label});

  final IconData icon;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Chip(
      avatar: Icon(icon, size: 18),
      label: Text(label),
      side: BorderSide(color: Theme.of(context).colorScheme.outlineVariant),
      backgroundColor: Theme.of(context).colorScheme.surface,
    );
  }
}

class _ErrorBanner extends StatelessWidget {
  const _ErrorBanner({required this.message});
  final String message;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Material(
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
              Expanded(child: Text(message)),
            ],
          ),
        ),
      ),
    );
  }
}
