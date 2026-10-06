using Agendamento.Application.Services;
using Agendamento.Application.Tests.ServiceTests.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests.ServiceTests;

public class BloquearPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService _pacienteService;

    public BloquearPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _notificador = new Notificador();

        _pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task BloquearPaciente_DeveBloquear_QuandoPacienteExistirEEstiverAtivo()
    {
        // Arrange - Organizar
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        _pacienteRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Paciente>()))
            .Returns(Task.CompletedTask);

        // Act - Agir
        await _pacienteService.BloquearPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.False(_notificador.TemNotificacao()); 
        Assert.Equal(StatusPaciente.Bloqueado, pacienteFake.StatusPaciente); 

        _pacienteRepositoryMock.Verify(r => r.Atualizar(pacienteFake), Times.Once);
    }

    [Fact]
    public async Task BloquearPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange - Organizar
        var idInexistente = Guid.NewGuid();

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idInexistente))
            .ReturnsAsync((Paciente)null);

        // Act - Agir
        await _pacienteService.BloquearPaciente(idInexistente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacao = _notificador.ObterNotificacoes();
        Assert.Contains(notificacao, n => n.Mensagem == "Paciente não encontrado.");

        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }

    [Fact]
    public async Task BloquearPaciente_DeveNotificar_QuandoPacienteJaEstiverBloqueado()
    {
        // Arrange - Organizar 
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Bloqueado);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        // Act - Agir
        await _pacienteService.BloquearPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacao = _notificador.ObterNotificacoes();
        Assert.Contains(notificacao, n => n.Mensagem == "Paciente já está bloqueado.");

        Assert.Equal(StatusPaciente.Bloqueado, pacienteFake.StatusPaciente);
        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }
}
