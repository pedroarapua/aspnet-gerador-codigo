using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class SolucaoVO
    {
        public String Nome { get; set; }
        public Guid Guid { get; set; }
        public List<ProjetoVO> Projetos { get; set; }

        public SolucaoVO()
        {
            this.Guid = Guid.NewGuid();
            this.Projetos = new List<ProjetoVO>();
        }
    }
}
