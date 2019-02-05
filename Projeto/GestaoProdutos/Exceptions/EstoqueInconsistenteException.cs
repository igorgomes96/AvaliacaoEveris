using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Exceptions
{
    public class EstoqueInconsistenteException: Exception
    {
        public EstoqueInconsistenteException() : base() { }
        public EstoqueInconsistenteException(string message) : base(message) { }
    }
}
