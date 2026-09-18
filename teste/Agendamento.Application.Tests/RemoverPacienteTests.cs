using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class RemoverPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService pacienteService;

    public RemoverPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();
        pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task RemoverPaciente_DeveRemover_QuandoPacienteExistente()
    {
        // Arrange
        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);
        var pacienteId = paciente.Id;

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(pacienteId))
            .ReturnsAsync(paciente);

        _pacienteRepositoryMock
            .Setup(r => r.Remover(pacienteId))
            .Returns(Task.CompletedTask);

        // Act
        await pacienteService.RemoverPaciente(pacienteId);

        // Assert
        Assert.False(_notificador.TemNotificacao());
        _pacienteRepositoryMock.Verify(r => r.Remover(pacienteId), Times.Once);
    }

    [Fact]
    public async Task RemoverPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange - Organizar
        var idInexistente = Guid.NewGuid();

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(idInexistente))
            .ReturnsAsync((Paciente)null);

        // Act - Agir
        await pacienteService.RemoverPaciente(idInexistente);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacoes = _notificador.ObterNotificacoes();
        Assert.Contains(notificacoes, n => n.Mensagem == "Paciente não encontrado.");

        _pacienteRepositoryMock.Verify(r => r.Remover(It.IsAny<Guid>()), Times.Never);
    }

}
