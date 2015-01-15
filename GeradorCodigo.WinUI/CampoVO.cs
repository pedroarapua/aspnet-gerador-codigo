using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controls.Components;

namespace GeradorCodigo.WinUI
{
    public class CampoVO
    {
        #region     .....:::::     CONSTRUTORES     :::::.....

        public CampoVO() { }

        public CampoVO(PropriedadeVO prop)
        {
            this.Propriedade = prop;
            this.TipoCampo = this.GetTypeTextField();
        }

        #endregion

        #region     .....:::::     PROPRIEDADES     :::::.....

        public PropriedadeVO Propriedade {get;set;}
        public Boolean Gerar{get;set;}
        public Boolean Obrigatorio{get;set;}
        public Type TipoComponent { get; set; }
        public WTextBox.ETypeMaskTextField TipoCampo{get;set;}
        public String Mascara{get;set;}
        public String Caption { get; set; }
        public String DisplayField { get; set; }
        public String ValueField { get; set; }
        
        #endregion
    }
}
