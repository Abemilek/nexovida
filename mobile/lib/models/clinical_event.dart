import 'model_helpers.dart';

class ClinicalEvent {
  ClinicalEvent({
    required this.id,
    required this.type,
    required this.title,
    required this.description,
    required this.eventAt,
    this.patientId = 1,
    this.userId,
  });

  factory ClinicalEvent.fromJson(Map<String, dynamic> json) {
    return ClinicalEvent(
      id: asInt(json['id'] ?? json['idHistorialPaciente']) ?? 0,
      type: asString(json['tipoEvento']) ?? 'Evento clínico',
      title: asString(json['titulo']) ?? 'Seguimiento',
      description: asString(json['descripcion']) ?? '',
      eventAt: asDate(json['fechaEvento']) ?? DateTime.now(),
      patientId: asInt(json['idPaciente']) ?? 1,
      userId: asInt(json['idUsuario']),
    );
  }

  final int id;
  final String type;
  final String title;
  final String description;
  final DateTime eventAt;
  final int patientId;
  final int? userId;

  Map<String, dynamic> toCreateJson() => {
    'idPaciente': patientId,
    'idUsuario': userId,
    'tipoEvento': type,
    'fechaEvento': eventAt.toIso8601String(),
    'titulo': title,
    'descripcion': description,
  };
}
