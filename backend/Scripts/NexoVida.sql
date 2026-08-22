USE master;
GO

CREATE DATABASE NexoVida;
GO

USE NexoVida;
GO

CREATE TABLE Roles(
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    NombreRol NVARCHAR(50),
    Descripcion NVARCHAR(250),
    Activo BIT
);

CREATE TABLE Usuario(
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(100),
    Correo NVARCHAR(200),
    Contrasena VARBINARY(512),
    Salt VARBINARY(32),
    FechaRegistro DATETIME,
    UltimoAcceso DATETIME,
    Activo BIT
);

CREATE TABLE UsuarioRol(
    IdUsuarioRol INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    IdRol INT,
    FechaAsignacion DATETIME,
    Activo BIT,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);

CREATE TABLE Perfil(
    IdPerfil INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Nombres NVARCHAR(100),
    Apellidos NVARCHAR(100),
    FechaNacimiento DATE,
    Sexo NVARCHAR(20),
    Telefono NVARCHAR(30),
    Direccion NVARCHAR(300),
    FotoPerfil NVARCHAR(500),
    ContactoEmergenciaNombre NVARCHAR(200),
    ContactoEmergenciaTelefono NVARCHAR(30),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE Paciente(
    IdPaciente INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    TipoPaciente NVARCHAR(50),
    PorcentajeDiscapacidad DECIMAL(5,2),
    NecesidadesEspeciales NVARCHAR(500),
    FechaIngreso DATETIME,
    EstadoPaciente NVARCHAR(30),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE ProfesionalSalud(
    IdProfesional INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Especialidad NVARCHAR(150),
    NumeroLicencia NVARCHAR(100),
    CentroSalud NVARCHAR(200),
    TelefonoProfesional NVARCHAR(30),
    Activo BIT,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE Familiares(
    IdFamiliar INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Parentesco NVARCHAR(100),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE AsistentePaciente(
    IdAsistentePaciente INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdFamiliar INT,
    TipoRelacion NVARCHAR(100),
    PuedeVerCitas BIT,
    PuedeVerMedicamentos BIT,
    PuedeRecibirAlertas BIT,
    PuedeGestionarRecordatorios BIT,
    FechaAsignacion DATETIME,
    Activo BIT,
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdFamiliar) REFERENCES Familiares(IdFamiliar)
);

CREATE TABLE Enfermedades(
    IdEnfermedad INT PRIMARY KEY IDENTITY(1,1),
    NombreEnfermedad NVARCHAR(200),
    Descripcion NVARCHAR(500),
    EsCronica BIT,
    Activa BIT
);

CREATE TABLE PacienteEnfermedad(
    IdPacienteEnfermedad INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdEnfermedad INT,
    FechaDiagnostico DATE,
    Observaciones NVARCHAR(500),
    Activa BIT,
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdEnfermedad) REFERENCES Enfermedades(IdEnfermedad)
);

CREATE TABLE Medicamentos(
    IdMedicamento INT PRIMARY KEY IDENTITY(1,1),
    NombreMedicamento NVARCHAR(200),
    PrincipioActivo NVARCHAR(200),
    Presentacion NVARCHAR(100),
    Concentracion NVARCHAR(100),
    Descripcion NVARCHAR(500),
    Activo BIT
);

CREATE TABLE Tratamiento(
    IdTratamiento INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdProfesional INT,
    IdEnfermedad INT,
    NombreTratamiento NVARCHAR(200),
    Indicaciones NVARCHAR(1000),
    FechaInicio DATE,
    FechaFin DATE,
    EstadoTratamiento NVARCHAR(30),
    Observaciones NVARCHAR(1000),
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdProfesional) REFERENCES ProfesionalSalud(IdProfesional),
    FOREIGN KEY (IdEnfermedad) REFERENCES Enfermedades(IdEnfermedad)
);

CREATE TABLE TratamientoMedicamento(
    IdTratamientoMedicamento INT PRIMARY KEY IDENTITY(1,1),
    IdTratamiento INT,
    IdMedicamento INT,
    Dosis NVARCHAR(100),
    Frecuencia NVARCHAR(100),
    ViaAdministracion NVARCHAR(100),
    Horarios NVARCHAR(300),
    Instrucciones NVARCHAR(500),
    FOREIGN KEY (IdTratamiento) REFERENCES Tratamiento(IdTratamiento),
    FOREIGN KEY (IdMedicamento) REFERENCES Medicamentos(IdMedicamento)
);

CREATE TABLE Citas(
    IdCita INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    FechaHoraInicio DATETIME,
    FechaHoraFin DATETIME,
    TipoCita NVARCHAR(100),
    Motivo NVARCHAR(500),
    Modalidad NVARCHAR(50),
    Lugar NVARCHAR(300),
    EstadoCita NVARCHAR(30),
    Observaciones NVARCHAR(1000),
    FechaCreacion DATETIME,
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente)
);

