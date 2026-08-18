using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Moq;

namespace Agendamento.Application.Tests;
public class ConfirmarAgendamentoTests
{
    [Fact]
    public void ConfirmarAgendamento_DeveAlterarStatusParaConfirmado_QuandoAgendamentoExistir()
    {
        // Arrange - Organizar
        var agendamentoId = Guid.NewGuid();
        var agendamento = new Agenda(
            Guid.NewGuid(),
            Guid.NewGuid(),
            StatusAgendamento.Agendado,
            DateTime.Now.AddHours(1),
            DateTime.Now.AddHours(2),
            "Consulta de rotina",
            null,
            null);

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        agendaRepositoryMock.Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        var notificadorMock = new Mock<INotificador>();
        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            new Mock<IPacienteRepository>().Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act - Agir
        agendaService.ConfirmarAgendamento(agendamentoId).Wait();

        // Assert - Afirmar
        Assert.Equal(StatusAgendamento.Confirmado, agendamento.StatusAgendamento);
    }
}