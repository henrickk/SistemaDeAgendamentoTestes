using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests.Services;
public class ConcluirAgendamentoTests
{
    [Fact]
    public async Task ConcluirAgendamento_DeveAlterarStatusParaConcluido_QuandoAgendamentoExistir()
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
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        agendaRepositoryMock
            .Setup(repo => repo.Atualizar(It.IsAny<Agenda>()))
            .Returns(Task.CompletedTask);

        agendaRepositoryMock
            .Setup(repo => repo.SaveChanges())
            .ReturnsAsync(0);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            new Mock<IPacienteRepository>().Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConcluirAgendamento(agendamentoId);

        // Assert
        Assert.Equal(
            StatusAgendamento.Concluido,
            agendamento.StatusAgendamento);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(agendamento),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Once);
    }

    [Fact]
    public async Task ConcluirAgendamento_DeveNotificar_QuandoAgendamentoNaoExistir()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync((Agenda?)null);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            new Mock<IPacienteRepository>().Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConcluirAgendamento(agendamentoId);

        // Assert
        notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem == "Agendamento não encontrado.")),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task ConcluirAgendamento_DeveNotificar_QuandoAgendamentoJaEstiverConcluido()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();

        var agendamento = new Agenda(
            Guid.NewGuid(),
            Guid.NewGuid(),
            StatusAgendamento.Concluido,
            DateTime.Now.AddHours(1),
            DateTime.Now.AddHours(2),
            "Consulta de rotina",
            null,
            null);

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            new Mock<IPacienteRepository>().Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConcluirAgendamento(agendamentoId);

        // Assert
        notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem == "O agendamento já está concluído.")),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Never);
    }
}
