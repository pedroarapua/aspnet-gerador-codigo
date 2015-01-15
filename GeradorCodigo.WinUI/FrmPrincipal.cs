using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeradorCodigo.WinUI
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void gerarProjetosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmGeradorProjetos formChild = new FrmGeradorProjetos();
            formChild.MdiParent = this;
            OpenForm(formChild);
        }

        private void OpenForm(Form form)
        {
            //int TopMargin = 0;
            //int LeftMargin = 0;
            //LeftMargin = ((this.Width - form.Width) / 2) + this.Left;
            //TopMargin = ((this.Height - form.Height) / 2) + this.Top - 50;
            //form.StartPosition = FormStartPosition.Manual;
            //form.Top = TopMargin;
            //form.Left = LeftMargin;

            form.Show();
        }

        private void menuGerarInterfaces_Click(object sender, EventArgs e)
        {
            FrmGerarInterfaces formChild = new FrmGerarInterfaces();
            formChild.MdiParent = this;
            OpenForm(formChild);
        }
    }
}
