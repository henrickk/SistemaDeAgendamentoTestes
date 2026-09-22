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

    public async Task<IEnumerable<Paciente>> Buscar(Expression<Func<Paciente, bool>> predicate)
    {
        return await Task.FromResult(_dbContext.Pacientes.AsNoTracking().Where(predicate).ToList());
    }

    public async Task<Paciente> ObterPorId(Guid id)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Paciente>> ObterTodos()
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .ToListAsync();
    }

    public async Task<List<Paciente>> ObterPacientesPorNome(string nome)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .Where(p => p.Nome.Contains(nome))
            .ToListAsync();
    }

    public async Task<Paciente> ObterPorCPF(string cpf)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.CPF == cpf);
    }

    public async Task<Paciente> ObterPorEmail(string email)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Contato.Email == email);
    }

    public async Task<Paciente> ObterPorTelefone(string telefone)
    {
        return await _dbContext.Pacientes.AsNoTracking()
            .Include(p => p.Contato)
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Contato.NumeroCelular == telefone);
    }

    public async Task Adicionar(Paciente paciente)
    {
        await _dbContext.Pacientes.AddAsync(paciente);
    }

    public async Task Atualizar(Paciente paciente)
    {
        _dbContext.Pacientes.Update(paciente);
        await Task.CompletedTask;
    }

    public async Task Remover(Guid id)
    {
        var paciente = await ObterPorId(id);
        if (paciente != null)
        {
            _dbContext.Pacientes.Remove(paciente);
        }
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
