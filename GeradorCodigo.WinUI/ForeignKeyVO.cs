using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class ForeignKeyVO
    {
        public String Nome { get; set; }
        public String NomeColuna { get; set; }
        public TabelaVO Tabela { get; set; }
        public ClasseVO Classe { get; set; }
    }
}
