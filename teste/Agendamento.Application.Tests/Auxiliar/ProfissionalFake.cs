using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests.Services.Auxiliar;

// 🟢 Adicione a definição da classe como public e static
public static class ProfissionalFixture
{
    // 🟢 Mude de private para public e adicione static
    public static Profissional CriarProfissionalFake(StatusPaciente status = StatusPaciente.Ativo)
    {
        return new Profissional(
            "Henrick",
            DateOnly.FromDateTime(DateTime.Today), // Adicionado para corresponder ao parâmetro 'DateOnly'
            "000000",
            "00000000000",
            status.ToString(), // Adicionado para corresponder ao parâmetro 'string'
            new Contato("teste@123.com", "11999999999"),
            TimeOnly.FromDateTime(DateTime.Today.AddHours(8)), // 08:00 AM
            TimeOnly.FromDateTime(DateTime.Today.AddHours(17)), // 05:00 PM
            "Observação", // Adicionado para corresponder ao parâmetro 'string'
            new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000")
        );
    }
}