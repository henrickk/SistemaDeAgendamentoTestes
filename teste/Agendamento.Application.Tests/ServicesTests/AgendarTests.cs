using Agendamento.Application.DTOs;
using Agendamento.Application.Services;
using Agendamento.Application.Tests.ServiceTests.Auxiliar;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using Moq;
namespace Agendamento.Application.Tests.ServiceTests;
public class AgendarTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly Mock<IAgendaRepository> _agendaRepositoryMock;
    private readonly Mock<IProfissionalRepository> _profissionalRepositoryMock;
    private readonly Mock<INotificador> _notificadorMock;

    private readonly AgendaService _agendaService;

    public AgendarTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _agendaRepositoryMock = new Mock<IAgendaRepository>();
        _profissionalRepositoryMock = new Mock<IProfissionalRepository>();
        _notificadorMock = new Mock<INotificador>();

        _agendaService = new AgendaService(
            _notificadorMock.Object,
            _agendaRepositoryMock.Object,
            _pacienteRepositoryMock.Object,
            _profissionalRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Agendar_DeveCriarAgendamento_QuandoDadosForemValidos()
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

        var paciente = PacienteFixture.CriarPacienteFake(StatusPaciente.Ativo);
        var profissional = ProfissionalFixture.CriarProfissionalFake();

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorId(agendaDto.PacienteId))
            .ReturnsAsync(paciente);

        _profissionalRepositoryMock
            .Setup(r => r.ObterPorId(agendaDto.ProfissionalId))
            .ReturnsAsync(profissional);

        _agendaRepositoryMock
            .Setup(r => r.ExisteConflitoHorario(
                agendaDto.ProfissionalId,
                agendaDto.DataInicio,
                agendaDto.DataFim))
            .ReturnsAsync(false);

        Agenda agendamentoSalvo = null;

        _agendaRepositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Agenda>()))
            .Callback<Agenda>(a => agendamentoSalvo = a)
            .Returns(Task.CompletedTask);

        _agendaRepositoryMock
            .Setup(r => r.SaveChanges())
            .ReturnsAsync(0);

        // Act
        await _agendaService.Agendar(agendaDto);

        // Assert
        Assert.False(_notificadorMock.Object.TemNotificacao());

        Assert.NotNull(agendamentoSalvo);
        Assert.Equal(agendaDto.PacienteId, agendamentoSalvo.PacienteId);
        Assert.Equal(agendaDto.ProfissionalId, agendamentoSalvo.ProfissionalId);
        Assert.Equal(StatusAgendamento.Agendado, agendamentoSalvo.StatusAgendamento);
        Assert.Equal(agendaDto.DataInicio, agendamentoSalvo.DataInicio);
        Assert.Equal(agendaDto.DataFim, agendamentoSalvo.DataFim);
        Assert.Equal(agendaDto.Observacao, agendamentoSalvo.Observacao);

        _agendaRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Agenda>()),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
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

        _agendaRepositoryMock.Verify(
            r => r.ExisteConflitoHorario(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Never);

        _agendaRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Agenda>()),
            Times.Never);
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
        _agendaRepositoryMock.Verify(
            r => r.ExisteConflitoHorario(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Never);

        _agendaRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Agenda>()),
            Times.Never);
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

        _agendaRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Agenda>()),
            Times.Never);

        _profissionalRepositoryMock.Verify(
            r => r.ObterPorId(It.IsAny<Guid>()),
            Times.Never);
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

        _agendaRepositoryMock.Setup(r => r.ExisteConflitoHorario(agendaDto.ProfissionalId, agendaDto.DataInicio, agendaDto.DataFim))
            .ReturnsAsync(true);

        // Act
        await _agendaService.Agendar(agendaDto);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(notificacao =>
            notificacao.Mensagem == "O profissional já possui um agendamento neste horário.")),
            Times.Once);

        _agendaRepositoryMock.Verify(
            r => r.Adicionar(It.IsAny<Agenda>()),
            Times.Never);

        _agendaRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public void DatasValidas_DeveRetornarTrue_QuandoDataInicioForAnteriorADataFim()
    {
        // Arrange
        var agenda = new Agenda
        {
            DataInicio = new DateTime(2026, 10, 1, 10, 0, 0),
            DataFim = new DateTime(2026, 10, 1, 11, 0, 0)
        };

        // Act
        var resultado = agenda.DatasValidas();

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void DatasValidas_DeveRetornarFalse_QuandoDataInicioForMaiorQueDataFim()
    {
        // Arrange
        var agenda = new Agenda
        {
            DataInicio = new DateTime(2026, 10, 1, 11, 0, 0),
            DataFim = new DateTime(2026, 10, 1, 10, 0, 0)
        };

        // Act
        var resultado = agenda.DatasValidas();

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void DatasValidas_DeveRetornarFalse_QuandoDataInicioForIgualADataFim()
    {
        // Arrange
        var data = new DateTime(2026, 10, 1, 10, 0, 0);

        var agenda = new Agenda
        {
            DataInicio = data,
            DataFim = data
        };

        // Act
        var resultado = agenda.DatasValidas();

        // Assert
        Assert.False(resultado);
    }
}
