import 'model_helpers.dart';

class HealthIndicator {
  HealthIndicator({
    required this.id,
    required this.name,
    required this.unit,
    required this.value,
    this.secondaryValue,
    required this.measuredAt,
    required this.source,
    required this.notes,
    this.patientId = 1,
    this.typeId = 1,
    this.userRegistrationId,
  });

  factory HealthIndicator.fromJson(Map<String, dynamic> json) {
    return HealthIndicator(
      id: asInt(json['id'] ?? json['idIndicadorSalud']) ?? 0,
      name:
          asString(json['nombreIndicador']) ??
          asString(json['tipoIndicador']) ??
          indicatorName(asInt(json['idTipoIndicador']) ?? 1),
      unit:
          asString(json['unidadMedida']) ??
          indicatorUnit(asInt(json['idTipoIndicador']) ?? 1),
      value: asDouble(json['valor']) ?? 0,
      secondaryValue: asDouble(json['valorSecundario']),
      measuredAt: asDate(json['fechaHoraMedicion']) ?? DateTime.now(),
      source: asString(json['fuente']) ?? 'Manual',
      notes: asString(json['observaciones']) ?? '',
      patientId: asInt(json['idPaciente']) ?? 1,
      typeId: asInt(json['idTipoIndicador']) ?? 1,
      userRegistrationId: asInt(json['idUsuarioRegistro']),
    );
  }

  final int id;
  final String name;
  final String unit;
  final double value;
  final double? secondaryValue;
  final DateTime measuredAt;
  final String source;
  final String notes;
  final int patientId;
  final int typeId;
  final int? userRegistrationId;

  String get displayValue {
    if (secondaryValue == null) {
      return '${formatNumber(value)} $unit';
    }

    return '${formatNumber(value)}/${formatNumber(secondaryValue!)} $unit';
  }

  Map<String, dynamic> toCreateJson() => {
    'idPaciente': patientId,
    'idTipoIndicador': typeId,
    'valor': value,
    'valorSecundario': secondaryValue,
    'fechaHoraMedicion': measuredAt.toIso8601String(),
    'idUsuarioRegistro': userRegistrationId,
    'observaciones': notes,
    'fuente': source,
  };
}
