String formatDate(DateTime date) {
  final day = date.day.toString().padLeft(2, '0');
  final month = date.month.toString().padLeft(2, '0');
  return '$day/$month/${date.year}';
}

String formatTime(DateTime date) {
  final hour = date.hour.toString().padLeft(2, '0');
  final minute = date.minute.toString().padLeft(2, '0');
  return '$hour:$minute';
}

String formatDateTime(DateTime date) =>
    '${formatDate(date)} ${formatTime(date)}';

String indicatorName(int id) {
  return switch (id) {
    1 => 'Presión arterial',
    2 => 'Glucosa',
    3 => 'Frecuencia cardiaca',
    4 => 'Saturación',
    _ => 'Indicador',
  };
}

String indicatorUnit(int id) {
  return switch (id) {
    1 => 'mmHg',
    2 => 'mg/dL',
    3 => 'lpm',
    4 => '%',
    _ => '',
  };
}

String formatNumber(double value) {
  if (value == value.roundToDouble()) {
    return value.toStringAsFixed(0);
  }

  return value.toStringAsFixed(1);
}

String? asString(Object? value) {
  if (value == null) return null;

  final text = value.toString();

  return text.isEmpty ? null : text;
}

int? asInt(Object? value) {
  if (value is int) return value;
  if (value is num) return value.toInt();
  if (value is String) return int.tryParse(value);

  return null;
}

double? asDouble(Object? value) {
  if (value is double) return value;
  if (value is num) return value.toDouble();

  if (value is String) {
    return double.tryParse(value.replaceAll(',', '.'));
  }

  return null;
}

DateTime? asDate(Object? value) {
  if (value is DateTime) return value;

  if (value is String && value.isNotEmpty) {
    return DateTime.tryParse(value);
  }

  return null;
}
