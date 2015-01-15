using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class TabelaVO
    {
        public String Nome { get; set; }
        public List<ColunaVO> Colunas { get; set; }

        public TabelaVO()
        {
            this.Colunas = new List<ColunaVO>();
        }
    }
}
