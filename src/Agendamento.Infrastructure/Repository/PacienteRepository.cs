using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using System.Linq.Expressions;

namespace Agendamento.Infrastructure.Repository;
public class PacienteRepository : IRepository<Paciente>, IPacienteRepository
{
    public PacienteRepository(MeuDbContext context) : base(context) { }
    
    public async Task Adicionar(Paciente entity)
    {
        throw new NotImplementedException();
    }

    public Task Atualizar(Paciente entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Paciente>> Buscar(Expression<Func<Paciente, bool>> predicate)
    {
        return Task.FromResult<IEnumerable<Paciente>>(new List<Paciente>());
    }

    public Task<List<Paciente>> ObterPacientesPorNome(string nome)
    {
        throw new NotImplementedException();
    }

    public Task<Paciente> ObterPorCPF(string cpf)
    {
        throw new NotImplementedException();
    }

    public Task<Paciente> ObterPorEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Paciente> ObterPorId(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Paciente> ObterPorTelefone(string telefone)
    {
        throw new NotImplementedException();
    }

    public Task<List<Paciente>> ObterTodos()
    {
        throw new NotImplementedException();
    }

    public Task<List<Paciente>> ObterTodosPacientes()
    {
        throw new NotImplementedException();
    }

    public Task Remover(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges()
    {
        throw new NotImplementedException();
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
