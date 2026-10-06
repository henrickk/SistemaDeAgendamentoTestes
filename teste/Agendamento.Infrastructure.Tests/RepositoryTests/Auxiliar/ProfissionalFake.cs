using Agendamento.Domain.Models;

namespace Agendamento.Infrastructure.Tests.RepositoryTests.Auxiliar;

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
            new TimeOnly(8, 0),
            new TimeOnly(17, 0),
            new Contato("teste@123.com", "11999999999"),
            new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000"));
    }
}
