using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Model
{
    public class Estoque
    {
        public int Id { get; set; }
        [Required]
        public int ProdutoId { get; set; }
        [Required]
        public int EmpresaId { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public int Entrada { get; set; }
        [Required]
        public int Saida { get; set; }
        [Required]
        public int Qtda { get; set; }

        public virtual Produto Produto { get; set; }
        public virtual Empresa Empresa { get; set; }

    }
}
