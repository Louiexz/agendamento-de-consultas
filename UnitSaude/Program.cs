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
// Load environment variables from .env file
Env.Load();
var builder = WebApplication.CreateBuilder(args);

// L� vari�veis de ambiente para sobrescrever configura��es sens�veis
var config = builder.Configuration;

void SetIfExists(string key, string envVar)
{
    var value = Environment.GetEnvironmentVariable(envVar);
    if (!string.IsNullOrEmpty(value))
        config[key] = value;
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

builder.Services.AddScoped<AdminInterface, AdminService>();
builder.Services.AddScoped<AnexoInterface, AnexoService>();
builder.Services.AddScoped<ConsultaInterface, ConsultaService>();
builder.Services.AddScoped<PacienteInterface, PacienteService>();
builder.Services.AddScoped<ProfessorInterface, ProfessorService>();
builder.Services.AddScoped<ProntuarioInterface, ProntuarioService>();
builder.Services.AddScoped<UsuarioInterface, UsuarioService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableDateOnlyConverter());
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
