using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests.ServiceTests.Auxiliar;

public static class ProfissionalFixture
{
    public static Profissional CriarProfissionalFake(
        string nome = "Nome Padrão",
        string cro = "000000",
        string cpf = "00000000000")
    {
        return new Profissional(
            nome,
            cro,
            cpf,
            TimeOnly.FromDateTime(DateTime.Today.AddHours(8)), // 08:00h
            TimeOnly.FromDateTime(DateTime.Today.AddHours(17)), // 17:00h
            contato: new Contato("teste@123.com", "11999999999"),
            endereco: new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000")
        );
    }
}