using Agendamento.Application.Interfaces;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.Auxiliar;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
using System.Linq.Expressions;

namespace Agendamento.Application.Tests;
public class AdicionarNovoProfissionalTests
{
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly IProfissionalService _profissionalService;
    private readonly Notificador _notificador;

    public AdicionarNovoProfissionalTests()
    {
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();

        _profissionalService = new ProfissionalService(_profissionalRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task AdicionarNovoProfissional_DeveDeveCriarPaciente_QuandoDadosForemValidos()
    {
        // Arrange
        var profissionalServiceMock = new Mock<IProfissionalService>();

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
            .Setup(r => r.Adicionar(It.IsAny<Profissional>()))
            .Callback<Profissional>(p => profissionalSalvo = p)
            .Returns(Task.CompletedTask);

        // Act
        await _profissionalService.AdicionarNovoProfissional(profissional);

        // Assert

        Assert.NotNull(profissionalSalvo);
        Assert.Equal(profissionalFake.Nome, profissionalSalvo.Nome);
        Assert.Equal(profissionalFake.CRO, profissionalSalvo.CRO);
        Assert.Equal(profissionalFake.CPF, profissionalSalvo.CPF);
        Assert.Equal(profissionalFake.HoraInicio, profissionalSalvo.HoraInicio);
        Assert.Equal(profissionalFake.HoraFim, profissionalSalvo.HoraFim);

        _profissionalRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Profissional>()), Times.Once);

    }

    [Fact]
    public async Task AdicionarNovoProfissional_DeveNotificar_QuandoProfissionalJaExistir()
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

        // Corrigido: usar Expression<Func<Profissional, bool>>
        Expression<Func<Profissional, bool>> filtro = p => p.CRO == profissional.CRO;
        _profissionalRepositoryMock
            .Setup(r => r.Buscar(It.Is<Expression<Func<Profissional, bool>>>(exp => exp.Compile().Invoke(profissional))))
            .ReturnsAsync(new List<Profissional> { profissional });

        // Act
        await _profissionalService.AdicionarNovoProfissional(profissional);

        // Assert
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Já existe um profissional com este CRO.");
        _profissionalRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Profissional>()), Times.Never);
    }

    [Fact]
    public async Task AdicionarNovoProfissional_DeveNotificarQuandoProfissionalNaoExistir()
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
        // Corrigido: usar Expression<Func<Profissional, bool>>
        Expression<Func<Profissional, bool>> filtro = p => p.CRO == profissional.CRO;
        _profissionalRepositoryMock
            .Setup(r => r.Buscar(It.Is<Expression<Func<Profissional, bool>>>(exp => exp.Compile().Invoke(profissional))))
            .ReturnsAsync((IEnumerable<Profissional>)null);

        // Act
        await _profissionalService.AdicionarNovoProfissional(profissional);

        // Assert
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Profissional não encontrado.");
        _profissionalRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Profissional>()), Times.Never);
    }
}
