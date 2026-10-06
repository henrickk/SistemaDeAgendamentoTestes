using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Agendamento.Infrastructure.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Tests.RepositoryTests;

public class ProfissionalRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly MeuDbContext _context;
    private readonly ProfissionalRepository _repository;

    public ProfissionalRepositoryTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<MeuDbContext>().UseSqlite(_connection).Options;
        _context = new MeuDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new ProfissionalRepository(_context);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarProfissionalComContatoEEndereco()
    {
        var profissional = CriarProfissional("Ana", "CRO1");
        _context.Profissionais.Add(profissional);
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPorId(profissional.Id);

        Assert.NotNull(resultado);
        Assert.Equal("ana@teste.com", resultado.Contato.Email);
        Assert.Equal("São Paulo", resultado.Endereco.Cidade);
        Assert.Null(await _repository.ObterPorId(Guid.NewGuid()));
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarProfissionaisComRelacionamentos()
    {
        _context.Profissionais.AddRange(CriarProfissional("Ana", "CRO1"), CriarProfissional("Bruno", "CRO2"));
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterTodos();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, profissional =>
        {
            Assert.NotNull(profissional.Contato);
            Assert.NotNull(profissional.Endereco);
        });
    }

    [Fact]
    public async Task ObterPorCRO_DeveLocalizarProfissional()
    {
        var profissional = CriarProfissional("Ana", "CRO1");
        _context.Profissionais.Add(profissional);
        await _context.SaveChangesAsync();

        Assert.Equal(profissional.Id, (await _repository.ObterPorCRO("CRO1"))?.Id);
        Assert.Null(await _repository.ObterPorCRO("CRO inexistente"));
    }

    [Fact]
    public async Task ObterPorNome_DeveRetornarCorrespondenciasExatas()
    {
        var profissional = CriarProfissional("Ana", "CRO1");
        _context.Profissionais.AddRange(profissional, CriarProfissional("Ana Silva", "CRO2"));
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPorNome("Ana");

        Assert.Equal(profissional.Id, Assert.Single(resultado).Id);
    }

    private static Profissional CriarProfissional(string nome, string cro) => new(
        nome, cro, Guid.NewGuid().ToString("N"), new TimeOnly(8, 0), new TimeOnly(17, 0),
        new Contato($"{nome.ToLowerInvariant()}@teste.com", "11999999999"),
        new Endereco("Rua A", "1", "Centro", "São Paulo", "SP", "", "00000000"));

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
