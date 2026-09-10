using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests.Auxiliar;

public static class ProfissionalFixture
{
    public static Profissional CriarProfissionalFake()
    {
        var profissional = new Profissional
        {
            Nome = "Henrick",
            CRO = "000000",
            CPF = "00000000000",
            HoraInicio = TimeOnly.FromDateTime(DateTime.Today.AddHours(8)), // 08:00h
            HoraFim = TimeOnly.FromDateTime(DateTime.Today.AddHours(17)), // 17:00h
            Contato = new Contato("teste@123.com", "11999999999"),
            Endereco = new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000")
        };
        // Se houver propriedade para status, atribua aqui, ex: profissional.Status = StatusPaciente.Ativo.ToString();
        // Se precisar de DataNascimento ou Observação, atribua também conforme propriedades existentes.
        return profissional;
    }
}