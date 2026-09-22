using Agendamento.API.Configurations;
using Agendamento.Application.Interfaces;
using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Notificacoes;
using Agendamento.Infrastructure.Context; // Link com o seu projeto de infraestrutura
using Agendamento.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CONFIGURAÇÃO DO ENTITY FRAMEWORK (Adicionado aqui)
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IAgendaService, AgendaService>();
builder.Services.AddScoped<IAgendaRepository, AgendaRepository>();     // ADICIONAR
builder.Services.AddScoped<IProfissionalRepository, ProfissionalRepository>(); // ADICIONAR
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<INotificador, Notificador>();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(AutomapperConfig).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
