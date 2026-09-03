USE NexoVida;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

INSERT INTO Roles (NombreRol, Descripcion, Activo) VALUES
('Administrador',     'Control total del sistema', 1),                       
('Paciente',          'Usuario que recibe tratamiento y seguimiento', 1),    
('Familiar',          'Cuidador o familiar con acceso al expediente del paciente', 1), 
('ProfesionalSalud',  'Medico o profesional que da seguimiento clinico', 1);  

INSERT INTO Usuario (NombreUsuario, Correo, Contrasena, Salt, FechaRegistro, UltimoAcceso, Activo) VALUES
('admin',      'admin@nexovida.com',     0x9B9D93E2DCC9EA55BC318E0052DB56C328E858205028CC24260E8E5CD0AB2B5A, 0xE9A672F79187AB1ADAB224A22A5578DF, GETDATE(), GETDATE(), 1), 
('mgonzalez',  'mgonzalez@correo.com',   0xFC8E2764E23BAAEF0B3DE15E96C0D31E08B4401AEADE538F8182A855E04425C9, 0x6A5F1D32E09FA82C3607C5A03E9BD702, GETDATE(), GETDATE(), 1), 
('jperez',     'jperez@correo.com',      0xB43E962A39DF6E94425FC4908B628C09639E2C9BBCBD04A10BBE9A970B3324BA, 0x6A6293A0601E0D0123341D2BFC04B365, GETDATE(), GETDATE(), 1), 
('rgonzalez',  'rgonzalez@correo.com',   0x159DE70EAAD9C60D3AE9B2B64A0A8303967EEAD896E4741D80DEC6C7242AD80C, 0xB74ABCBA7ADC81E51A2EF02D3AC1AFB2, GETDATE(), GETDATE(), 1);

INSERT INTO UsuarioRol (IdUsuario, IdRol, FechaAsignacion, Activo) VALUES
(1, 1, GETDATE(), 1), 
(2, 2, GETDATE(), 1), 
(3, 4, GETDATE(), 1), 
(4, 3, GETDATE(), 1); 

INSERT INTO Perfil (IdUsuario, Nombres, Apellidos, FechaNacimiento, Sexo, Telefono, Direccion, FotoPerfil, ContactoEmergenciaNombre, ContactoEmergenciaTelefono) VALUES
(1, 'Admin',   'Sistema',   '1990-01-01', 'N/A',       '00000000', 'N/A',                    NULL, NULL,              NULL),
(2, 'Maria',   'Gonzalez',  '1968-04-12', 'Femenino',  '88881111', 'Managua, Nicaragua',      NULL, 'Rosa Gonzalez',   '88884444'),
(3, 'Juan',    'Perez',     '1980-09-23', 'Masculino', '88882222', 'Hospital Bautista, Managua', NULL, NULL,           NULL),
(4, 'Rosa',    'Gonzalez',  '1995-02-15', 'Femenino',  '88884444', 'Managua, Nicaragua',      NULL, NULL,              NULL);

INSERT INTO Paciente (IdUsuario, TipoPaciente, PorcentajeDiscapacidad, NecesidadesEspeciales, FechaIngreso, EstadoPaciente) VALUES
(2, 'Fisica', 0, NULL, GETDATE(), 'Activo'); 

INSERT INTO ProfesionalSalud (IdUsuario, Especialidad, NumeroLicencia, CentroSalud, TelefonoProfesional, Activo) VALUES
(3, 'Medicina Interna', 'LIC-1234', 'Hospital Bautista', '88887777', 1); 

INSERT INTO Familiares (IdUsuario, Parentesco) VALUES
(4, 'Hija'); 

INSERT INTO AsistentePaciente (IdPaciente, IdFamiliar, TipoRelacion, PuedeVerCitas, PuedeVerMedicamentos, PuedeRecibirAlertas, PuedeGestionarRecordatorios, FechaAsignacion, Activo) VALUES
(1, 1, 'Cuidador principal', 1, 1, 1, 1, GETDATE(), 1);

INSERT INTO Enfermedades (NombreEnfermedad, Descripcion, EsCronica, Activa) VALUES
('Hipertension arterial', 'Presion arterial elevada de forma sostenida', 1, 1), 
('Diabetes tipo 2',       'Nivel elevado de glucosa en sangre', 1, 1);         

