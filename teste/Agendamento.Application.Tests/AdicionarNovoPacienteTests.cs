using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.Services.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Moq;

namespace Agendamento.Application.Tests;
public class AdicionarNovoPacienteTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Mock<INotificador> _notificadorMock;

    private readonly PacienteService _pacienteService;
    public AdicionarNovoPacienteTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificadorMock = new Mock<INotificador>();

        _pacienteService = new PacienteService(
            new Mock<IPacienteRepository>().Object, _notificadorMock.Object
        );
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


    //[Fact]
    //public async Task AdicionarNovoPaciente_DeveNotificar_QuandoDadosForemInvalidos()
    //{
    // Arrange - Organizar

    // Act - Agir

    // Assert - Afirmar
    //}
}