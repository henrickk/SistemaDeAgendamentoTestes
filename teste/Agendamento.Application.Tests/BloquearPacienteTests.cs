using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class BloquearPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService _pacienteService;

    public BloquearPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();

        _pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task BloquearPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange - Organizar
        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);

        var pacienteDto = new BloquearPacienteDto
        {
            PacienteId = paciente.Id,
            Nome = paciente.Nome,
            StatusPaciente = paciente.StatusPaciente
        };

        // Act - Agir
        await _pacienteService.BloquearPaciente(paciente.Id);

        // Assert - Afirmar
        Assert.Equal(pacienteDto.StatusPaciente, paciente.StatusPaciente);

        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacao = _notificador.ObterNotificacoes();
        Assert.Contains(notificacao, n => n.Mensagem == "Paciente não encontrado.");
    }

    [Fact]
    public async Task BloquearPaciente_DeveNotificar_QuandoPacienteJaEstiverBloqueado()
    {
        // Arrange - Organizar
        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Bloqueado);

        var pacienteDto = new BloquearPacienteDto
        {
            PacienteId = paciente.Id,
            Nome = paciente.Nome,
            StatusPaciente = paciente.StatusPaciente
        };
        _pacienteRepositoryMock.Setup(repo => repo.ObterPorId(paciente.Id))
            .ReturnsAsync(paciente);

        // Act - Agir
        await _pacienteService.BloquearPaciente(paciente.Id);

        // Assert - Afirmar
        Assert.Equal(pacienteDto.StatusPaciente, paciente.StatusPaciente);

        Assert.True(_notificador.TemNotificacao());

        var notificacao = _notificador.ObterNotificacoes();
        Assert.Contains(notificacao, n => n.Mensagem == "Paciente já está bloqueado.");
    }
}
