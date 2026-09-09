using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Repository;
public class AgendaRepository : Repository<Agenda>, IAgendaRepository
{
    private readonly DbContext _context;

    public AgendaRepository(MeuDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task Adicionar(Agenda agenda)
    {
        await _context.Set<Agenda>().AddAsync(agenda);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> VerificarConflitoProfissional(Guid profissionalId, DateTime inicio, DateTime fim)
    {
        return await _context.Set<Agenda>()
            .AnyAsync(a => a.ProfissionalId == profissionalId
                           && inicio < a.DataFim
                           && fim > a.DataInicio);
    }

    public async Task<bool> ExisteConflitoHorario(Guid profissionalId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Set<Agenda>()
            .AnyAsync(a => a.ProfissionalId == profissionalId
                           && dataInicio < a.DataFim
                           && dataFim > a.DataInicio);
    }

    public async Task<List<Agenda>> ObterAgendamentosPorProfissional(Guid profissionalId)
    {
        return await _context.Set<Agenda>()
            .Where(a => a.ProfissionalId == profissionalId)
            .ToListAsync();
    }
}
