import 'package:flutter/material.dart';

import '../../../models/nexo_models.dart';
import '../../../services/nexo_repository.dart';
import '../../widgets/form_helpers.dart';
import '../../shared_widgets.dart';

class DialogForm extends StatelessWidget {
  const DialogForm({super.key, required this.formKey, required this.children});

  final GlobalKey<FormState> formKey;
  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 440,
      child: Form(
        key: formKey,
        child: SingleChildScrollView(
          child: Column(mainAxisSize: MainAxisSize.min, children: children),
        ),
      ),
    );
  }
}


class DateTile extends StatelessWidget {
  const DateTile({super.key, required this.date, required this.onPick});

  final DateTime date;
  final ValueChanged<DateTime> onPick;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.zero,
      leading: const Icon(Icons.schedule_outlined),
      title: Text(formatDateTime(date)),
      trailing: const Icon(Icons.edit_calendar_outlined),
      onTap: () async {
        final picked = await pickDateTime(context, date);
        if (picked != null) {
          onPick(picked);
        }
      },
    );
  }
}

Future<void> showReminderDialog(
  BuildContext context,
  NexoRepository repository,
  AppUser user, {
  Reminder? existing,
}) async {
  final formKey = GlobalKey<FormState>();
  final title = TextEditingController(text: existing?.title);
  final description = TextEditingController(text: existing?.description);
  final frequency = TextEditingController(
    text: existing?.frequency.isNotEmpty == true
        ? existing!.frequency
        : 'Diario',
  );
  var type = existing?.type ?? 'Medicamento';
  var repeats = existing?.repeats ?? true;
  var date =
      existing?.scheduledAt ?? DateTime.now().add(const Duration(hours: 2));

  await showDialog<void>(
    context: context,
    builder: (context) => StatefulBuilder(
      builder: (context, setState) => AlertDialog(
        title: Text(
          existing == null ? 'Nuevo recordatorio' : 'Editar recordatorio',
        ),
        content: DialogForm(
          formKey: formKey,
          children: [
            TextFormField(
              controller: title,
              decoration: const InputDecoration(
                labelText: 'Título',
                prefixIcon: Icon(Icons.alarm_outlined),
              ),
              validator: requiredField,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: description,
              decoration: const InputDecoration(
                labelText: 'Descripción',
                prefixIcon: Icon(Icons.notes_outlined),
              ),
              maxLines: 2,
            ),
            const SizedBox(height: 12),
            DropdownButtonFormField<String>(
              initialValue: type,
              decoration: const InputDecoration(labelText: 'Tipo'),
              items: const [
                'Medicamento',
                'Cita',
                'Control',
                'Actividad',
              ].map(menuItem).toList(),
              onChanged: (value) => setState(() => type = value ?? type),
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: frequency,
              decoration: const InputDecoration(
                labelText: 'Frecuencia',
                prefixIcon: Icon(Icons.repeat),
              ),
            ),
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              value: repeats,
              onChanged: (value) => setState(() => repeats = value),
              title: const Text('Repetir'),
            ),
            DateTile(
              date: date,
              onPick: (value) => setState(() => date = value),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancelar'),
          ),
          FilledButton.icon(
            onPressed: () async {
              if (!formKey.currentState!.validate()) return;
              final reminder = Reminder(
                id: existing?.id ?? 0,
                title: title.text.trim(),
                description: description.text.trim(),
                type: type,
                scheduledAt: date,
                repeats: repeats,
                frequency: frequency.text.trim(),
                status: existing?.status ?? 'Pendiente',
                patientId: existing?.patientId ?? user.patientId,
              );
              if (existing == null) {
                await repository.addReminder(reminder);
              } else {
                await repository.updateReminder(reminder);
              }
              if (context.mounted) Navigator.pop(context);
            },
            icon: const Icon(Icons.save_outlined),
            label: const Text('Guardar'),
          ),
        ],
      ),
    ),
  );

  title.dispose();
  description.dispose();
  frequency.dispose();
}

