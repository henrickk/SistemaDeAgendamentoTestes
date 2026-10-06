using Agendamento.Application.Interfaces;
using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Notificacoes;
using Agendamento.Infrastructure.Context;
using Agendamento.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.API.Configurations;

public static class DependencyInectionConfig
{
    public static IServiceCollection AddDependencyInectionConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada.");
        }

        services.AddDbContext<MeuDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAgendaService, AgendaService>();
        services.AddScoped<IAgendaRepository, AgendaRepository>();
        services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<INotificador, Notificador>();
        services.AddAutoMapper(typeof(AutomapperConfig).Assembly);

        return services;
    }
}
