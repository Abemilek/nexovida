using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using WebApi.Interface;
using WebApi.Implementation;
using WebApi.Middleware;
using WebApi.Filters;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.StartsWith("CHANGE_ME", StringComparison.Ordinal))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Jwt:Key no configurado. Define una clave aleatoria de al menos 32 caracteres (variable Jwt__Key o appsettings).");
    }
    builder.Logging.AddFilter("Startup", LogLevel.Debug);
}

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10,
            }));

    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "NexoVida";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "NexoVida";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new 
                    { 
                        message = "El token de acceso ha expirado. Usa /api/auth/refresh para obtener uno nuevo.", 
                        tokenExpired = true 
                    });
                    return context.Response.WriteAsync(result);
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { message = "Se requiere autenticacion para acceder a este recurso." });
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { message = "No tiene permisos suficientes para realizar esta accion." });
                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddControllers(options => options.Filters.Add<ModelValidationFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NexoVida", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
// builder.Services.AddScoped<ITratamientoService, TratamientoService>();
// builder.Services.AddScoped<ITratamientoMedicamentoService, TratamientoMedicamentoService>();
// builder.Services.AddScoped<ICitaService, CitaService>();
// builder.Services.AddScoped<IAsignarCitaService, AsignarCitaService>();
// builder.Services.AddScoped<IRecordatorioService, RecordatorioService>();
// builder.Services.AddScoped<ITipoIndicadorSaludService, TipoIndicadorSaludService>();
// builder.Services.AddScoped<IIndicadorSaludService, IndicadorSaludService>();
// builder.Services.AddScoped<IAlertaService, AlertaService>();
// builder.Services.AddScoped<IHistorialPacienteService, HistorialPacienteService>();

builder.Services.AddScoped<IMetricaService, MetricaService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRolService, UsuarioRolService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IProfesionalSaludService, ProfesionalSaludService>();
builder.Services.AddScoped<IFamiliarService, FamiliarService>();
builder.Services.AddScoped<IAsistentePacienteService, AsistentePacienteService>();
// builder.Services.AddScoped<IEnfermedadService, EnfermedadService>();
// builder.Services.AddScoped<IPacienteEnfermedadService, PacienteEnfermedadService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ITotpService, TotpService>();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = false;
});

var app = builder.Build();

app.UseGlobalExceptionHandling();
app.UseSecurityHeaders();
app.UseRequestSanitization();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }))
   .AllowAnonymous();

app.Run();