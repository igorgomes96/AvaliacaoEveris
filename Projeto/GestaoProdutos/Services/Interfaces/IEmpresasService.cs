using GestaoProdutos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{
    public interface IEmpresasService: IGenericCrud<Empresa>
    {
        IQueryable<Empresa> Filter(string nome, string ordem, bool desc);
    }
}
