using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class ClasseVO
    {
        #region     .....:::::     PROPRIEDADES     :::::.....

        public String Nome { get; set; }
        public List<PropriedadeVO> Propriedades {get; set; }
        public TabelaVO Tabela { get; set; }

        #endregion

        #region     .....:::::     CONSTRUTORES     :::::.....

        public ClasseVO()
        {
            this.Propriedades = new List<PropriedadeVO>();
        }

        #endregion
    }
}
