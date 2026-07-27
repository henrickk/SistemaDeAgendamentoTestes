using Agendamento.Application.DTOs;
using Agendamento.Domain.Models;

namespace Agendamento.Application.Tests;

public class AgendaTests
{
    [Fact]
    public void Agendar_DeveCriarAgendamento_QuandoDadosForemValidos()
    {
        // Arrange
        var agenda = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        // Act
        var agendamento = new Agenda(
            agenda.PacienteId,
            agenda.ProfissionalId,
            StatusAgendamento.Agendado,
            agenda.DataInicio,
            agenda.DataFim,
            agenda.Observacao,
            null,
            null);

        // Assert
        Assert.Equal(agenda.PacienteId, agendamento.PacienteId);
    }
}