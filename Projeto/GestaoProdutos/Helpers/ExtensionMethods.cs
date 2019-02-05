using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoProdutos.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Faz a paginação de um IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedResult<T> GetPaged<T>(
            this IQueryable<T> query, int page, int pageSize
        ) where T: class
        {
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query,
            Expression<Func<T, TKey>> exp, bool desc)
        {
            if (!desc)
            {
                return query.OrderBy(exp);
            } else
            {
                return query.OrderByDescending(exp);
            }
        }
    }
}
