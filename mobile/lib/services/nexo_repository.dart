import 'package:flutter/foundation.dart';
import 'package:nexovida_app/models/appointment.dart';
import 'package:nexovida_app/models/care_alert.dart';
import 'package:nexovida_app/models/clinical_event.dart';
import 'package:nexovida_app/models/health_indicator.dart';
import 'package:nexovida_app/models/reminder.dart';
import 'api_client.dart';

class NexoRepository extends ChangeNotifier {
  NexoRepository(this._apiClient);

  final ApiClient _apiClient;

  bool isLoading = false;
  String? lastError;

  final List<Reminder> reminders = [];
  final List<HealthIndicator> indicators = [];
  final List<Appointment> appointments = [];
  final List<CareAlert> alerts = [];
  final List<ClinicalEvent> history = [];

  Future<void> refresh() async {
    isLoading = true;
    lastError = null;
    notifyListeners();

    final results = await Future.wait<_CollectionResult>([
      _tryCollection('/api/Recordatorio'),
      _tryCollection('/api/IndicadorSalud'),
      _tryCollection('/api/Cita'),
      _tryCollection('/api/Alerta'),
      _tryCollection('/api/HistorialPaciente'),
    ]);

    final reachedBackend = results.any((r) => r.ok);

    if (reachedBackend) {
      reminders
        ..clear()
        ..addAll(results[0].data.map(Reminder.fromJson));

      indicators
        ..clear()
        ..addAll(results[1].data.map(HealthIndicator.fromJson));

      appointments
        ..clear()
        ..addAll(results[2].data.map(Appointment.fromJson));

      alerts
        ..clear()
        ..addAll(results[3].data.map(CareAlert.fromJson));

      history
        ..clear()
        ..addAll(results[4].data.map(ClinicalEvent.fromJson));
    } else {
      lastError = 'Sin conexión con el backend. Verifica que esté encendido.';
    }

    isLoading = false;
    notifyListeners();
  }

  void clearLocalData() {
    reminders.clear();
    indicators.clear();
    appointments.clear();
    alerts.clear();
    history.clear();
    lastError = null;
    notifyListeners();
  }

  Future<void> addReminder(Reminder reminder) async {
    try {
      final body = reminder.toCreateJson();
      final response = await _apiClient.post('/api/Recordatorio', body);
      final serverReminder = Reminder.fromJson(response);

      reminders.insert(0, serverReminder);
      lastError = null;
    } on ApiException catch (e) {
      lastError = e.message;
    } finally {
      notifyListeners();
    }
  }

  Future<void> addIndicator(HealthIndicator indicator) async {
    try {
      final body = indicator.toCreateJson();
      final response = await _apiClient.post('/api/IndicadorSalud', body);
      final serverIndicator = HealthIndicator.fromJson(response);

      indicators.insert(0, serverIndicator);
      _createPreventiveAlertIfNeeded(serverIndicator);
      lastError = null;
    } on ApiException catch (e) {
      lastError = e.message;
    } finally {
      notifyListeners();
    }
  }

  Future<void> addAppointment(Appointment appointment) async {
    try {
      final body = appointment.toCreateJson();
      final response = await _apiClient.post('/api/Cita', body);
      final serverAppointment = Appointment.fromJson(response);

      appointments.insert(0, serverAppointment);
      lastError = null;
    } on ApiException catch (e) {
      lastError = e.message;
    } finally {
      notifyListeners();
    }
  }

  Future<void> addClinicalEvent(ClinicalEvent event) async {
    try {
      final body = event.toCreateJson();
      final response = await _apiClient.post('/api/HistorialPaciente', body);
      final serverEvent = ClinicalEvent.fromJson(response);

      history.insert(0, serverEvent);
      lastError = null;
    } on ApiException catch (e) {
      lastError = e.message;
    } finally {
      notifyListeners();
    }
  }

  Future<void> completeReminder(Reminder reminder) async {
    final index = reminders.indexWhere((item) => item.id == reminder.id);

    if (index == -1) return;

    final previous = reminders[index];

    reminders[index] = reminder.copyWith(status: 'Completado');

    notifyListeners();

    try {
      await _apiClient.post('/api/Recordatorio/${reminder.id}/completar', {});

      lastError = null;
    } on ApiException catch (e) {
      reminders[index] = previous;
      lastError = e.message;
    } finally {
      notifyListeners();
    }
  }

  Future<void> updateReminder(Reminder reminder) async {
    final index = reminders.indexWhere((item) => item.id == reminder.id);

    if (index == -1) return;

    final previous = reminders[index];

    try {
      final response = await _apiClient.put(
        '/api/Recordatorio/${reminder.id}',
        reminder.toUpdateJson(),
      );

      reminders[index] = Reminder.fromJson(response);
      lastError = null;
    } on ApiException catch (e) {
      reminders[index] = previous;
      lastError = e.message;
    } finally {
      notifyListeners();
    }
  }

  List<Reminder> get pendingReminders {
    final list = reminders
        .where((item) => item.status.toLowerCase() != 'completado')
        .toList();

    list.sort((a, b) => a.scheduledAt.compareTo(b.scheduledAt));

    return list;
  }

  List<Appointment> get upcomingAppointments {
    final list = appointments
        .where(
          (item) => item.startsAt.isAfter(
            DateTime.now().subtract(const Duration(days: 1)),
          ),
        )
        .toList();

    list.sort((a, b) => a.startsAt.compareTo(b.startsAt));

    return list;
  }

  List<CareAlert> get activeAlerts =>
      alerts.where((item) => !item.attended).toList();

  double get adherenceScore {
    if (reminders.isEmpty) return 0;

    final completed = reminders
        .where((item) => item.status.toLowerCase() == 'completado')
        .length;

    return completed / reminders.length;
  }

  void _createPreventiveAlertIfNeeded(HealthIndicator indicator) {
    final pressureHigh =
        indicator.typeId == 1 &&
        (indicator.value >= 140 ||
            (indicator.secondaryValue != null &&
                indicator.secondaryValue! >= 90));

    final glucoseHigh = indicator.typeId == 2 && indicator.value >= 180;

    final oxygenLow = indicator.typeId == 4 && indicator.value < 92;

    if (!pressureHigh && !glucoseHigh && !oxygenLow) {
      return;
    }

    alerts.insert(
      0,
      CareAlert(
        id: _nextId(alerts.map((item) => item.id)),
        title: '${indicator.name} fuera de rango',
        message:
            'Se registró ${indicator.displayValue}. Se recomienda revisar el plan de seguimiento.',
        type: 'Indicador Anormal',
        priority: pressureHigh || oxygenLow ? 'Alta' : 'Media',
        attended: false,
        createdAt: DateTime.now(),
        patientId: indicator.patientId,
        healthIndicatorId: indicator.id,
      ),
    );
  }

  Future<_CollectionResult> _tryCollection(String path) async {
    try {
      return _CollectionResult(await _apiClient.getCollection(path), true);
    } on ApiException {
      return const _CollectionResult([], false);
    }
  }

  int _nextId(Iterable<int> values) {
    if (values.isEmpty) return 1;

    return values.reduce((a, b) => a > b ? a : b) + 1;
  }
}

class _CollectionResult {
  const _CollectionResult(this.data, this.ok);

  final List<Map<String, dynamic>> data;
  final bool ok;
}
