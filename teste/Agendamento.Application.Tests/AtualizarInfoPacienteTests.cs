using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
using System.Linq.Expressions;

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
        var pacienteExistente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);

        pacienteExistente.Nome = "Nome Atualizado do Paciente";

        _pacienteRepositoryMock
            .Setup(r => r.Buscar(It.IsAny<Expression<Func<Paciente, bool>>>()))
            .ReturnsAsync(new List<Paciente> { pacienteExistente });


        Paciente pacienteAtualizado = null;

        _pacienteRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Paciente>()))
            .Callback<Paciente>(p => pacienteAtualizado = p)
            .Returns(Task.CompletedTask);

        // Act - Agir
        await _pacienteService.AtualizarInfoPaciente(pacienteExistente);

        // Assert - Afirmar
        Assert.False(_notificador.TemNotificacao());

        Assert.NotNull(pacienteAtualizado);
        Assert.Equal(pacienteExistente.Nome, pacienteAtualizado.Nome);

        _pacienteRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Paciente>()), Times.Once);
    }


    [Fact]
    public async Task AtualizarInfoPaciente_DeveNotificar_QuandoPacienteNaoExistir()
    {
        // Arrange
        var paciente = new Paciente
        {
            Nome = "Paciente Inexistente",
            DataNascimento = new DateOnly(1990, 1, 1),
            StatusGenero = StatusGenero.Masculino,
            StatusEstadoCivil = StatusEstadoCivil.Solteiro,
            StatusPaciente = StatusPaciente.Ativo,
            Endereco = new Endereco { Logradouro = "Rua A", Numero = "123", Cidade = "Cidade X", UF = "SP", CEP = "12345-678" },
            Contato = new Contato(email: "", numeroCelular: "1199999-9999")
        };

        _pacienteRepositoryMock
            .Setup(r => r.Buscar(
                It.IsAny<Expression<Func<Paciente, bool>>>()))
            .ReturnsAsync(new List<Paciente>());

        // Act
        await _pacienteService.AtualizarInfoPaciente(paciente);

        // Assert
        Assert.True(_notificador.TemNotificacao());

        Assert.Equal(
            "Paciente não encontrado.",
            _notificador.ObterNotificacoes().First().Mensagem);

        _pacienteRepositoryMock.Verify(
            r => r.Atualizar(It.IsAny<Paciente>()),
            Times.Never);
    }
}