using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Repository;

public class AgendaRepository : Repository<Agenda>, IAgendaRepository
{
    private readonly MeuDbContext _dbContext;
    public AgendaRepository(MeuDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<List<Agenda>> ObterTodos()
    {
        return await _dbContext.Agendas.AsNoTracking()
            .Include(a => a.Paciente)
                .ThenInclude(p => p.Contato)
            .Include(a => a.Profissional)
            .ToListAsync();
    }

    public async Task<Agenda> ObterPorId(Guid id)
    {
        return await _dbContext.Agendas.AsNoTracking()
            .Include(p => p.Paciente)
            .Include(p => p.Profissional)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExisteConflitoHorario(Guid profissionalId, DateTime dataInicio, DateTime dataFim)
    {
        // Usando o 'DbSet' herdado da base
        return await DbSet.AnyAsync(a => a.ProfissionalId == profissionalId
                                       && dataInicio < a.DataFim
                                       && dataFim > a.DataInicio);
    }

    public async Task<IEnumerable<Agenda>> ObterTodosComRelacionamentos()
    {
        return await DbSet.AsNoTracking()
            .Include(a => a.Paciente)
                .ThenInclude(p => p.Contato)
            .Include(a => a.Profissional)
            .ToListAsync();
    }

    public async Task<Agenda> ObterPorIdComRelacionamentos(Guid id)
    {
        return await DbSet.AsNoTracking()
            .Include(a => a.Paciente)
                .ThenInclude(p => p.Contato)
            .Include(a => a.Profissional)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
