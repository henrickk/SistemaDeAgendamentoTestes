using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Agendamento.Infrastructure.Repository;
using Agendamento.Infrastructure.Tests.RepositoryTests.Auxiliar;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Tests.RepositoryTests.GenericRepository;

public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly MeuDbContext _context;
    private readonly Repository<Paciente> _repository;

    public RepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<MeuDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new MeuDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new Repository<Paciente>(_context);
    }

    [Fact]
    public async Task Adicionar_DevePersistirEntidade()
    {
        // Arrange
        var paciente = PacienteFixture.CriarPacienteFake();

        // Act
        await _repository.Adicionar(paciente);

        // Assert
        var pacienteSalvo = await _context.Set<Paciente>()
            .FindAsync(paciente.Id);

        Assert.NotNull(pacienteSalvo);
        Assert.Equal(paciente.Id, pacienteSalvo.Id);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarEntidade_QuandoExistir()
    {
        // Arrange
        var paciente = PacienteFixture.CriarPacienteFake();

        _context.Set<Paciente>().Add(paciente);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObterPorId(paciente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(paciente.Id, resultado.Id);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNull_QuandoEntidadeNaoExistir()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var resultado = await _repository.ObterPorId(idInexistente);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarTodasAsEntidades()
    {
        // Arrange
        var paciente1 = PacienteFixture.CriarPacienteFake();
        var paciente2 = PacienteFixture.CriarPacienteFake();

        _context.Set<Paciente>().AddRange(paciente1, paciente2);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObterTodos();

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Contains(resultado, p => p.Id == paciente1.Id);
        Assert.Contains(resultado, p => p.Id == paciente2.Id);
    }

    [Fact]
    public async Task Buscar_DeveRetornarSomenteEntidadesQueAtendemAoFiltro()
    {
        // Arrange
        var paciente1 = PacienteFixture.CriarPacienteFake();
        var paciente2 = PacienteFixture.CriarPacienteFake();

        paciente1.CPF = "11111111111";
        paciente2.CPF = "22222222222";

        _context.Set<Paciente>().AddRange(paciente1, paciente2);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.Buscar(
            p => p.CPF == "11111111111");

        // Assert
        var pacientes = resultado.ToList();

        Assert.Single(pacientes);
        Assert.Equal(paciente1.Id, pacientes[0].Id);
    }

    [Fact]
    public async Task Atualizar_DevePersistirAlteracoes()
    {
        // Arrange
        var paciente = PacienteFixture.CriarPacienteFake();

        _context.Set<Paciente>().Add(paciente);
        await _context.SaveChangesAsync();

        _context.Entry(paciente).State = EntityState.Detached;

        paciente.Nome = "Nome Atualizado";

        // Act
        await _repository.Atualizar(paciente);

        // Assert
        var pacienteAtualizado = await _context.Set<Paciente>()
            .FindAsync(paciente.Id);

        Assert.NotNull(pacienteAtualizado);
        Assert.Equal("Nome Atualizado", pacienteAtualizado.Nome);
    }

    [Fact]
    public async Task Remover_DeveExcluirEntidade_QuandoExistir()
    {
        // Arrange
        var paciente = PacienteFixture.CriarPacienteFake();

        _context.Set<Paciente>().Add(paciente);
        await _context.SaveChangesAsync();

        // Act
        await _repository.Remover(paciente.Id);

        // Assert
        var pacienteRemovido = await _context.Set<Paciente>()
            .FindAsync(paciente.Id);

        Assert.Null(pacienteRemovido);
    }

    [Fact]
    public async Task Remover_NaoDeveLancarExcecao_QuandoEntidadeNaoExistir()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var excecao = await Record.ExceptionAsync(
            () => _repository.Remover(idInexistente));

        // Assert
        Assert.Null(excecao);
    }

    [Fact]
    public async Task SaveChanges_DevePersistirAlteracoes()
    {
        // Arrange
        var paciente = PacienteFixture.CriarPacienteFake();

        _context.Set<Paciente>().Add(paciente);

        // Act
        var resultado = await _repository.SaveChanges();

        // Assert
        // Paciente, Contato e Endereco são persistidos como entradas distintas pelo EF Core.
        Assert.Equal(3, resultado);

        var pacienteSalvo = await _context.Set<Paciente>()
            .FindAsync(paciente.Id);

        Assert.NotNull(pacienteSalvo);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
