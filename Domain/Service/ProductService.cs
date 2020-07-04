using Domain.Core;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public interface IProductService
    {
        Task<Product> Add(Product entity);
        Task<int> Count();
        Task<Product> Delete(int key);
        Task<Product> Edit(Product entity);
        IQueryable<Product> Get();
        Task<Product> Get(int key);
        IQueryable<Product> FindBy(Expression<Func<Product, bool>> predicate);
    }
    public class ProductService : IProductService
    {
        IEntityRepository<Product> entityRepository;
        public ProductService()
        {
            this.entityRepository = (IEntityRepository<Product>)new EntityRepository<Product>();
        }

        public async Task<Product> Add(Product entity)
        {
            entity.CreatedOn = DateTime.Now;
            var added = await entityRepository.Add(entity);
            return added;
        }

        public async Task<int> Count()
        {
            return await entityRepository.Count(x => x.DeletedOn == null);
        }

        public async Task<Product> Delete(int key)
        {
            var entity = await entityRepository.Get(key);
            entity.DeletedBy = 1;
            entity.DeletedOn = DateTime.Now;
            return await entityRepository.Edit(entity);
        }

        public async Task<Product> Edit(Product entity)
        {
            entity.UpdatedOn = DateTime.Now;
            return await entityRepository.Edit(entity);
        }

        public IQueryable<Product> FindBy(Expression<Func<Product, bool>> predicate)
        {            
            return entityRepository.FindBy(predicate);
        }

        public IQueryable<Product> Get()
        {           
            return entityRepository.FindBy(x => x.DeletedOn == null);
        }

        public async Task<Product> Get(int key)
        {
            return await entityRepository.Get(key);
        }
    }
}
