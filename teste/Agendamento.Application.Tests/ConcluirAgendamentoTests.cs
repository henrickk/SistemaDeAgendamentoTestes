using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Moq;

namespace Agendamento.Application.Tests.Services;
public class ConcluirAgendamentoTests
{
    [Fact]
    public void ConcluirAgendamento_DeveAlterarStatusParaConcluido_QuandoAgendamentoExistir()
    {
        // Arrange
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
        // Act
        agendaService.ConcluirAgendamento(agendamentoId).Wait();

        // Assert
        Assert.Equal(StatusAgendamento.Concluido, agendamento.StatusAgendamento);
    }
}
