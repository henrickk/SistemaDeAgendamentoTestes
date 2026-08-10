using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;

public class AgendaTests
{
    // 1. Definição dos Mocks necessários para construir a Service
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Mock<INotificador> _notificadorMock;

    // O sistema sob teste (System Under Test)
    private readonly AgendaService _agendaService;

    public AgendaTests()
    {
        // 2. Inicialização correta de todos os Mocks
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificadorMock = new Mock<INotificador>();

        // 3. Instanciação manual passando os objetos simulados (.Object)
        _agendaService = new AgendaService(
            _notificadorMock.Object,
            _agendaRepositoryMock.Object,
            _pacienteRepositoryMock.Object,
            _profissionalRepositoryMock.Object
        );
    }

    [Fact]
    public void Agendar_DeveCriarAgendamento_QuandoDadosForemValidos()
    {
        // Arrange
        var agendaDto = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        // Act
        // Passando null nos campos de objeto complexo como você estruturou temporariamente
#pragma warning disable CS8625
        var agendamento = new Agenda(
            agendaDto.PacienteId,
            agendaDto.ProfissionalId,
            StatusAgendamento.Agendado,
            agendaDto.DataInicio,
            agendaDto.DataFim,
            agendaDto.Observacao,
            null,
            null);
#pragma warning restore CS8625

        // Assert
        Assert.Equal(agendaDto.PacienteId, agendamento.PacienteId);
    }

    [Fact]
    public async Task Agendar_DeveNotificarErro_QuandoPacienteNaoExistir()
    {
        // Arrange
        var agendaDto = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        // Configura o Mock para retornar nulo simulando que o paciente não existe
        _pacienteRepositoryMock.Setup(r => r.ObterPorId(agendaDto.PacienteId))
            .ReturnsAsync((Paciente?)null);

        // Act
        await _agendaService.Agendar(agendaDto);

        // Assert 
        // Corrigido para criar uma instância de Notificacao ao invés de passar uma string diretamente
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(notificacao => notificacao.Mensagem == "Paciente não encontrado.")), Times.Once);
    }



    [Fact]
    public void Agendar_DeveFalhar_QuandoProfissionalNaoExistir()
    {
        // Arrange - Organizar

        // Act - Agir

        // Assert - Afirmar
    }

    [Fact]
    public void Agendar_DeveFalhar_QuandoPacienteEstiverBloqueado()
    {
        // Arrange - Organizar

        // Act - Agir

        // Assert - Afirmar
    }

    [Fact]
    public void Agendar_DeveFalhar_QuandoExistirConflitoDeHorario()
    {
        // Arrange - Organizar

        // Act - Agir

        // Assert - Afirmar
    }
}