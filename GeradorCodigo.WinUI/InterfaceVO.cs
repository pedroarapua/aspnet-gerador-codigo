using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeradorCodigo.WinUI
{
    public class InterfaceVO
    {
        #region     .....:::::     CONSTRUTORES     :::::.....

        public InterfaceVO()
        {
            this.CamposPesquisa = new List<CampoVO>();
            this.CamposGrid = new List<CampoVO>();
            this.CamposForm = new List<CampoVO>();
        }

        public InterfaceVO(ClasseVO classe)
            : this()
        {
            this.Classe = classe;
            this.Classe.Propriedades.ForEach(x => this.CamposPesquisa.Add(new CampoVO(x)));
            this.Classe.Propriedades.ForEach(x => this.CamposGrid.Add(new CampoVO(x)));
            this.Classe.Propriedades.ForEach(x => this.CamposForm.Add(new CampoVO(x)));
        }

        #endregion

        #region     .....:::::     PROPRIEDADES     :::::.....

        public ClasseVO Classe{ get; set; }
        public Boolean Gerar{ get; set; }
        public String Titulo{ get; set; }
        public List<CampoVO> CamposPesquisa { get; set; }
        public List<CampoVO> CamposGrid { get; set; }
        public List<CampoVO> CamposForm { get; set; }
        
        #endregion
    }
}
