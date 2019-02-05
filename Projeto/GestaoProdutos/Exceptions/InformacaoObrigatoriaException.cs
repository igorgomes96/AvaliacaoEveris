using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Exceptions
{
    public class InformacaoObrigatoriaException: Exception
    {
        public InformacaoObrigatoriaException(): base() { }
        public InformacaoObrigatoriaException(string message) : base(message) { }
    }
}