CREATE TABLE AsignarCitas(
    IdAsignarCita INT PRIMARY KEY IDENTITY(1,1),
    IdCita INT,
    IdProfesional INT,
    FechaAsignacion DATETIME,
    EsPrincipal BIT,
    EstadoAsignacion NVARCHAR(30),
    FOREIGN KEY (IdCita) REFERENCES Citas(IdCita),
    FOREIGN KEY (IdProfesional) REFERENCES ProfesionalSalud(IdProfesional)
);

CREATE TABLE Recordatorios(
    IdRecordatorio INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdTratamientoMedicamento INT,
    IdCita INT,
    Titulo NVARCHAR(200),
    Descripcion NVARCHAR(500),
    TipoRecordatorio NVARCHAR(50),
    FechaHoraProgramada DATETIME,
    Repetir BIT,
    FrecuenciaRepeticion NVARCHAR(100),
    EstadoRecordatorio NVARCHAR(30),
    FechaCompletado DATETIME,
    Activo BIT,
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdTratamientoMedicamento) REFERENCES TratamientoMedicamento(IdTratamientoMedicamento),
    FOREIGN KEY (IdCita) REFERENCES Citas(IdCita)
);

CREATE TABLE TipoIndicadorSalud(
    IdTipoIndicador INT PRIMARY KEY IDENTITY(1,1),
    NombreIndicador NVARCHAR(100),
    UnidadMedida NVARCHAR(50),
    Descripcion NVARCHAR(300),
    Activo BIT
);

CREATE TABLE IndicadorSalud(
    IdIndicadorSalud BIGINT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdTipoIndicador INT,
    Valor DECIMAL(12,4),
    ValorSecundario DECIMAL(12,4),
    FechaHoraMedicion DATETIME,
    IdUsuarioRegistro INT,
    Observaciones NVARCHAR(500),
    Fuente NVARCHAR(100),
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdTipoIndicador) REFERENCES TipoIndicadorSalud(IdTipoIndicador),
    FOREIGN KEY (IdUsuarioRegistro) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE Alertas(
    IdAlerta BIGINT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdIndicadorSalud BIGINT,
    IdRecordatorio INT,
    Titulo NVARCHAR(200),
    Mensaje NVARCHAR(1000),
    TipoAlerta NVARCHAR(50),
    NivelPrioridad NVARCHAR(30),
    FechaGeneracion DATETIME,
    FechaLectura DATETIME,
    Atendida BIT,
    FechaAtencion DATETIME,
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdIndicadorSalud) REFERENCES IndicadorSalud(IdIndicadorSalud),
    FOREIGN KEY (IdRecordatorio) REFERENCES Recordatorios(IdRecordatorio)
);

CREATE TABLE HistorialPaciente(
    IdHistorialPaciente BIGINT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT,
    IdUsuario INT,
    TipoEvento NVARCHAR(100),
    FechaEvento DATETIME,
    Titulo NVARCHAR(200),
    Descripcion NVARCHAR(2000),
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);
