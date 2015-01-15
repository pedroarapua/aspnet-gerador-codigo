using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class ColunaVO
    {
        public String Nome { get; set; }
        public ETipoColuna TipoColuna { get; set; }
        public Int64? Tamanho { get; set; }
        public Int32? Scala { get; set; }
        public Boolean IsChavePrimaria { get; set; }
        public Boolean IsNullable { get; set; }
        public Boolean IsChaveEstrangeira { get; set; }
        public ForeignKeyVO ChaveEstrangeira { get; set; }
        
    }
}
