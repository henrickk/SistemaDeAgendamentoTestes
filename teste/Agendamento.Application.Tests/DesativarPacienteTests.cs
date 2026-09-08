using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class DesativarPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService _pacienteService;

    public DesativarPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _notificador = new Notificador();
        _pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task DesativarPaciente_DeveDesativar_QuandoPacienteEstiverAtivo()
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
        await _pacienteService.DesativarPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.False(_notificador.TemNotificacao());
        Assert.Equal(StatusPaciente.Inativo, pacienteFake.StatusPaciente);

        _pacienteRepositoryMock.Verify(r => r.Atualizar(pacienteFake), Times.Once);
    }



    [Fact]
    public async Task DesativarPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange - Organizar
        var idInexistente = Guid.NewGuid();

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idInexistente))
            .ReturnsAsync((Paciente)null);

        // Act - Agir
        await _pacienteService.DesativarPaciente(idInexistente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Paciente não encontrado.");

        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }


    [Fact]
    public async Task DesativarPaciente_DeveNotificar_QuandoPacienteJaEstiverInativo()
    {
        // Arrange - Organizar
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Inativo);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        // Act - Agir
        await _pacienteService.DesativarPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Paciente já está inativo.");

        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }


    [Fact]
    public async Task DesativarPaciente_DeveNotificar_QuandoPacienteEstiverBloqueado()
    {
        // Arrange - Organizar
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Bloqueado);
        var idDoPaciente = pacienteFake.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idDoPaciente))
            .ReturnsAsync(pacienteFake);

        // Act - Agir
        await _pacienteService.DesativarPaciente(idDoPaciente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Paciente está bloqueado e não pode ser desativado.");

        Assert.Equal(StatusPaciente.Bloqueado, pacienteFake.StatusPaciente);
        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Never);
    }
}
