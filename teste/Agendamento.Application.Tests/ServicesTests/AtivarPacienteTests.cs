using Agendamento.Application.Services;
using Agendamento.Application.Tests.ServiceTests.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests.ServiceTests;
public class AtivarPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService _pacienteService;

    public AtivarPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _notificador = new Notificador();
        _pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task AtivarPaciente_DeveAtivar_QuandoPacienteEstiverInativo()
    {
        // Arrange - Organizar
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Inativo);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        _pacienteRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Paciente>()))
            .Returns(Task.CompletedTask);

        // Act - Agir
        await _pacienteService.AtivarPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.False(_notificador.TemNotificacao());
        Assert.Equal(StatusPaciente.Ativo, pacienteFake.StatusPaciente);

        _pacienteRepositoryMock.Verify(r => r.Atualizar(pacienteFake), Times.Once);
    }


    [Fact]
    public async Task AtivarPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange - Organizar
        var idInexistente = Guid.NewGuid();

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idInexistente))
            .ReturnsAsync((Paciente)null);

        // Act - Agir
        await _pacienteService.AtivarPaciente(idInexistente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Paciente não encontrado.");

        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }


    [Fact]
    public async Task AtivarPaciente_DeveNotificar_QuandoPacienteJaEstiverAtivo()
    {
        // Arrange - Organizar 
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        // Act - Agir
        await _pacienteService.AtivarPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Paciente já está ativo.");

        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }


    [Fact]
    public async Task AtivarPaciente_DeveNotificar_QuandoPacienteEstiverBloqueado()
    {
        // Arrange - Organizar
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Bloqueado);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        // Act - Agir
        await _pacienteService.AtivarPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Paciente está bloqueado e não pode ser ativado.");

        Assert.Equal(StatusPaciente.Bloqueado, pacienteFake.StatusPaciente);
        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }

}
