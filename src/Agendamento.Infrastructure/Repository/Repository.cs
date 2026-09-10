using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Agendamento.Infrastructure.Repository;
public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
{
    protected readonly MeuDbContext Db;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(MeuDbContext db)
    {
        Db = db;
        DbSet = db.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }
    public virtual async Task<TEntity> ObterPorId(Guid id)
    {
        return await DbSet.FindAsync(id);
    }
    public virtual async Task<List<TEntity>> ObterTodos()
    {
        return await DbSet.ToListAsync();
    }
    public virtual async Task Adicionar(TEntity entity)
    {
        DbSet.Add(entity);
        await SaveChanges();
    }
    public virtual async Task Atualizar(TEntity entity)
    {
        DbSet.Update(entity);
        await SaveChanges();
    }
    public virtual async Task Remover(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await SaveChanges();
        }
    }
    public async Task<int> SaveChanges()
    {
        return await Db.SaveChangesAsync();
    }

    public void Dispose()
    {
        Db.Dispose();
    }
}
