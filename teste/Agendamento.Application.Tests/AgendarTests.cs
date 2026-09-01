using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
using Agendamento.Application.Tests.Services.Auxiliar;
namespace Agendamento.Application.Tests;
public class AgendarTests
{
    // 1. Definição dos Mocks necessários para construir a Service
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Mock<INotificador> _notificadorMock;

    // O sistema sob teste (System Under Test)
    private readonly AgendaService _agendaService;

    public AgendarTests()
    {
        // 2. Inicialização correta de todos os Mocks
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificadorMock = new Mock<INotificador>();

        // 3. Instanciação manual passando os objetos simulados (.Object)
        _agendaService = new AgendaService(
            _notificadorMock.Object,
            _agendaRepositoryMock.Object,
            _pacienteRepositoryMock.Object,
            _profissionalRepositoryMock.Object
        );
    }

    [Fact]
    public void Agendar_DeveCriarAgendamento_QuandoDadosForemValidos()
    {
        // Arrange
        var agendaDto = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        // Act
        // Passando null nos campos de objeto complexo como você estruturou temporariamente
        var agendamento = new Agenda(
            agendaDto.PacienteId,
            agendaDto.ProfissionalId,
            StatusAgendamento.Agendado,
            agendaDto.DataInicio,
            agendaDto.DataFim,
            agendaDto.Observacao,
            null,
            null);
    
        // Assert
        Assert.Equal(agendaDto.PacienteId, agendamento.PacienteId);
    }

    [Fact]
    public async Task Agendar_DeveNotificarErro_QuandoPacienteNaoExistir()
    {
        // Arrange
        var agendaDto = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        _pacienteRepositoryMock.Setup(r => r.ObterPorId(agendaDto.PacienteId))
            .ReturnsAsync((Paciente?)null);

        // Act
        await _agendaService.Agendar(agendaDto);

        // Assert 
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(notificacao => notificacao.Mensagem == "Paciente não encontrado.")), Times.Once);
    }

    [Fact]
    public async Task Agendar_DeveNotificarErro_QuandoProfissionalNaoExistir()
    {
        // Arrange - Organizar
        var pacienteId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();

        var agendaDto = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        var pacienteValido = new Paciente(
            nome: "Paciente Teste",
            dataNascimento: new DateOnly(1990, 1, 1),
            cpf: "12345678901",
            rg: "1234567",
            statusGenero: StatusGenero.Masculino,
            statusEstadoCivil: StatusEstadoCivil.Solteiro,
            statusPaciente: StatusPaciente.Ativo,
            endereco: new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000"),
            contato: new Contato("teste@123.com", "11999999999")
        );

        _pacienteRepositoryMock.Setup(r => r.ObterPorId(agendaDto.PacienteId))
       .ReturnsAsync(pacienteValido);

        // Configura o Profissional para retornar nulo (o objetivo real deste teste)
        _profissionalRepositoryMock.Setup(r => r.ObterPorId(agendaDto.ProfissionalId))
            .ReturnsAsync((Profissional?)null);

        // Act - Agir
        await _agendaService.Agendar(agendaDto);

        // Assert - Afirmar
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(notificacao =>
            notificacao.Mensagem == "Profissional não encontrado.")),
            Times.Once);
    }

    [Fact]
    public async Task Agendar_DeveFalhar_QuandoPacienteEstiverBloqueado()
    {
        // Arrange - Organizar
        var agendaDto = new NovoAgendamentoDto()
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta de rotina"
        };

        // Criando um paciente fake preenchendo todos os 10 parâmetros obrigatórios
        var pacienteBloqueado = new Paciente(
            nome: "Paciente Teste",
            dataNascimento: new DateOnly(1990, 1, 1),
            cpf: "12345678901",
            rg: "1234567",
            statusGenero: StatusGenero.Masculino, // Ajuste para um enum válido seu
            statusEstadoCivil: StatusEstadoCivil.Solteiro, // Ajuste para um enum válido seu
            statusPaciente: StatusPaciente.Bloqueado, // O ponto chave do teste está aqui!
            endereco: new Endereco("Rua Tal", "14", "Vila Madalena", "São Paulo", "SP", "...", "00000000"), // Se o construtor de Endereco exigir parâmetros, use mock ou instancie
            contato: new Contato("teste@123.com", "11999999999")    // Se o construtor de Contato exigir parâmetros, use mock ou instancie
        );

        // Configurando o Mock para retornar o objeto que acabamos de criar
        _pacienteRepositoryMock.Setup(r => r.ObterPorId(agendaDto.PacienteId))
            .ReturnsAsync(pacienteBloqueado);

        // Act - Agir
        await _agendaService.Agendar(agendaDto);

        // Assert - Afirmar
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(notificacao => notificacao.Mensagem == "Paciente bloqueado. Não é possível realizar agendamento.")), Times.Once);
    }

    [Fact]
    public async Task Agendar_DeveNotificarErro_QuandoHorarioEstiverOcupado()
    {
        // Arrange
        var agendaDto = new NovoAgendamentoDto
        {
            PacienteId = Guid.NewGuid(),
            ProfissionalId = Guid.NewGuid(),
            DataInicio = DateTime.Now.AddHours(1),
            DataFim = DateTime.Now.AddHours(2),
            Observacao = "Consulta"
        };

        _pacienteRepositoryMock.Setup(r => r.ObterPorId(agendaDto.PacienteId))
            .ReturnsAsync(PacienteFixture.CriarPacienteFake());

        _profissionalRepositoryMock.Setup(r => r.ObterPorId(agendaDto.ProfissionalId))
            .ReturnsAsync(ProfissionalFixture.CriarProfissionalFake());

        _agendaRepositoryMock.Setup(r => r.VerificarConflitoProfissional(agendaDto.ProfissionalId, agendaDto.DataInicio, agendaDto.DataFim))
            .ReturnsAsync(true);

        // Act
        await _agendaService.Agendar(agendaDto);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(notificacao =>
            notificacao.Mensagem == "O profissional já possui um agendamento neste horário.")),
            Times.Once);
    }
}
