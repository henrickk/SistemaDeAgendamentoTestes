using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Tests.RepositoryTests.Auxiliar;
using Agendamento.Infrastructure.Context;
using Agendamento.Infrastructure.Repository;
using Agendamento.Infrastructure.Tests.RepositoryTests.Auxiliar;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Tests.RepositoryTests;

public class AgendaRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly MeuDbContext _context;
    private readonly AgendaRepository _repository;

    public AgendaRepositoryTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<MeuDbContext>().UseSqlite(_connection).Options;
        _context = new MeuDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new AgendaRepository(_context);
    }

    [Fact]
    public async Task ExisteConflitoHorario_DeveDetectarSobreposicaoDoMesmoProfissional()
    {
        var inicio = new DateTime(2026, 10, 5, 10, 0, 0);
        var profissional = ProfissionalFixture.CriarProfissionalFake();
        var paciente = PacienteFixture.CriarPacienteFake();
        _context.Agendas.Add(CriarAgenda(paciente, profissional, inicio, inicio.AddHours(1)));
        await _context.SaveChangesAsync();

        Assert.True(await _repository.ExisteConflitoHorario(profissional.Id, inicio.AddMinutes(30), inicio.AddHours(2)));
        Assert.False(await _repository.ExisteConflitoHorario(profissional.Id, inicio.AddHours(1), inicio.AddHours(2)));
        Assert.False(await _repository.ExisteConflitoHorario(Guid.NewGuid(), inicio, inicio.AddHours(1)));
    }

    [Fact]
    public async Task ObterPorIdComRelacionamentos_DeveRetornarPacienteEProfissional()
    {
        var agenda = await PersistirAgenda();

        var resultado = await _repository.ObterPorIdComRelacionamentos(agenda.Id);

        Assert.NotNull(resultado);
        Assert.Equal(agenda.PacienteId, resultado.Paciente.Id);
        Assert.Equal(agenda.ProfissionalId, resultado.Profissional.Id);
        Assert.Null(await _repository.ObterPorIdComRelacionamentos(Guid.NewGuid()));
    }

    [Fact]
    public async Task ObterTodosComRelacionamentos_DeveRetornarEntidadesRelacionadas()
    {
        await PersistirAgenda();

        var resultado = (await _repository.ObterTodosComRelacionamentos()).ToList();

        var agenda = Assert.Single(resultado);
        Assert.NotNull(agenda.Paciente);
        Assert.NotNull(agenda.Paciente.Contato);
        Assert.NotNull(agenda.Profissional);
    }

    [Fact]
    public async Task ObterPorIdEObterTodos_DeveRetornarAgendasComRelacionamentos()
    {
        var agenda = await PersistirAgenda();

        var porId = await _repository.ObterPorId(agenda.Id);
        var todos = await _repository.ObterTodos();

        Assert.NotNull(porId?.Paciente);
        Assert.NotNull(porId?.Profissional);
        Assert.NotNull(Assert.Single(todos).Paciente);
    }

    private async Task<Agenda> PersistirAgenda()
    {
        var paciente = PacienteFixture.CriarPacienteFake();
        var profissional = ProfissionalFixture.CriarProfissionalFake();
        var inicio = new DateTime(2026, 10, 5, 10, 0, 0);
        var agenda = CriarAgenda(paciente, profissional, inicio, inicio.AddHours(1));
        _context.Agendas.Add(agenda);
        await _context.SaveChangesAsync();
        return agenda;
    }

    private static Agenda CriarAgenda(Paciente paciente, Profissional profissional, DateTime inicio, DateTime fim) =>
        new(paciente.Id, profissional.Id, StatusAgendamento.Agendado, inicio, fim, "Consulta", paciente, profissional);

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
