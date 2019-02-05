using GestaoProdutos.Helpers;
using GestaoProdutos.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{
    public class EmpresasService : GenericCrud<Empresa>, IEmpresasService
    {
        private readonly DbContext _db;
        public EmpresasService(DbContext db) : base(db)
        {
            _db = db;
        }

        public IQueryable<Empresa> Filter(string nome, string ordem = null, bool desc = false)
        {
            IQueryable<Empresa> query;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(nome)) { 
                query = Query();
            }
            else
            {
                query = Query(e => e.Nome.StartsWith(nome, StringComparison.CurrentCultureIgnoreCase))
                    .OrderBy(e => e.Nome)
                    .Take(10);
            }

            switch (ordem?.ToLower()?.Trim())
            {
                case "nome":
                    query = query.OrderBy(e => e.Nome, desc);
                    break;
                default:
                    query = query.OrderBy(e => e.Nome, desc);
                    break;
            }

            return query;
        }
    }
}
