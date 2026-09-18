using Agendamento.Application.Interfaces;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class RemoverProfissionalTests
{
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly IProfissionalService _profissionalService;
    private readonly Notificador _notificador;
    private readonly ProfissionalService profissionalService;

    public RemoverProfissionalTests()
    {
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();
        profissionalService = new ProfissionalService(_profissionalRepositoryMock.Object, _notificador);

        _profissionalService = new ProfissionalService(_profissionalRepositoryMock.Object, _notificador);

    }

    [Fact]
    public async Task RemoverProfissional_DeveRemover_QuandoProfissionalExistente()
    {
        // Arrange
        var profissional = ProfissionalFixture.CriarProfissionalFake();
        var profissionalId = profissional.Id;

        var profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        var notificadorMock = new Mock<INotificador>();

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissionalId))
            .ReturnsAsync(profissional);

        _profissionalRepositoryMock
            .Setup(r => r.Remover(profissionalId))
            .Returns(Task.CompletedTask);

        // Act
        await profissionalService.RemoverProfissional(profissionalId);

        // Assert
        Assert.False(_notificador.TemNotificacao());
        _profissionalRepositoryMock.Verify(r => r.Remover(profissionalId), Times.Once);
    }

    [Fact]
    public async Task RemoverProfissional_DeveNotificar_QuandoProfissionalNaoExistir()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(idInexistente))
            .ReturnsAsync((Profissional)null);

        // Act
        await profissionalService.RemoverProfissional(idInexistente);

        // Assert
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Profissional não encontrado.");

        _profissionalRepositoryMock.Verify(r => r.Remover(It.IsAny<Guid>()), Times.Never);
    }
}
