using GestaoProdutos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{
    public interface IProdutosService: IGenericCrud<Produto>
    {
        IQueryable<Produto> Filter(string nome, string ordem, bool desc);
    }
}
