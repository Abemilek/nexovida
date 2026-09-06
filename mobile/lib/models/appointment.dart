import 'model_helpers.dart';

class Appointment {
  Appointment({
    required this.id,
    required this.type,
    required this.reason,
    required this.modality,
    required this.place,
    required this.status,
    required this.startsAt,
    required this.endsAt,
    required this.notes,
    this.patientId = 1,
  });

  factory Appointment.fromJson(Map<String, dynamic> json) {
    final start = asDate(json['fechaHoraInicio']) ?? DateTime.now();

    return Appointment(
      id: asInt(json['id'] ?? json['idCita']) ?? 0,
      type: asString(json['tipoCita']) ?? 'Control',
      reason: asString(json['motivo']) ?? '',
      modality: asString(json['modalidad']) ?? 'Presencial',
      place: asString(json['lugar']) ?? '',
      status: asString(json['estadoCita']) ?? 'Programada',
      startsAt: start,
      endsAt:
          asDate(json['fechaHoraFin']) ??
          start.add(const Duration(minutes: 30)),
      notes: asString(json['observaciones']) ?? '',
      patientId: asInt(json['idPaciente']) ?? 1,
    );
  }

  final int id;
  final String type;
  final String reason;
  final String modality;
  final String place;
  final String status;
  final DateTime startsAt;
  final DateTime endsAt;
  final String notes;
  final int patientId;

  Map<String, dynamic> toCreateJson() => {
    'idPaciente': patientId,
    'fechaHoraInicio': startsAt.toIso8601String(),
    'fechaHoraFin': endsAt.toIso8601String(),
    'tipoCita': type,
    'motivo': reason,
    'modalidad': modality,
    'lugar': place,
    'estadoCita': status,
    'observaciones': notes,
  };
}
