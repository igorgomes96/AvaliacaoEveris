using GestaoProdutos.Exceptions;
using GestaoProdutos.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{
    public class GenericCrud<T>: IGenericCrud<T> where T : class
    {
        private readonly DbContext _db;

        public GenericCrud(DbContext db)
        {
            _db = db;
        }

        public virtual IQueryable<T> Query() => _db.Set<T>();

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> query) => _db.Set<T>().Where(query);

        public virtual T Find(params object[] key) => _db.Set<T>().Find(key);

        public virtual T Add(T entity)
        {
            T newEntity = _db.Set<T>().Add(entity).Entity;
            _db.SaveChanges();
            return newEntity;
        }

        public virtual void Add(ICollection<T> entities)
        {
            _db.Set<T>().AddRange(entities);
            _db.SaveChanges();
        }

        public virtual T Update(T entity, params object[] key)
        {
            T currentEntity = Find(key);
            if (currentEntity == null) throw new NotFoundException();
            _db.Entry(currentEntity).CurrentValues.SetValues(entity);
            _db.SaveChanges();
            return currentEntity;
        }

        public virtual T Delete(params object[] key)
        {
            T entity = Find(key);
            if (entity == null) throw new NotFoundException();
            entity = _db.Set<T>().Remove(entity).Entity;
            _db.SaveChanges();
            return entity;
        }

        public virtual void Delete(Expression<Func<T, bool>> query)
        {
            _db.RemoveRange(Query(query));
            _db.SaveChanges();
        }

        public virtual int Count(Expression<Func<T, bool>> query) => Query(query).Count();

        public virtual bool Exist(params object[] key) => Find(key) != null;

        public virtual bool Exist(Func<T, bool> query) => _db.Set<T>().Any(query);
    }
}
