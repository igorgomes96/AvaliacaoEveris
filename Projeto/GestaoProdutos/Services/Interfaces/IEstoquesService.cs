using GestaoProdutos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{
    public interface IEstoqueService: IGenericCrud<Estoque>
    {
        IQueryable<Estoque> Filter(DateTime? data, string produto, string empresa, string ordem, bool desc);
        void ImportFile(string path);
    }
}
