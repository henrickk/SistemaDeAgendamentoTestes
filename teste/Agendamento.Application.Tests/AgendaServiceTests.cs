using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
using Xunit;

namespace Agendamento.Application.Tests;

public class AgendaServiceTests
{
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Mock<INotificador> _notificadorMock;
    private readonly AgendaService _agendaService;

    public AgendaServiceTests()
    {
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificadorMock = new Mock<INotificador>();

        // Instancia a service injetando os mocks mockados
        _agendaService = new AgendaService(
            _notificadorMock.Object,
            _agendaRepositoryMock.Object,
            _pacienteRepositoryMock.Object,
            _profissionalRepositoryMock.Object
        );
    }

    [Fact(DisplayName = "Atualizar Agendamento com Sucesso")]
    [Trait("Categoria", "Agenda Service NDD")]
    public async Task Atualizar_AgendamentoValido_DeveExecutarComSucesso()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();

        var agendamentoExistente = new Agenda
            (
            agendaId, // Corrigido: passa o Id correto no construtor
            profissionalId,
            StatusAgendamento.Agendado,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Obs",
            new Paciente(), // Corrigido: não passar null para tipos não anuláveis
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

        _agendaRepositoryMock.Setup(r => r.ObterPorId(agendaId)).ReturnsAsync(agendamentoExistente);
        _profissionalRepositoryMock.Setup(r => r.ObterPorId(profissionalId)).ReturnsAsync(new Profissional());
        _agendaRepositoryMock.Setup(r => r.ExisteConflitoHorario(profissionalId, dto.DataInicio, dto.DataFim)).ReturnsAsync(false);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _agendaRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Agenda>()), Times.Once);
        _agendaRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        _notificadorMock.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Never);
    }

    [Fact(DisplayName = "Atualizar Agendamento Deve Falhar Quando Não Encontrado")]
    [Trait("Categoria", "Agenda Service NDD")]
    public async Task App_Atualizar_AgendamentoInexistente_DeveNotificarErro()
    {
        // Arrange
        var agendaId = Guid.NewGuid();
        var dto = new AtualizarAgendamentoDto();

        _agendaRepositoryMock.Setup(r => r.ObterPorId(agendaId)).ReturnsAsync((Agenda?)null);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(msg => msg.Mensagem == "Agendamento não encontrado.")), Times.Once);
        _agendaRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Agenda>()), Times.Never);
    }

    [Fact(DisplayName = "Atualizar Agendamento Deve Falhar Quando Profissional Não Existir")]
    [Trait("Categoria", "Agenda Service NDD")]
    public async Task App_Atualizar_ProfissionalInexistente_DeveNotificarErro()
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
            "Obs",
            new Paciente(),
            new Profissional()
        );

        var dto = new AtualizarAgendamentoDto { ProfissionalId = profissionalId };

        _agendaRepositoryMock.Setup(r => r.ObterPorId(agendaId)).ReturnsAsync(agendamentoExistente);
        _profissionalRepositoryMock.Setup(r => r.ObterPorId(profissionalId)).ReturnsAsync((Profissional?)null);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(msg => msg.Mensagem == "Profissional não encontrado.")), Times.Once);
        _agendaRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Agenda>()), Times.Never);
    }

    [Fact(DisplayName = "Atualizar Agendamento Deve Falhar Quando Houver Conflito de Horário")]
    [Trait("Categoria", "Agenda Service NDD")]
    public async Task App_Atualizar_HorarioConflitante_DeveNotificarErro()
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
            "Obs",
            new Paciente(),
            new Profissional()
        );

        var dto = new AtualizarAgendamentoDto
        {
            ProfissionalId = profissionalId,
            DataInicio = DateTime.Now.AddDays(2), // Mudou o horário
            DataFim = DateTime.Now.AddDays(2).AddHours(1)
        };

        _agendaRepositoryMock.Setup(r => r.ObterPorId(agendaId)).ReturnsAsync(agendamentoExistente);
        _profissionalRepositoryMock.Setup(r => r.ObterPorId(profissionalId)).ReturnsAsync(new Profissional());

        // Simula que a agenda do banco diz que JÁ EXISTE uma consulta de outra pessoa nesse mesmo horário novo
        _agendaRepositoryMock.Setup(r => r.ExisteConflitoHorario(profissionalId, dto.DataInicio, dto.DataFim)).ReturnsAsync(true);

        // Act
        await _agendaService.AtualizarAgendamento(agendaId, dto);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(msg => msg.Mensagem == "O profissional já possui um agendamento neste horário.")), Times.Once);
        _agendaRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Agenda>()), Times.Never);
    }
}
