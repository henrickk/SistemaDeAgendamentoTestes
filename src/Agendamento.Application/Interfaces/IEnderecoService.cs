namespace Agendamento.Application.Interfaces;

public interface IEnderecoService : IDisposable
{
    Task AdicionarEndereco(NovoPacienteDto dto);

    Task AtualizarEndereco(AtualizarPacienteDto dto);

    Task RemoverEndereco(Guid id);
}
