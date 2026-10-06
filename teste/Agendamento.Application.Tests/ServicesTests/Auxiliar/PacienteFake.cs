using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests.ServiceTests.Auxiliar;

public static class PacienteFixture
{
    public static Paciente CriarPacienteFake(
        StatusPaciente status = StatusPaciente.Ativo, 
        string nome = "Nome Padrão", 
        string cpf = "00000000000")
    {
        return new Paciente(
            nome, // usa o parâmetro
            new DateOnly(2000, 1, 1),
            cpf,  // usa o parâmetro
            "0000000",
            StatusGenero.Masculino,
            StatusEstadoCivil.Solteiro,
            status,
            endereco: new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000"),
            contato: new Contato("teste@123.com", "11999999999")
        );
    }
}
