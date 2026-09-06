import 'package:flutter/material.dart';
import 'app_session.dart';
import 'services/api_client.dart';
import 'ui/app_theme.dart';
import 'ui/auth_screen.dart';
import 'ui/home_shell.dart';

void main() {
  runApp(const NexoVidaApp());
}

class NexoVidaApp extends StatefulWidget {
  const NexoVidaApp({super.key});

  @override
  State<NexoVidaApp> createState() => _NexoVidaAppState();
}

class _NexoVidaAppState extends State<NexoVidaApp> {
  late final AppSession _session;

  @override
  void initState() {
    super.initState();
    _session = AppSession(ApiClient());
    _session.addListener(_onSessionChanged);
  }

  @override
  void dispose() {
    _session.removeListener(_onSessionChanged);
    super.dispose();
  }

  void _onSessionChanged() => setState(() {});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'NexoVida',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light(),
      darkTheme: AppTheme.dark(),
      themeMode: ThemeMode.system,
      home: _session.isAuthenticated
          ? HomeShell(session: _session)
          : AuthScreen(session: _session),
    );
  }
}
