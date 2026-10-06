using Agendamento.Infrastructure.Context;
using Agendamento.Infrastructure.Repository;
using Agendamento.Infrastructure.Tests.RepositoryTests.Auxiliar;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Tests.RepositoryTests;

public class PacienteRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly MeuDbContext _context;
    private readonly PacienteRepository _repository;

    public PacienteRepositoryTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<MeuDbContext>().UseSqlite(_connection).Options;
        _context = new MeuDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new PacienteRepository(_context);
    }

    [Fact]
    public async Task Buscar_DeveFiltrarPacientes()
    {
        var esperado = PacienteFixture.CriarPacienteFake(nome: "Ana", cpf: "111");
        var outro = PacienteFixture.CriarPacienteFake(nome: "Bruno", cpf: "222");
        _context.Pacientes.AddRange(esperado, outro);
        await _context.SaveChangesAsync();

        var resultado = await _repository.Buscar(p => p.CPF == "111");

        Assert.Equal(esperado.Id, Assert.Single(resultado).Id);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarPacienteComContatoEEndereco()
    {
        var paciente = PacienteFixture.CriarPacienteFake();
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPorId(paciente.Id);

        Assert.NotNull(resultado);
        Assert.Equal(paciente.Contato.Email, resultado.Contato.Email);
        Assert.Equal(paciente.Endereco.Cidade, resultado.Endereco.Cidade);
    }

    [Fact]
    public async Task ObterTodos_DeveIncluirContatoEEndereco()
    {
        _context.Pacientes.Add(PacienteFixture.CriarPacienteFake());
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterTodos();

        var paciente = Assert.Single(resultado);
        Assert.NotNull(paciente.Contato);
        Assert.NotNull(paciente.Endereco);
    }

    [Fact]
    public async Task ObterPacientesPorNome_DeveRetornarCorrespondenciasParciais()
    {
        var esperado = PacienteFixture.CriarPacienteFake(nome: "Ana Silva", cpf: "111");
        _context.Pacientes.AddRange(esperado, PacienteFixture.CriarPacienteFake(nome: "Bruno", cpf: "222"));
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPacientesPorNome("Ana");

        Assert.Equal(esperado.Id, Assert.Single(resultado).Id);
    }

    [Fact]
    public async Task ObterPorCPF_DeveRetornarPacienteOuNull()
    {
        var paciente = PacienteFixture.CriarPacienteFake(cpf: "123");
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        Assert.Equal(paciente.Id, (await _repository.ObterPorCPF("123"))?.Id);
        Assert.Null(await _repository.ObterPorCPF("inexistente"));
    }

    [Fact]
    public async Task ObterPorEmailEObterPorTelefone_DevemLocalizarPaciente()
    {
        var paciente = PacienteFixture.CriarPacienteFake();
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        Assert.Equal(paciente.Id, (await _repository.ObterPorEmail("teste@123.com"))?.Id);
        Assert.Equal(paciente.Id, (await _repository.ObterPorTelefone("11999999999"))?.Id);
        Assert.Null(await _repository.ObterPorEmail("ausente@teste.com"));
        Assert.Null(await _repository.ObterPorTelefone("000"));
    }

    [Fact]
    public async Task OperacoesDeEscrita_DevemPersistirApenasAposSaveChanges()
    {
        var paciente = PacienteFixture.CriarPacienteFake(cpf: "111");
        await _repository.Adicionar(paciente);
        Assert.Equal(3, await _repository.SaveChanges());

        paciente.Nome = "Atualizado";
        await _repository.Atualizar(paciente);
        await _repository.SaveChanges();
        Assert.Equal("Atualizado", (await _repository.ObterPorId(paciente.Id))?.Nome);

        await _repository.Remover(paciente.Id);
        await _repository.SaveChanges();
        Assert.Null(await _repository.ObterPorId(paciente.Id));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
