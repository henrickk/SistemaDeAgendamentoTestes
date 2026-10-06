using Agendamento.Application.Interfaces;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.ServiceTests.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
using System.Reflection;

namespace Agendamento.Application.Tests.ServiceTests;

public class AtualizarProfissionalTests
{
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly IProfissionalService _profissionalService;
    private readonly Notificador _notificador;

    public AtualizarProfissionalTests()
    {
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();

        _profissionalService = new ProfissionalService(
            _profissionalRepositoryMock.Object,
            _notificador);
    }

    [Fact]
    public async Task AtualizarProfissional_DeveAtualizar_QuandoProfissionalExistir()
    {
        // Arrange
        var profissionalExistente = ProfissionalFixture.CriarProfissionalFake();

        var profissional = new Profissional
        {
            Nome = "Novo Nome Atualizado",
            HoraInicio = profissionalExistente.HoraInicio,
            HoraFim = profissionalExistente.HoraFim,
            Contato = profissionalExistente.Contato,
            Endereco = profissionalExistente.Endereco
        };

        typeof(Profissional)
            .GetProperty(
                nameof(Profissional.Id),
                BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(profissional, profissionalExistente.Id);

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissional.Id))
            .ReturnsAsync(profissionalExistente);

        Profissional profissionalAtualizado = null;

        _profissionalRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Profissional>()))
            .Callback<Profissional>(p => profissionalAtualizado = p);

        _profissionalRepositoryMock
            .Setup(r => r.SaveChanges())
            .ReturnsAsync(0);

        // Act
        await _profissionalService.AtualizarProfissional(profissional);

        // Assert
        Assert.False(_notificador.TemNotificacao());

        Assert.NotNull(profissionalAtualizado);
        Assert.Equal(profissional.Nome, profissionalAtualizado.Nome);

        _profissionalRepositoryMock.Verify(
            r => r.Atualizar(profissional),
            Times.Once);

        _profissionalRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarProfissional_DeveNotificar_QuandoProfissionalNaoExistir()
    {
        // Arrange
        var profissional = new Profissional
        {
            Nome = "Profissional Teste",
            HoraInicio = new TimeOnly(8, 0),
            HoraFim = new TimeOnly(17, 0),
            Contato = new Contato(
                email: "",
                numeroCelular: "1199999-9999")
        };

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(profissional.Id))
            .ReturnsAsync((Profissional?)null);

        // Act
        await _profissionalService.AtualizarProfissional(profissional);

        // Assert
        var notificacao = Assert.Single(
            _notificador.ObterNotificacoes());

        Assert.Equal(
            "Profissional não encontrado.",
            notificacao.Mensagem);

        _profissionalRepositoryMock.Verify(
            r => r.Atualizar(It.IsAny<Profissional>()),
            Times.Never);

        _profissionalRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }
}