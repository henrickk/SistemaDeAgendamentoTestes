using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;

public class AtualizarAgendamentoTests
{
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Mock<INotificador> _notificadorMock;
    private readonly AgendaService _agendaService;

    public AtualizarAgendamentoTests()
    {
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificadorMock = new Mock<INotificador>();

        _agendaService = new AgendaService(
            _notificadorMock.Object,
            _agendaRepositoryMock.Object,
            _pacienteRepositoryMock.Object,
            _profissionalRepositoryMock.Object
        );
    }

    [Fact]
    public async Task AtualizarAgendamento_DeveAtualizar_QuandoDadosForemValidos()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();

        var agendamentoExistente = new Agenda(
            agendaId,
            profissionalId,
            StatusAgendamento.Agendado,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Observação antiga",
            new Paciente(),
            new Profissional()
        );

        var dto = new AtualizarAgendamentoDto
        {
            ProfissionalId = profissionalId,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(1).AddHours(1),
            Observacao = "Nova observação",
            StatusAgendamento = StatusAgendamento.Confirmado
        };

        _agendaRepositoryMock
            .Setup(r => r.ObterPorId(agendaId))
            .ReturnsAsync(agendamentoExistente);

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissionalId))
            .ReturnsAsync(new Profissional());

        _agendaRepositoryMock
            .Setup(r => r.ExisteConflitoHorario(
                profissionalId,
                dto.DataInicio,
                dto.DataFim))
            .ReturnsAsync(false);

        _agendaRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Agenda>()))
            .Returns(Task.CompletedTask);

        _agendaRepositoryMock
            .Setup(r => r.SaveChanges())
            .ReturnsAsync(0);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        Assert.Equal(dto.DataInicio, agendamentoExistente.DataInicio);
        Assert.Equal(dto.DataFim, agendamentoExistente.DataFim);
        Assert.Equal(dto.Observacao, agendamentoExistente.Observacao);
        Assert.Equal(dto.ProfissionalId, agendamentoExistente.ProfissionalId);
        Assert.Equal(dto.StatusAgendamento, agendamentoExistente.StatusAgendamento);

        _agendaRepositoryMock.Verify(
            r => r.Atualizar(agendamentoExistente),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);

        _notificadorMock.Verify(
            n => n.Handle(It.IsAny<Notificacao>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAgendamento_DeveNotificar_QuandoAgendamentoNaoExistir()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var dto = new AtualizarAgendamentoDto();

        _agendaRepositoryMock
            .Setup(r => r.ObterPorId(agendaId))
            .ReturnsAsync((Agenda?)null);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem == "Agendamento não encontrado.")),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAgendamento_DeveNotificar_QuandoProfissionalNaoExistir()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();

        var agendamentoExistente = new Agenda(
            agendaId,
            profissionalId,
            StatusAgendamento.Agendado,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Observação",
            new Paciente(),
            new Profissional()
        );

        var dto = new AtualizarAgendamentoDto
        {
            ProfissionalId = profissionalId,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(1).AddHours(1),
            Observacao = "Nova observação",
            StatusAgendamento = StatusAgendamento.Confirmado
        };

        _agendaRepositoryMock
            .Setup(r => r.ObterPorId(agendaId))
            .ReturnsAsync(agendamentoExistente);

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissionalId))
            .ReturnsAsync((Profissional?)null);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem == "Profissional não encontrado.")),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAgendamento_DeveNotificar_QuandoHouverConflitoDeHorario()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();

        var agendamentoExistente = new Agenda(
            agendaId,
            profissionalId,
            StatusAgendamento.Agendado,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Observação",
            new Paciente(),
            new Profissional()
        );

        var dto = new AtualizarAgendamentoDto
        {
            ProfissionalId = profissionalId,
            DataInicio = DateTime.Now.AddDays(2),
            DataFim = DateTime.Now.AddDays(2).AddHours(1),
            Observacao = "Nova observação",
            StatusAgendamento = StatusAgendamento.Confirmado
        };

        _agendaRepositoryMock
            .Setup(r => r.ObterPorId(agendaId))
            .ReturnsAsync(agendamentoExistente);

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissionalId))
            .ReturnsAsync(new Profissional());

        _agendaRepositoryMock
            .Setup(r => r.ExisteConflitoHorario(
                profissionalId,
                dto.DataInicio,
                dto.DataFim))
            .ReturnsAsync(true);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _notificadorMock.Verify(
            n => n.Handle(It.Is<Notificacao>(notificacao =>
                notificacao.Mensagem ==
                "O profissional já possui um agendamento neste horário.")),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.Atualizar(It.IsAny<Agenda>()),
            Times.Never);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAgendamento_DeveAtualizar_QuandoConflitoForDoProprioAgendamento()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();

        var dataInicio = DateTime.Now.AddDays(1);
        var dataFim = dataInicio.AddHours(1);

        var agendamentoExistente = new Agenda(
            agendaId,
            profissionalId,
            StatusAgendamento.Agendado,
            dataInicio,
            dataFim,
            "Observação antiga",
            new Paciente(),
            new Profissional()
        );

        var dto = new AtualizarAgendamentoDto
        {
            ProfissionalId = profissionalId,
            DataInicio = dataInicio,
            DataFim = dataFim,
            Observacao = "Nova observação",
            StatusAgendamento = StatusAgendamento.Confirmado
        };

        _agendaRepositoryMock
            .Setup(r => r.ObterPorId(agendaId))
            .ReturnsAsync(agendamentoExistente);

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissionalId))
            .ReturnsAsync(new Profissional());

        _agendaRepositoryMock
            .Setup(r => r.ExisteConflitoHorario(
                profissionalId,
                dataInicio,
                dataFim))
            .ReturnsAsync(true);

        _agendaRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Agenda>()))
            .Returns(Task.CompletedTask);

        _agendaRepositoryMock
            .Setup(r => r.SaveChanges())
            .ReturnsAsync(0);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        Assert.Equal(dto.DataInicio, agendamentoExistente.DataInicio);
        Assert.Equal(dto.DataFim, agendamentoExistente.DataFim);
        Assert.Equal(dto.Observacao, agendamentoExistente.Observacao);
        Assert.Equal(dto.ProfissionalId, agendamentoExistente.ProfissionalId);
        Assert.Equal(dto.StatusAgendamento, agendamentoExistente.StatusAgendamento);

        _agendaRepositoryMock.Verify(
            r => r.Atualizar(agendamentoExistente),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);

        _notificadorMock.Verify(
            n => n.Handle(It.IsAny<Notificacao>()),
            Times.Never);
    }
}