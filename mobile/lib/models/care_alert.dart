import 'model_helpers.dart';

class CareAlert {
  CareAlert({
    required this.id,
    required this.title,
    required this.message,
    required this.type,
    required this.priority,
    required this.attended,
    required this.createdAt,
    this.patientId = 1,
    this.healthIndicatorId,
    this.reminderId,
  });

  factory CareAlert.fromJson(Map<String, dynamic> json) {
    return CareAlert(
      id: asInt(json['id'] ?? json['idAlerta']) ?? 0,
      title: asString(json['titulo']) ?? 'Alerta',
      message: asString(json['mensaje']) ?? '',
      type: asString(json['tipoAlerta']) ?? 'Preventiva',
      priority: asString(json['nivelPrioridad']) ?? 'Media',
      attended: json['atendida'] == true,
      createdAt:
          asDate(json['fechaLectura']) ??
          asDate(json['fechaAtencion']) ??
          DateTime.now(),
      patientId: asInt(json['idPaciente']) ?? 1,
      healthIndicatorId: asInt(json['idIndicadorSalud']),
      reminderId: asInt(json['idRecordatorio']),
    );
  }

  final int id;
  final String title;
  final String message;
  final String type;
  final String priority;
  final bool attended;
  final DateTime createdAt;
  final int patientId;
  final int? healthIndicatorId;
  final int? reminderId;

  Map<String, dynamic> toCreateJson() => {
    'idPaciente': patientId,
    'idIndicadorSalud': healthIndicatorId,
    'idRecordatorio': reminderId,
    'titulo': title,
    'mensaje': message,
    'tipoAlerta': type,
    'nivelPrioridad': priority,
  };
}
