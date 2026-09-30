using Agendamento.Application.Services;
using Agendamento.Application.Tests.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
using System.Linq.Expressions;

namespace Agendamento.Application.Tests;

public class AdicionarNovoProfissionalTests
{
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Notificador _notificador;
    private readonly ProfissionalService _profissionalService;

    public AdicionarNovoProfissionalTests()
    {
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();

        _profissionalService = new ProfissionalService(
            _profissionalRepositoryMock.Object,
            _notificador);
    }

    [Fact]
    public async Task AdicionarNovoProfissional_DeveAdicionar_QuandoCRONaoEstiverCadastrado()
    {
        // Arrange
        var profissionalFake = ProfissionalFixture.CriarProfissionalFake();

        var profissional = new Profissional
        {
            Nome = profissionalFake.Nome,
            CRO = profissionalFake.CRO,
            CPF = profissionalFake.CPF,
            HoraInicio = profissionalFake.HoraInicio,
            HoraFim = profissionalFake.HoraFim,
            Contato = profissionalFake.Contato,
            Endereco = profissionalFake.Endereco
        };

        Profissional profissionalSalvo = null;

        _profissionalRepositoryMock
            .Setup(r => r.Buscar(
                It.IsAny<Expression<Func<Profissional, bool>>>()))
            .ReturnsAsync(new List<Profissional>());

        _profissionalRepositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Profissional>()))
            .Callback<Profissional>(p => profissionalSalvo = p)
            .Returns(Task.CompletedTask);

        _profissionalRepositoryMock
            .Setup(r => r.SaveChanges())
            .ReturnsAsync(0);

        // Act
        await _profissionalService.AdicionarNovoProfissional(profissional);

        // Assert
        Assert.NotNull(profissionalSalvo);

        Assert.Equal(profissional.Nome, profissionalSalvo.Nome);
        Assert.Equal(profissional.CRO, profissionalSalvo.CRO);
        Assert.Equal(profissional.CPF, profissionalSalvo.CPF);
        Assert.Equal(profissional.HoraInicio, profissionalSalvo.HoraInicio);
        Assert.Equal(profissional.HoraFim, profissionalSalvo.HoraFim);

        _profissionalRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Profissional>()),
            Times.Once);

        _profissionalRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);

        Assert.False(_notificador.TemNotificacao());
    }

    [Fact]
    public async Task AdicionarNovoProfissional_DeveNotificar_QuandoCROJaEstiverCadastrado()
    {
        // Arrange
        var profissionalFake = ProfissionalFixture.CriarProfissionalFake();

        var profissional = new Profissional
        {
            Nome = profissionalFake.Nome,
            CRO = profissionalFake.CRO,
            CPF = profissionalFake.CPF,
            HoraInicio = profissionalFake.HoraInicio,
            HoraFim = profissionalFake.HoraFim,
            Contato = profissionalFake.Contato,
            Endereco = profissionalFake.Endereco
        };

        _profissionalRepositoryMock
            .Setup(r => r.Buscar(
                It.IsAny<Expression<Func<Profissional, bool>>>()))
            .ReturnsAsync(new List<Profissional>
            {
                profissional
            });

        // Act
        await _profissionalService.AdicionarNovoProfissional(profissional);

        // Assert
        Assert.True(_notificador.TemNotificacao());

        Assert.Contains(
            _notificador.ObterNotificacoes(),
            n => n.Mensagem == "Já existe um profissional com este CRO.");

        _profissionalRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Profissional>()),
            Times.Never);

        _profissionalRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }
}