using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Agendamento.Infrastructure.Tests.RepositoryTests.Auxiliar;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Tests.ContextTests;

public class MeuDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly MeuDbContext _context;

    public MeuDbContextTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<MeuDbContext>().UseSqlite(_connection).Options;
        _context = new MeuDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void Construtor_DeveConfigurarConsultasSemTrackingEDbSets()
    {
        Assert.Equal(QueryTrackingBehavior.NoTracking, _context.ChangeTracker.QueryTrackingBehavior);
        Assert.False(_context.ChangeTracker.AutoDetectChangesEnabled);
        Assert.NotNull(_context.Agendas);
        Assert.NotNull(_context.Pacientes);
        Assert.NotNull(_context.Profissionais);
    }

    [Fact]
    public void OnModelCreating_DeveAplicarChavesTiposERelacionamentos()
    {
        var paciente = _context.Model.FindEntityType(typeof(Paciente))!;
        var profissional = _context.Model.FindEntityType(typeof(Profissional))!;
        var agenda = _context.Model.FindEntityType(typeof(Agenda))!;

        Assert.Equal(nameof(Paciente.Id), paciente.FindPrimaryKey()!.Properties.Single().Name);
        Assert.Equal("varchar(200)", paciente.FindProperty(nameof(Paciente.Nome))!.GetColumnType());
        Assert.Equal("date", paciente.FindProperty(nameof(Paciente.DataNascimento))!.GetColumnType());
        Assert.Equal("time", profissional.FindProperty(nameof(Profissional.HoraInicio))!.GetColumnType());
        Assert.Equal("varchar(50)", agenda.FindProperty(nameof(Agenda.StatusAgendamento))!.GetColumnType());

        Assert.All(agenda.GetForeignKeys(), fk =>
            Assert.Equal(DeleteBehavior.ClientSetNull, fk.DeleteBehavior));
    }

    [Fact]
    public async Task SaveChangesAsync_DeveDefinirDataCadastroAoAdicionarEntidades()
    {
        var paciente = PacienteFixture.CriarPacienteFake();
        _context.Pacientes.Add(paciente);
        var antesDeSalvar = DateTime.Now;

        await _context.SaveChangesAsync();

        var pacienteSalvo = await _context.Pacientes.AsNoTracking()
            .SingleAsync(p => p.Id == paciente.Id);
        Assert.InRange(pacienteSalvo.DataCadastro, antesDeSalvar, DateTime.Now);
    }

    [Fact]
    public async Task SaveChangesAsync_NaoDeveAlterarDataCadastroAoAtualizarEntidade()
    {
        var paciente = PacienteFixture.CriarPacienteFake();
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();
        var dataCadastroOriginal = paciente.DataCadastro;

        _context.Entry(paciente).State = EntityState.Detached;
        paciente.Nome = "Nome atualizado";
        _context.Pacientes.Update(paciente);
        _context.Entry(paciente).Property(p => p.DataCadastro).CurrentValue = DateTime.MinValue;

        await _context.SaveChangesAsync();

        var pacienteAtualizado = await _context.Pacientes.AsNoTracking()
            .SingleAsync(p => p.Id == paciente.Id);
        Assert.Equal("Nome atualizado", pacienteAtualizado.Nome);
        Assert.Equal(dataCadastroOriginal, pacienteAtualizado.DataCadastro);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
