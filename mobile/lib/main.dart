import 'package:flutter/material.dart';

void main() {
  runApp(const NexoVidaApp());
}

class NexoVidaApp extends StatelessWidget {
  const NexoVidaApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'NexoVida',
      home: Scaffold(
        appBar: AppBar(title: const Text('NexoVida')),
        body: const Center(child: Text('NexoVida')),
      ),
    );
  }
}
