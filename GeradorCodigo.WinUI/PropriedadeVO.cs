using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class PropriedadeVO
    {
        public String Nome { get; set; }
        public ETipoVeriavel Tipo { get; set; }
        public Boolean Nullable { get; set; }
        public ColunaVO Coluna { get; set; }
    }
}