INSERT INTO PacienteEnfermedad (IdPaciente, IdEnfermedad, FechaDiagnostico, Observaciones, Activa) VALUES
(1, 1, '2026-08-01', 'Diagnostico inicial en consulta de rutina', 1); 
GO

INSERT INTO Medicamentos (NombreMedicamento, PrincipioActivo, Presentacion, Concentracion, Descripcion, Activo) VALUES
('Losartan',   'Losartan potasico',     'Tableta', '50mg',  'Antihipertensivo', 1), 
('Metformina', 'Metformina clorhidrato','Tableta', '850mg', 'Antidiabetico oral', 1);

INSERT INTO Tratamiento (IdPaciente, IdProfesional, IdEnfermedad, NombreTratamiento, Indicaciones, FechaInicio, FechaFin, EstadoTratamiento, Observaciones) VALUES
(1, 1, 1, 'Control de hipertension arterial', 'Tomar en ayunas, control de presion semanal', '2026-08-23', '2026-12-23', 'Activo', 'Paciente inicia tratamiento tras diagnostico');

INSERT INTO TratamientoMedicamento (IdTratamiento, IdMedicamento, Dosis, Frecuencia, ViaAdministracion, Horarios, Instrucciones) VALUES
(1, 1, '50mg', 'Cada 24 horas', 'Oral', '08:00', 'Tomar con abundante agua, en ayunas');

INSERT INTO Citas (IdPaciente, FechaHoraInicio, FechaHoraFin, TipoCita, Motivo, Modalidad, Lugar, EstadoCita, Observaciones, FechaCreacion) VALUES
(1, '2026-09-01T09:00:00', '2026-09-01T09:30:00', 'Control', 'Revision inicial de presion arterial', 'Presencial', 'Hospital Bautista', 'Programada', 'Primera cita de seguimiento', GETDATE());

INSERT INTO AsignarCitas (IdCita, IdProfesional, FechaAsignacion, EsPrincipal, EstadoAsignacion) VALUES
(1, 1, GETDATE(), 1, 'Confirmada'); -- IdAsignarCita = 1

INSERT INTO Recordatorios (IdPaciente, IdTratamientoMedicamento, IdCita, Titulo, Descripcion, TipoRecordatorio, FechaHoraProgramada, Repetir, FrecuenciaRepeticion, EstadoRecordatorio, FechaCompletado, Activo) VALUES
(1, 1, NULL, 'Tomar Losartan', 'Recordatorio diario para tomar el medicamento', 'Medicamento', '2026-08-24T08:00:00', 1, 'Diario', 'Pendiente', NULL, 1);

INSERT INTO TipoIndicadorSalud (NombreIndicador, UnidadMedida, Descripcion, Activo) VALUES
('Presion arterial sistolica', 'mmHg',  'Medicion de presion arterial sistolica', 1),
('Glucosa',                    'mg/dL', 'Nivel de glucosa en sangre', 1);            

INSERT INTO IndicadorSalud (IdPaciente, IdTipoIndicador, Valor, ValorSecundario, FechaHoraMedicion, IdUsuarioRegistro, Observaciones, Fuente) VALUES
(1, 1, 148.0, 95.0, '2026-08-24T08:15:00', 2, 'Medicion tomada despues de la primera dosis', 'Manual');

INSERT INTO Alertas (IdPaciente, IdIndicadorSalud, IdRecordatorio, Titulo, Mensaje, TipoAlerta, NivelPrioridad, FechaGeneracion, FechaLectura, Atendida, FechaAtencion) VALUES
(1, 1, NULL, 'Presion arterial elevada', 'El valor registrado (148/95 mmHg) supera el rango normal', 'IndicadorSalud', 'Alta', '2026-08-24T08:16:00', NULL, 0, NULL);

INSERT INTO HistorialPaciente (IdPaciente, IdUsuario, TipoEvento, FechaEvento, Titulo, Descripcion) VALUES
(1, 1, 'Tratamiento', '2026-08-23T00:00:00', 'Inicio de tratamiento', 'Se diagnostico hipertension arterial y se inicio tratamiento con Losartan');
GO

PRINT 'seed completo de NexoVida insertado correctamente';