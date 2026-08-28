using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IPacienteRepository : IRepository<Paciente>
{
    Task<List<Paciente>> ObterTodosPacientes();
    Task<List<Paciente>> ObterPacientesPorNome(string nome);
    Task<Paciente> ObterPorCPF(string cpf);
    Task<Paciente> ObterPorEmail(string email);
    Task<Paciente> ObterPorTelefone(string telefone);
}
