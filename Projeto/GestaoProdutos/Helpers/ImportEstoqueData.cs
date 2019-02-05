using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Helpers
{
    public class ImportEstoqueData
    {
        public object Empresa { get; set; }
        public object CodigoProduto { get; set; }
        public object NomeProduto { get; set; }
        public object Entrada { get; set; }
        public object Saida { get; set; }
        public object Estoque { get; set; }
        public DateTime Data { get; set; }
        public int Row { get; set; }
    }
}
