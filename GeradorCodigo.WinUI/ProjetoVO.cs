using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class ProjetoVO
    {
        public String Nome { get; set; }
        public String Namespace { get; set; }
        public ETipoProjeto Tipo { get; set; }
        public Guid Guid { get; set; }
        public List<ClasseVO> Classes { get; set; }
        public List<InterfaceVO> Interfaces { get; set; }

        public ProjetoVO()
        {
            this.Guid = Guid.NewGuid();
            this.Classes = new List<ClasseVO>();
            this.Interfaces = new List<InterfaceVO>();
        }
    }
}
