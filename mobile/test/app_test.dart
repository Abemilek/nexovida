import 'package:flutter_test/flutter_test.dart';
import 'package:nexovida_app/main.dart';

void main() {
  testWidgets('la app arranca y muestra el título', (tester) async {
    await tester.pumpWidget(const NexoVidaApp());
    expect(find.text('NexoVida'), findsWidgets);
  });
}