import 'package:flutter/material.dart';

DropdownMenuItem<T> menuItem<T>(T value) =>
    DropdownMenuItem(value: value, child: Text(value.toString()));

String? requiredField(String? value) {
  if (value == null || value.trim().isEmpty) {
    return 'Campo requerido';
  }
  return null;
}

int metricValue(Map<String, dynamic> metrics, String camelKey) {
  final pascalKey = camelKey[0].toUpperCase() + camelKey.substring(1);
  final value = metrics[camelKey] ?? metrics[pascalKey];
  if (value is int) return value;
  return int.tryParse(value?.toString() ?? '') ?? 0;
}

String textValue(
  Map<String, dynamic> values,
  List<String> keys,
  String fallback,
) {
  for (final key in keys) {
    final value = values[key]?.toString().trim();
    if (value != null && value.isNotEmpty) return value;
  }
  return fallback;
}
