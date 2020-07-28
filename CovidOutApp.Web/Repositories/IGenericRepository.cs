using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CovidOutApp.Web.Repositories {
    public interface IGenericRepository<TEntity> 
        where TEntity: class {
        public TEntity Find(Guid id);
        public IEnumerable<TEntity> GetAll();
        
        public IEnumerable<TEntity> Query (Expression<Func<TEntity, bool>> predicate); 

        public void Add(TEntity entity);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);

    }
}