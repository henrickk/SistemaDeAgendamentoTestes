using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Repository;

public class ProfissionalRepository : Repository<Profissional>, IProfissionalRepository
{
    private readonly MeuDbContext _context;
    public ProfissionalRepository(MeuDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Profissional> ObterPorId(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Profissional> ObterPorId(Guid id) =>
        await _context.Set<Profissional>().FindAsync(id);

    public async Task<List<Profissional>> ObterTodos() =>
        await _context.Set<Profissional>().ToListAsync();

    public async Task<Profissional> ObterPorCRO(string cro) =>
        await _context.Set<Profissional>().FirstOrDefaultAsync(p => p.CRO == cro);

    public async Task<Profissional> ObterPorNome(string nome) =>
        await _context.Set<Profissional>().FirstOrDefaultAsync(p => p.Nome == nome);


}