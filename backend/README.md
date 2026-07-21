# NexoVida - Backend

Solución digital inteligente enfocada en el acompañamiento y seguimiento continuo de pacientes con enfermedades crónicas o discapacidad, permitiendo mantener el control del tratamiento y monitoreo de salud. La solución integrará recordatorios inteligentes para la toma puntual de medicamentos, seguimiento compartido con familiares o cuidadores mediante visualización de indicadores de salud y alertas preventivas que fortalezcan la adherencia al tratamiento, la continuidad del cuidado y la atención oportuna del paciente.

## Estructura del proyecto

```
NexoVida-Backend/
├── NexoVida.sln
├── compose.yaml         
├── Scripts/                    # Scripts .sql (creación de BD, datos, procedimientos)
├── WebApi/                     # Capa de presentación (API REST)
│   ├── Controllers/            # Controladores REST
│   ├── Dto/                    # Data Transfer Objects
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs              # Punto de entrada, configuración de JWT, CORS, Swagger
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── WebApi.csproj
├── WebApi.Models/              # Entidades / modelos de dominio
│   └── WebApi.Models.csproj
└── Services/
    ├── WebApi.Interface/        # Contratos (interfaces) de los servicios
    │   └── WebApi.Interface.csproj
    └── WebApi.Implementation/  # Implementación de la lógica de negocio
        └── WebApi.Implementation.csproj
```

### Relación entre capas

```
WebApi  --->  WebApi.Implementation  --->  WebApi.Interface  --->  WebApi.Models
   |
   +--------> WebApi.Interface
```

- **WebApi.Models**: entidades puras (POCOs), sin dependencias externas.
- **WebApi.Interface**: define los contratos de los servicios (`IUsuarioService`, `IRolService`, etc). Depende de `WebApi.Models`.
- **WebApi.Implementation**: implementa los servicios, contiene el acceso a datos (ADO.NET / `System.Data.SqlClient`). Depende de `WebApi.Interface`.
- **WebApi**: expone los Controllers, configura JWT, CORS y Swagger. Depende de `WebApi.Implementation` y `WebApi.Interface`.

## Paquetes NuGet utilizados

**WebApi**
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11)
- `Swashbuckle.AspNetCore` (6.9.0)
- `System.IdentityModel.Tokens.Jwt` (8.2.1)

**WebApi.Implementation**
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11)
- `Microsoft.Extensions.Configuration` (9.0.0)
- `System.Data.SqlClient` (4.9.0)
- `System.IdentityModel.Tokens.Jwt` (8.2.1)
