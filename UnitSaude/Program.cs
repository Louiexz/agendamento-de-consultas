using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using System.Text;
using UnitSaude.Data;
using UnitSaude.Interfaces;
using UnitSaude.Services;
using UnitSaude.Utils;
using Hangfire;
using Hangfire.PostgreSql;

// Load environment variables from .env file
Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(); // sobrescreve o que veio antes
// L� vari�veis de ambiente para sobrescrever configura��es sens�veis
var config = builder.Configuration;

void SetIfExists(string key, string envVar)
{
    var value = Environment.GetEnvironmentVariable(envVar);
    if (!string.IsNullOrEmpty(value)) {
        config[key] = value;
    }
}

// JWT
SetIfExists("Jwt:Key", "JWT_KEY");
SetIfExists("Jwt:Issuer", "JWT_ISSUER");
SetIfExists("Jwt:Audience", "JWT_AUDIENCE");

// Conex�o
SetIfExists("ConnectionStrings:DefaultConnection", "CONNECTION_STRING");

// Email
SetIfExists("EmailSettings:Servidor", "EMAIL_SERVIDOR");
SetIfExists("EmailSettings:Porta", "EMAIL_PORTA");
SetIfExists("EmailSettings:Usuario", "EMAIL_USUARIO");
SetIfExists("EmailSettings:Senha", "EMAIL_SENHA");
SetIfExists("EmailSettings:UsarSSL", "EMAIL_SSL");
SetIfExists("EmailSettings:UrlRedefinicaoSenha", "URL_REDEFINICAO");

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClinicaDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var hangfireConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? connectionString;

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(hangfireConnectionString, new PostgreSqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(15), // Intervalo de verificação de jobs
        InvisibilityTimeout = TimeSpan.FromHours(3), // Tempo máximo que um job pode ficar invisível
        DistributedLockTimeout = TimeSpan.FromMinutes(5), // Tempo máximo para locks distribuídos
        PrepareSchemaIfNecessary = true, // Cria automaticamente as tabelas necessárias
        EnableTransactionScopeEnlistment = true
    }));

// Adiciona o servidor de processamento de jobs
builder.Services.AddHangfireServer(options => {
    options.ServerName = "UnitSaude.Hangfire"; // Nome do servidor
    options.Queues = new[] { "default" }; // Filas a serem monitoradas
    options.WorkerCount = Environment.ProcessorCount * 5; // Número de workers
});

builder.Services.AddScoped<AdminInterface, AdminService>();
builder.Services.AddScoped<AnexoInterface, AnexoService>();
builder.Services.AddScoped<ConsultaInterface, ConsultaService>();
builder.Services.AddScoped<PacienteInterface, PacienteService>();
builder.Services.AddScoped<ProfessorInterface, ProfessorService>();
builder.Services.AddScoped<ProntuarioInterface, ProntuarioService>();
builder.Services.AddScoped<UsuarioInterface, UsuarioService>();
builder.Services.AddScoped<DisponibilidadeInterface, DisponibilidadeService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UnitSaude - API", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Digite o token JWT no formato: Bearer {seu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securitySchema,
            new string[] {}
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");

var key = jwtSettings["Key"]
    ?? Environment.GetEnvironmentVariable("JWT_KEY");

if (string.IsNullOrWhiteSpace(key))
{
    throw new Exception("A chave JWT não foi encontrada. Configure 'Jwt:Key' no appsettings.json ou defina a variável de ambiente JWT_KEY.");
}

var keyBytes = Encoding.UTF8.GetBytes(key);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"]
            ?? Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = jwtSettings["Audience"]
            ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
        policy.RequireRole("Administrador"));

    options.AddPolicy("Professor", policy =>
        policy.RequireRole("Professor"));

    options.AddPolicy("Paciente", policy =>
        policy.RequireRole("Paciente"));
});

var allowedOrigins = new[] {
    "https://agendamento-de-consultas-psi.vercel.app",
    "http://localhost:5173"
};
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendOrigin",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    );
});


var app = builder.Build();

ServiceActivator.Configure(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Dashboard do Hangfire apenas em desenvolvimento
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        DashboardTitle = "UnitSaude - Hangfire Dashboard",
        Authorization = new[] { new HangfireAuthorizationFilter() },
        StatsPollingInterval = 5000 // Atualiza a cada 5 segundos
    });
}


app.UseHttpsRedirection();

app.UseCors("AllowFrontendOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
