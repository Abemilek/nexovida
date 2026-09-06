import 'package:nexovida_app/main.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('NexoVida muestra la pantalla de acceso', (tester) async {
    await tester.pumpWidget(const NexoVidaApp());

    expect(find.text('Ingresar'), findsWidgets);
    expect(find.text('Registro'), findsOneWidget);
    expect(find.byType(Image), findsWidgets);
  });
}
