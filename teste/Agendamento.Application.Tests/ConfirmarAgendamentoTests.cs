using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class ConfirmarAgendamentoTests
{
    [Fact]
    public async Task ConfirmarAgendamento_DeveAlterarStatusParaConfirmado_QuandoAgendamentoEstiverAgendado()
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

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            new Mock<IPacienteRepository>().Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConfirmarAgendamento(agendamentoId);

        // Assert
        Assert.Equal(
            StatusAgendamento.Confirmado,
            agendamento.StatusAgendamento);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(agendamento),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmarAgendamento_DeveNotificar_QuandoAgendamentoNaoExistir()
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
        await agendaService.ConfirmarAgendamento(agendamentoId);

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
    public async Task ConfirmarAgendamento_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();

        var agendamento = new Agenda(
            pacienteId,
            Guid.NewGuid(),
            StatusAgendamento.Concluido,
            DateTime.Now.AddHours(1),
            DateTime.Now.AddHours(2),
            "Consulta de rotina",
            null,
            null);

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        var pacienteRepositoryMock = new Mock<IPacienteRepository>();
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        pacienteRepositoryMock
            .Setup(repo => repo.ObterPorId(pacienteId))
            .ReturnsAsync((Paciente?)null);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            pacienteRepositoryMock.Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConfirmarAgendamento(agendamentoId);

        // Assert
        notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem == "Paciente não encontrado.")),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmarAgendamento_DeveNotificar_QuandoAgendamentoJaEstiverConfirmado()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();

        var agendamento = new Agenda(
            pacienteId,
            Guid.NewGuid(),
            StatusAgendamento.Confirmado,
            DateTime.Now.AddHours(1),
            DateTime.Now.AddHours(2),
            "Consulta de rotina",
            null,
            null);

        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        var pacienteRepositoryMock = new Mock<IPacienteRepository>();
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        pacienteRepositoryMock
            .Setup(repo => repo.ObterPorId(pacienteId))
            .ReturnsAsync(paciente);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            pacienteRepositoryMock.Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConfirmarAgendamento(agendamentoId);

        // Assert
        notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem == "O agendamento já está confirmado.")),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmarAgendamento_DeveNotificar_QuandoPacienteEstiverBloqueado()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();

        var agendamento = new Agenda(
            pacienteId,
            Guid.NewGuid(),
            StatusAgendamento.Concluido,
            DateTime.Now.AddHours(1),
            DateTime.Now.AddHours(2),
            "Consulta de rotina",
            null,
            null);

        var pacienteBloqueado = PacienteFixture.CriarPacienteFake(
            StatusPaciente.Bloqueado);

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        var pacienteRepositoryMock = new Mock<IPacienteRepository>();
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        pacienteRepositoryMock
            .Setup(repo => repo.ObterPorId(pacienteId))
            .ReturnsAsync(pacienteBloqueado);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            pacienteRepositoryMock.Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConfirmarAgendamento(agendamentoId);

        // Assert
        notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem ==
                "Paciente bloqueado. Não é possível confirmar agendamento.")),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmarAgendamento_DeveAlterarStatusParaConfirmado_QuandoPacienteEstiverValido()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();

        var agendamento = new Agenda(
            pacienteId,
            Guid.NewGuid(),
            StatusAgendamento.Concluido,
            DateTime.Now.AddHours(1),
            DateTime.Now.AddHours(2),
            "Consulta de rotina",
            null,
            null);

        var paciente = PacienteFixture.CriarPacienteFake(
            StatusPaciente.Ativo);

        var agendaRepositoryMock = new Mock<IAgendaRepository>();
        var pacienteRepositoryMock = new Mock<IPacienteRepository>();
        var notificadorMock = new Mock<INotificador>();

        agendaRepositoryMock
            .Setup(repo => repo.ObterPorId(agendamentoId))
            .ReturnsAsync(agendamento);

        pacienteRepositoryMock
            .Setup(repo => repo.ObterPorId(pacienteId))
            .ReturnsAsync(paciente);

        agendaRepositoryMock
            .Setup(repo => repo.Atualizar(It.IsAny<Agenda>()))
            .Returns(Task.CompletedTask);

        agendaRepositoryMock
            .Setup(repo => repo.SaveChanges())
            .ReturnsAsync(0);

        var agendaService = new AgendaService(
            notificadorMock.Object,
            agendaRepositoryMock.Object,
            pacienteRepositoryMock.Object,
            new Mock<IProfissionalRepository>().Object
        );

        // Act
        await agendaService.ConfirmarAgendamento(agendamentoId);

        // Assert
        Assert.Equal(
            StatusAgendamento.Confirmado,
            agendamento.StatusAgendamento);

        agendaRepositoryMock.Verify(
            repo => repo.Atualizar(agendamento),
            Times.Once);

        agendaRepositoryMock.Verify(
            repo => repo.SaveChanges(),
            Times.Once);
    }
}