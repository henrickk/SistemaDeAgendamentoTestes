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
        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);

        var pacienteDto = new NovoPacienteDto
        {
            PacienteId = paciente.Id,
            Nome = paciente.Nome,
            DataDeNascimento = paciente.DataNascimento,
            CPF = paciente.CPF,
            RG = paciente.RG,
            StatusGenero = paciente.StatusGenero,
            StatusEstadoCivil = paciente.StatusEstadoCivil,
            StatusPaciente = paciente.StatusPaciente,
            Endereco = paciente.Endereco,
            Contato = paciente.Contato
        };

        // Act - Agir
        await _pacienteService.AdicionarNovoPaciente(pacienteDto);

        pacienteDto.PacienteId = paciente.Id;

        // Assert - Afirmar
        Assert.Equal(pacienteDto.PacienteId, paciente.Id);
        Assert.Equal(pacienteDto.Nome, paciente.Nome);
        Assert.NotEqual(Guid.Empty, paciente.Id);
    }


    [Fact]
    public async Task AdicionarNovoPaciente_DeveNotificar_QuandoNomeForVazio()
    {
        //Arrange - Organizar
        var nomeVazio = "";

        var pacienteDto = new NovoPacienteDto
        {
            PacienteId = Guid.NewGuid(),
            Nome = nomeVazio,
            DataDeNascimento = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            CPF = "12345678901",
            RG = "MG1234567",
            StatusGenero = StatusGenero.Masculino,
            StatusEstadoCivil = StatusEstadoCivil.Solteiro,
            StatusPaciente = StatusPaciente.Ativo,
            Endereco = new Endereco { Logradouro = "Rua A", Numero = "123", Cidade = "Cidade X", UF = "SP", CEP = "12345-678" }
        };

        //Act - Agir
        await _pacienteService.AdicionarNovoPaciente(pacienteDto);

        //Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacoes = _notificador.ObterNotificacoes();
        Assert.Contains(notificacoes, n => n.Mensagem == "O nome é obrigatório.");
    }
}