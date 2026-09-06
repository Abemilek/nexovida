import 'model_helpers.dart';

class Reminder {
  Reminder({
    required this.id,
    required this.title,
    required this.description,
    required this.type,
    required this.scheduledAt,
    required this.repeats,
    required this.frequency,
    required this.status,
    this.patientId = 1,
    this.treatmentMedicationId,
    this.appointmentId,
  });

  factory Reminder.fromJson(Map<String, dynamic> json) {
    return Reminder(
      id: asInt(json['id'] ?? json['idRecordatorio']) ?? 0,
      title: asString(json['titulo']) ?? 'Recordatorio',
      description: asString(json['descripcion']) ?? '',
      type: asString(json['tipoRecordatorio']) ?? 'General',
      scheduledAt: asDate(json['fechaHoraProgramada']) ?? DateTime.now(),
      repeats: json['repetir'] == true,
      frequency: asString(json['frecuenciaRepeticion']) ?? '',
      status: asString(json['estadoRecordatorio']) ?? 'Pendiente',
      patientId: asInt(json['idPaciente']) ?? 1,
      treatmentMedicationId: asInt(json['idTratamientoMedicamento']),
      appointmentId: asInt(json['idCita']),
    );
  }

  final int id;
  final String title;
  final String description;
  final String type;
  final DateTime scheduledAt;
  final bool repeats;
  final String frequency;
  final String status;
  final int patientId;
  final int? treatmentMedicationId;
  final int? appointmentId;

  Reminder copyWith({
    int? id,
    String? title,
    String? description,
    String? type,
    DateTime? scheduledAt,
    bool? repeats,
    String? frequency,
    String? status,
    int? patientId,
    int? treatmentMedicationId,
    int? appointmentId,
  }) {
    return Reminder(
      id: id ?? this.id,
      title: title ?? this.title,
      description: description ?? this.description,
      type: type ?? this.type,
      scheduledAt: scheduledAt ?? this.scheduledAt,
      repeats: repeats ?? this.repeats,
      frequency: frequency ?? this.frequency,
      status: status ?? this.status,
      patientId: patientId ?? this.patientId,
      treatmentMedicationId:
          treatmentMedicationId ?? this.treatmentMedicationId,
      appointmentId: appointmentId ?? this.appointmentId,
    );
  }

  Map<String, dynamic> toCreateJson() => {
    'idPaciente': patientId,
    'idTratamientoMedicamento': treatmentMedicationId,
    'idCita': appointmentId,
    'titulo': title,
    'descripcion': description,
    'tipoRecordatorio': type,
    'fechaHoraProgramada': scheduledAt.toIso8601String(),
    'repetir': repeats,
    'frecuenciaRepeticion': frequency,
  };

  Map<String, dynamic> toUpdateJson({String? status}) => {
    'idTratamientoMedicamento': treatmentMedicationId,
    'idCita': appointmentId,
    'titulo': title,
    'descripcion': description,
    'tipoRecordatorio': type,
    'fechaHoraProgramada': scheduledAt.toIso8601String(),
    'repetir': repeats,
    'frecuenciaRepeticion': frequency,
    'estadoRecordatorio': status ?? this.status,
    'fechaCompletado': status == 'Completado'
        ? DateTime.now().toIso8601String()
        : null,
    'activo': true,
  };
}
