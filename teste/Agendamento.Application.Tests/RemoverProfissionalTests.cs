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

    public RemoverProfissionalTests()
    {
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();

        _profissionalService = new ProfissionalService(
            _profissionalRepositoryMock.Object,
            _notificador);
    }

    [Fact]
    public async Task RemoverProfissional_DeveRemover_QuandoProfissionalExistir()
    {
        // Arrange
        var profissional = ProfissionalFixture.CriarProfissionalFake();
        var profissionalId = profissional.Id;

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissionalId))
            .ReturnsAsync(profissional);

        _profissionalRepositoryMock
            .Setup(r => r.Remover(profissionalId))
            .Returns(Task.CompletedTask);

        // Act
        await _profissionalService.RemoverProfissional(profissionalId);

        // Assert
        Assert.False(_notificador.TemNotificacao());

        _profissionalRepositoryMock.Verify(
            r => r.Remover(profissionalId),
            Times.Once);
    }

    [Fact]
    public async Task RemoverProfissional_DeveNotificar_QuandoProfissionalNaoExistir()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(idInexistente))
            .ReturnsAsync((Profissional?)null);

        // Act
        await _profissionalService.RemoverProfissional(idInexistente);

        // Assert
        var notificacao = Assert.Single(
            _notificador.ObterNotificacoes());

        Assert.Equal(
            "Profissional não encontrado.",
            notificacao.Mensagem);

        _profissionalRepositoryMock.Verify(
            r => r.Remover(It.IsAny<Guid>()),
            Times.Never);
    }
}