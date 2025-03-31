using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Interfaces;
using UnitSaude.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
