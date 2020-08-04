using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CovidOutApp.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CovidOutApp.Web.Repositories {
    public abstract class GenericRepository<T> : IGenericRepository<T>
    where T: class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> dbSet; 
        private readonly ILogger<GenericRepository<T>> _logger;
        public GenericRepository(ILogger<GenericRepository<T>> logger, ApplicationDbContext db){
           
            this._dbContext = db;
            this.dbSet = db.Set<T>();
            this._logger = logger;
        }
        public void Add(T entity)
        {   
            try
            {
                using (this._dbContext){
                    var item = this.dbSet.Add(entity);  
                    this._dbContext.SaveChanges();
                }            
            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error,ex.StackTrace);
                throw;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                using (this._dbContext){
                    var item = this.dbSet.Remove(entity);  
                    this._dbContext.SaveChanges();
                }            
            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error,ex.StackTrace);
                throw;
            }
        }

        public T Find(Guid id)
        {
            T result = null;

            try {
                   result = dbSet.Find(id);
            }
            catch(Exception ex) {
                _logger.Log(LogLevel.Error, ex.Message);
            }
          
            return result;
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new NullReferenceException("Predicate was null");

            try
            {
                var query = dbSet.AsQueryable();
                var items = query.Where(predicate);
                return items.ToList();
            }
            catch (System.Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.StackTrace);
                throw;
            }
        }   

        public void Update(T entity)
        {   
            try
            {
                  using (_dbContext){
                    this.dbSet.Attach(entity);
                    this._dbContext.Entry(entity).State = EntityState.Modified;

                    _dbContext.SaveChanges();
                }
            }
            catch (System.Exception ex)
            {
                _logger.Log(LogLevel.Error,ex.StackTrace);
                throw;
            }
        }
    }
}