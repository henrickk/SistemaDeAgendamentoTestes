using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Repository;

public class ProfissionalRepository : Repository<Profissional>, IProfissionalRepository
{
    private readonly MeuDbContext _dbContext;
    public ProfissionalRepository(MeuDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<Profissional> ObterPorId(Guid id)
    {
        return await _dbContext.Profissionais.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Profissional>> ObterTodos()
    {
        return await _dbContext.Profissionais.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .ToListAsync();
    }

    public async Task<Profissional> ObterPorCRO(string cro)
    {
        return await _dbContext.Profissionais.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CRO == cro);
    }

    public async Task<List<Profissional>> ObterPorNome(string nome)
    {
        return await _dbContext.Profissionais.AsNoTracking()
            .Where(p => p.Nome == nome)
            .ToListAsync();
    }
}