Future<void> showIndicatorDialog(
  BuildContext context,
  NexoRepository repository,
  AppUser user,
) async {
  final formKey = GlobalKey<FormState>();
  final value = TextEditingController();
  final secondaryValue = TextEditingController();
  final notes = TextEditingController();
  var typeId = 1;
  var date = DateTime.now();

  await showDialog<void>(
    context: context,
    builder: (context) => StatefulBuilder(
      builder: (context, setState) => AlertDialog(
        title: const Text('Registrar indicador'),
        content: DialogForm(
          formKey: formKey,
          children: [
            DropdownButtonFormField<int>(
              initialValue: typeId,
              decoration: const InputDecoration(labelText: 'Indicador'),
              items: const [
                DropdownMenuItem(value: 1, child: Text('Presión arterial')),
                DropdownMenuItem(value: 2, child: Text('Glucosa')),
                DropdownMenuItem(value: 3, child: Text('Frecuencia cardiaca')),
                DropdownMenuItem(value: 4, child: Text('Saturación')),
              ],
              onChanged: (value) => setState(() => typeId = value ?? typeId),
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: value,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Valor principal',
                prefixIcon: Icon(Icons.speed_outlined),
              ),
              validator: _numberRequired,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: secondaryValue,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Valor secundario',
                prefixIcon: Icon(Icons.timeline_outlined),
              ),
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: notes,
              decoration: const InputDecoration(
                labelText: 'Observaciones',
                prefixIcon: Icon(Icons.notes_outlined),
              ),
              maxLines: 2,
            ),
            const SizedBox(height: 12),
            DateTile(
              date: date,
              onPick: (value) => setState(() => date = value),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancelar'),
          ),
          FilledButton.icon(
            onPressed: () async {
              if (!formKey.currentState!.validate()) return;
              await repository.addIndicator(
                HealthIndicator(
                  id: 0,
                  name: _indicatorLabel(typeId),
                  unit: _indicatorUnit(typeId),
                  value: double.parse(value.text.replaceAll(',', '.')),
                  secondaryValue: secondaryValue.text.trim().isEmpty
                      ? null
                      : double.parse(secondaryValue.text.replaceAll(',', '.')),
                  measuredAt: date,
                  source: 'Manual',
                  notes: notes.text.trim(),
                  patientId: user.patientId,
                  typeId: typeId,
                  userRegistrationId: user.userId,
                ),
              );
              if (context.mounted) Navigator.pop(context);
            },
            icon: const Icon(Icons.save_outlined),
            label: const Text('Guardar'),
          ),
        ],
      ),
    ),
  );

  value.dispose();
  secondaryValue.dispose();
  notes.dispose();
}

