using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class AdicionarNovoPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService _pacienteService;

    public AdicionarNovoPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificador = new Notificador();

        _pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task AdicionarNovoPaciente_DeveCriarPaciente_QuandoDadosForemValidos()
    {
        // Arrange - Organizar
        var pacienteFake = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);

        var paciente = new Paciente
        {
            Nome = pacienteFake.Nome,
            DataNascimento = pacienteFake.DataNascimento,
            CPF = pacienteFake.CPF,
            RG = pacienteFake.RG,
            StatusGenero = pacienteFake.StatusGenero,
            StatusEstadoCivil = pacienteFake.StatusEstadoCivil,
            StatusPaciente = pacienteFake.StatusPaciente,
            Endereco = pacienteFake.Endereco,
            Contato = pacienteFake.Contato
        };

        Paciente pacienteSalvo = null;
        _pacienteRepositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Paciente>()))
            .Callback<Paciente>(p => pacienteSalvo = p)
            .Returns(Task.CompletedTask);

        // Act - Agir
        await _pacienteService.AdicionarNovoPaciente(paciente);

        // Assert - Afirmar
        Assert.False(_notificador.TemNotificacao());
        Assert.NotNull(pacienteSalvo);
        Assert.Equal(paciente.Nome, pacienteSalvo.Nome);
        Assert.Equal(paciente.CPF, pacienteSalvo.CPF);

        _pacienteRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Paciente>()), Times.Once);
    }



    [Fact]
    public async Task AdicionarNovoPaciente_DeveNotificar_QuandoNomeForVazio()
    {
        //Arrange - Organizar
        var nomeVazio = "";

        var paciente = new Paciente
        {
            Nome = nomeVazio,
            DataNascimento = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            CPF = "12345678901",
            RG = "MG1234567",
            StatusGenero = StatusGenero.Masculino,
            StatusEstadoCivil = StatusEstadoCivil.Solteiro,
            StatusPaciente = StatusPaciente.Ativo,
            Endereco = new Endereco { Logradouro = "Rua A", Numero = "123", Cidade = "Cidade X", UF = "SP", CEP = "12345-678" },
            Contato = new Contato(email: "teste@teste.com", numeroCelular: "1199999-9999")
        };

        //Act - Agir
        await _pacienteService.AdicionarNovoPaciente(paciente);

        //Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacoes = _notificador.ObterNotificacoes();
        Assert.Contains(notificacoes, n => n.Mensagem == "O nome é obrigatório.");

        _pacienteRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Paciente>()), Times.Never);
    }

    [Fact]
    public async Task AdicionarNovoPaciente_DeveNotificar_QuandoNumeroDeCelularForVazio()
    {
        //Arrange - Organizar
        var numeroVazio = "";

        var paciente = new Paciente
        {
            Nome = "Nome Teste",
            DataNascimento = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            CPF = "12345678901",
            RG = "MG1234567",
            StatusGenero = StatusGenero.Masculino,
            StatusEstadoCivil = StatusEstadoCivil.Solteiro,
            StatusPaciente = StatusPaciente.Ativo,
            Endereco = new Endereco { Logradouro = "Rua A", Numero = "123", Cidade = "Cidade X", UF = "SP", CEP = "12345-678" },
            Contato = new Contato(email: "teste@teste.com", numeroCelular: numeroVazio)
        };

        //Act - Agir
        await _pacienteService.AdicionarNovoPaciente(paciente);

        //Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacoes = _notificador.ObterNotificacoes();
        Assert.Contains(notificacoes, n => n.Mensagem == "O número de celular é obrigatório.");

        _pacienteRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Paciente>()), Times.Never);
    }

    [Fact]
    public async Task AdicionarNovoPaciente_DeveNotificar_QuandoCPFNaoForAdicionado()
    {
        //Arrange - Organizar
        var cpfVazio = "";

        var paciente = new Paciente
        {
            Nome = "Nome Teste",
            DataNascimento = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            CPF = cpfVazio,
            RG = "MG1234567",
            StatusGenero = StatusGenero.Masculino,
            StatusEstadoCivil = StatusEstadoCivil.Solteiro,
            StatusPaciente = StatusPaciente.Ativo,
            Endereco = new Endereco { Logradouro = "Rua A", Numero = "123", Cidade = "Cidade X", UF = "SP", CEP = "12345-678" },
            Contato = new Contato(email: null, numeroCelular: "1199999-9999")
        };

        //Act - Agir
        await _pacienteService.AdicionarNovoPaciente(paciente);

        //Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacoes = _notificador.ObterNotificacoes();
        Assert.Contains(notificacoes, n => n.Mensagem == "O CPF é obrigatório.");

        _pacienteRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Paciente>()), Times.Never);
    }
}
