using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Agendamento.Infrastructure.Repository;
public class PacienteRepository : IRepository<Paciente>, IPacienteRepository
{
    private readonly MeuDbContext _dbContext;
    public PacienteRepository(MeuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IEnumerable<Paciente>> Buscar(Expression<Func<Paciente, bool>> predicate)
    {
        return Task.FromResult<IEnumerable<Paciente>>(new List<Paciente>());
    }
    public async Task<Paciente> ObterPorId(Guid id)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<List<Paciente>> ObterTodos()
    {
        return await _dbContext.Pacientes.AsNoTracking().ToListAsync();
    }

    public async Task<List<Paciente>> ObterPacientesPorNome(string nome)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Where(p => p.Nome.Contains(nome))
            .ToListAsync();
    }

    public async Task<Paciente> ObterPorCPF(string cpf)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CPF == cpf);
    }

    public async Task<Paciente> ObterPorEmail(string email)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Contato.Email == email);
    }

    public async Task<Paciente> ObterPorTelefone(string telefone)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Contato.NumeroCelular== telefone);
    }

    public void Adicionar(Paciente paciente)
    {
        _dbContext.Pacientes.Add(paciente);
    }

    public void Atualizar(Paciente paciente)
    {
        _dbContext.Pacientes.Update(paciente);
    }

    public void Remover(Guid id)
    {
        //_dbContext.Pacientes.Remove(id); Erro chato
    }

    public async Task<int> SaveChanges()
    {
        return await _dbContext.SaveChangesAsync();
    }
    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