Future<void> showAppointmentDialog(
  BuildContext context,
  NexoRepository repository,
  AppUser user,
) async {
  final formKey = GlobalKey<FormState>();
  final type = TextEditingController(text: 'Control mensual');
  final reason = TextEditingController();
  final place = TextEditingController();
  final notes = TextEditingController();
  var modality = 'Presencial';
  var date = DateTime.now().add(const Duration(days: 1));

  await showDialog<void>(
    context: context,
    builder: (context) => StatefulBuilder(
      builder: (context, setState) => AlertDialog(
        title: const Text('Agendar cita'),
        content: DialogForm(
          formKey: formKey,
          children: [
            TextFormField(
              controller: type,
              decoration: const InputDecoration(
                labelText: 'Tipo de cita',
                prefixIcon: Icon(Icons.medical_services_outlined),
              ),
              validator: requiredField,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: reason,
              decoration: const InputDecoration(
                labelText: 'Motivo',
                prefixIcon: Icon(Icons.assignment_outlined),
              ),
              maxLines: 2,
              validator: requiredField,
            ),
            const SizedBox(height: 12),
            DropdownButtonFormField<String>(
              initialValue: modality,
              decoration: const InputDecoration(labelText: 'Modalidad'),
              items: const [
                'Presencial',
                'Virtual',
                'Domicilio',
              ].map(menuItem).toList(),
              onChanged: (value) =>
                  setState(() => modality = value ?? modality),
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: place,
              decoration: const InputDecoration(
                labelText: 'Lugar',
                prefixIcon: Icon(Icons.place_outlined),
              ),
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: notes,
              decoration: const InputDecoration(
                labelText: 'Observaciones',
                prefixIcon: Icon(Icons.notes_outlined),
              ),
            ),
            const SizedBox(height: 12),
            DateTile(
              date: date,
              onPick: (value) => setState(() => date = value),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancelar'),
          ),
          FilledButton.icon(
            onPressed: () async {
              if (!formKey.currentState!.validate()) return;
              await repository.addAppointment(
                Appointment(
                  id: 0,
                  type: type.text.trim(),
                  reason: reason.text.trim(),
                  modality: modality,
                  place: place.text.trim(),
                  status: 'Programada',
                  startsAt: date,
                  endsAt: date.add(const Duration(minutes: 30)),
                  notes: notes.text.trim(),
                  patientId: user.patientId,
                ),
              );
              if (context.mounted) Navigator.pop(context);
            },
            icon: const Icon(Icons.save_outlined),
            label: const Text('Guardar'),
          ),
        ],
      ),
    ),
  );

  type.dispose();
  reason.dispose();
  place.dispose();
  notes.dispose();
}

Future<void> showClinicalEventDialog(
  BuildContext context,
  NexoRepository repository,
  AppUser user,
) async {
  final formKey = GlobalKey<FormState>();
  final title = TextEditingController();
  final description = TextEditingController();
  var type = 'Consulta Médica';
  var date = DateTime.now();

  await showDialog<void>(
    context: context,
    builder: (context) => StatefulBuilder(
      builder: (context, setState) => AlertDialog(
        title: const Text('Evento clínico'),
        content: DialogForm(
          formKey: formKey,
          children: [
            DropdownButtonFormField<String>(
              initialValue: type,
              decoration: const InputDecoration(labelText: 'Tipo de evento'),
              items: const [
                'Consulta Médica',
                'Laboratorio',
                'Procedimiento',
                'Hospitalización',
                'Seguimiento',
              ].map(menuItem).toList(),
              onChanged: (value) => setState(() => type = value ?? type),
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: title,
              decoration: const InputDecoration(
                labelText: 'Título',
                prefixIcon: Icon(Icons.title_outlined),
              ),
              validator: requiredField,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: description,
              decoration: const InputDecoration(
                labelText: 'Descripción',
                prefixIcon: Icon(Icons.notes_outlined),
              ),
              maxLines: 3,
            ),
            const SizedBox(height: 12),
            DateTile(
              date: date,
              onPick: (value) => setState(() => date = value),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancelar'),
          ),
          FilledButton.icon(
            onPressed: () async {
              if (!formKey.currentState!.validate()) return;
              await repository.addClinicalEvent(
                ClinicalEvent(
                  id: 0,
                  type: type,
                  title: title.text.trim(),
                  description: description.text.trim(),
                  eventAt: date,
                  patientId: user.patientId,
                  userId: user.userId,
                ),
              );
              if (context.mounted) Navigator.pop(context);
            },
            icon: const Icon(Icons.save_outlined),
            label: const Text('Guardar'),
          ),
        ],
      ),
    ),
  );

  title.dispose();
  description.dispose();
}


String? _numberRequired(String? value) {
  final parsed = double.tryParse((value ?? '').replaceAll(',', '.'));
  if (parsed == null || parsed < 0 || parsed > 9999) {
    return 'Ingresa un número entre 0 y 9999';
  }
  return null;
}

String _indicatorLabel(int id) {
  return switch (id) {
    1 => 'Presión arterial',
    2 => 'Glucosa',
    3 => 'Frecuencia cardiaca',
    4 => 'Saturación',
    _ => 'Indicador',
  };
}

String _indicatorUnit(int id) {
  return switch (id) {
    1 => 'mmHg',
    2 => 'mg/dL',
    3 => 'lpm',
    4 => '%',
    _ => '',
  };
}
