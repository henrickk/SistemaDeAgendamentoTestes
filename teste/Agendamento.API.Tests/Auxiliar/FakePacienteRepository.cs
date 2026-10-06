using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using System.Linq.Expressions;

namespace Agendamento.API.Tests.Auxiliar;

internal sealed class FakePacienteRepository : IPacienteRepository
{
    public List<Paciente> Pacientes { get; } = [];
    public int SaveChangesCount { get; private set; }

    public Task<Paciente> ObterPorId(Guid id) => Task.FromResult(Pacientes.FirstOrDefault(p => p.Id == id)!);
    public Task<List<Paciente>> ObterTodos() => Task.FromResult(Pacientes);
    public Task<IEnumerable<Paciente>> Buscar(Expression<Func<Paciente, bool>> predicate) =>
        Task.FromResult<IEnumerable<Paciente>>(Pacientes.AsQueryable().Where(predicate));
    public Task Adicionar(Paciente entity) { Pacientes.Add(entity); return Task.CompletedTask; }
    public Task Atualizar(Paciente entity) => Task.CompletedTask;
    public Task Remover(Guid id) { Pacientes.RemoveAll(p => p.Id == id); return Task.CompletedTask; }
    public Task<int> SaveChanges() { SaveChangesCount++; return Task.FromResult(1); }
    public Task<List<Paciente>> ObterPacientesPorNome(string nome) =>
        Task.FromResult(Pacientes.Where(p => p.Nome.Contains(nome)).ToList());
    public Task<Paciente> ObterPorCPF(string cpf) => Task.FromResult(Pacientes.FirstOrDefault(p => p.CPF == cpf)!);
    public Task<Paciente> ObterPorEmail(string email) => Task.FromResult(Pacientes.FirstOrDefault(p => p.Contato.Email == email)!);
    public Task<Paciente> ObterPorTelefone(string telefone) =>
        Task.FromResult(Pacientes.FirstOrDefault(p => p.Contato.NumeroCelular == telefone)!);
    public void Dispose() { }
}
