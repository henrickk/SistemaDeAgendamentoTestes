using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests.Services.Auxiliar;

public static class ProfissionalFixture
{
    public static Profissional CriarProfissionalFake(StatusPaciente status = StatusPaciente.Ativo)
    {
        return new Profissional(
            "Henrick",
            DateOnly.FromDateTime(DateTime.Today),
            "000000",
            "00000000000",
            status.ToString(),
            new Contato("teste@123.com", "11999999999"),
            TimeOnly.FromDateTime(DateTime.Today.AddHours(8)), // 08:00h
            TimeOnly.FromDateTime(DateTime.Today.AddHours(17)), // 17:00h
            "Observação",
            new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000")
        );
    }
}