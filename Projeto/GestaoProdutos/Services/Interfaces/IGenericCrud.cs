using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{
    public interface IGenericCrud<T> where T:class
    {
        IQueryable<T> Query();

        IQueryable<T> Query(Expression<Func<T, bool>> query);

        T Find(params object[] key);

        T Add(T entity);

        void Add(ICollection<T> entities);

        T Update(T entity, params object[] key);

        T Delete(params object[] key);

        void Delete(Expression<Func<T, bool>> query);

        int Count(Expression<Func<T, bool>> query);

        bool Exist(params object[] key);

        bool Exist(Func<T, bool> query);
    }
}
