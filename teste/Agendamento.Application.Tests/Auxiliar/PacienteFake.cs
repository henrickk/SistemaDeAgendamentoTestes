using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests.Services.Auxiliar;

public static class PacienteFixture
{
    public static Paciente CriarPacienteFake(StatusPaciente status = StatusPaciente.Ativo)
    {
        return new Paciente(
            "Nome Padrão",
            new DateOnly(2000, 1, 1),
            "00000000000",
            "0000000",
            StatusGenero.Masculino,
            StatusEstadoCivil.Solteiro,
            status,
            endereco: new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000"),
            contato: new Contato("teste@123.com", "11999999999")
        );
    }
}