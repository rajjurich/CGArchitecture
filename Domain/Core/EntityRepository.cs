using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IEntityRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task<int> Count(Expression<Func<T, bool>> predicate);
        Task<T> Edit(T entity);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> Get();
        Task<T> Get(int key);
        Task<T> Delete(int key);
        Task Save();
    }
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        EntitiesContext entitiesContext;
        public EntityRepository()
        {
            this.entitiesContext = new EntitiesContext();
        }
        public async Task<T> Add(T entity)
        {
            DbEntityEntry dbEntityEntry = entitiesContext.Entry<T>(entity);
            entitiesContext.Set<T>().Add(entity);
            await Save();
            return entity;
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {            
            return await entitiesContext.Set<T>().Where(predicate).CountAsync();
        }

        public async Task<T> Delete(int key)
        {
            T entity = await Get(key);
            DbEntityEntry dbEntityEntry = entitiesContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
            await Save();
            return entity;
        }

        public async Task<T> Edit(T entity)
        {
            DbEntityEntry dbEntityEntry = entitiesContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
            await Save();
            return entity;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {            
            return entitiesContext.Set<T>().Where(predicate);
        }

        public IQueryable<T> Get()
        {           
            return entitiesContext.Set<T>();
        }

        public async Task<T> Get(int key)
        {
            return await entitiesContext.Set<T>().FindAsync(key);
        }

        public async Task Save()
        {
            await entitiesContext.SaveChangesAsync();
        }
    }
}
