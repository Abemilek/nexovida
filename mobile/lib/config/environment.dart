class Environment {
  Environment._();

  static const String apiBaseUrl = local;
  static const String local = 'http://127.0.0.1:5005';
  static const String docker = 'http://localhost:8080';
  static const String androidEmulatorLocal = 'http://10.0.2.2:5005';
  static const String androidEmulatorDocker = 'http://10.0.2.2:8080';
}
