using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;

namespace Agendamento.Application.Tests;
public class AtualizarInfoPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PacienteService _pacienteService;

    public AtualizarInfoPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _notificador = new Notificador();
        _pacienteService = new PacienteService(_pacienteRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task AtualizarInfoPaciente_DeveAtualizar_QuandoPacienteExistir()
    {
        // Arrange - Organizar
        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);

        var pacienteDto = new AtualizarPacienteDto
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            DataDeNascimento = paciente.DataNascimento,
            StatusGenero = paciente.StatusGenero,
            StatusEstadoCivil = paciente.StatusEstadoCivil,
            StatusPaciente = paciente.StatusPaciente,
            Endereco = paciente.Endereco,
            Contato = paciente.Contato
        };

        // Act - Agir
        await _pacienteService.AtualizarInfoPaciente(pacienteDto);
        pacienteDto.Id = paciente.Id;

        // Assert - Afirmar
        Assert.Equal(pacienteDto.Id, paciente.Id);
        Assert.Equal(pacienteDto.Nome, paciente.Nome);
    }

    [Fact]
    public async Task AtualizarInfoPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange - Organizar
        var pacienteDto = new AtualizarPacienteDto
        {
            Id = Guid.NewGuid(),
            Nome = "Paciente Inexistente",
            DataDeNascimento = new DateOnly(1990, 1, 1),
            StatusGenero = StatusGenero.Masculino,
            StatusEstadoCivil = StatusEstadoCivil.Solteiro,
            StatusPaciente = StatusPaciente.Ativo,
            Endereco = new Endereco(),
            Contato = new Contato( email: "", numeroCelular: "1199999-9999")
        };
        
        // Act - Agir
        await _pacienteService.AtualizarInfoPaciente(pacienteDto);

        // Assert - Afirmar
        Assert.True(_notificador.TemNotificacao());
        Assert.Equal(1, _notificador.ObterNotificacoes().Count);

        var notificacao = _notificador.ObterNotificacoes();
        Assert.Contains(notificacao, n => n.Mensagem == "Paciente não encontrado.");
    }
}
