using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Web.Controls.Components;

namespace GeradorCodigo.WinUI
{
    public partial class FrmGerarInterfaces : Form
    {
        public Int32 Index { get; set; }

        private List<InterfaceVO> interfaces;

        public FrmGerarInterfaces()
        {
            InitializeComponent();
        }

        public FrmGerarInterfaces(List<InterfaceVO> _interfaces) : this()
        {
            this.interfaces = _interfaces;
            SetVisibleButtons();
            LoadComponents();
        }

        #region     .....:::::     EVENTOS     :::::.....

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            Index--;
            LimparForm();
            LoadComponents();
            SetVisibleButtons();   
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            this.interfaces[Index].Gerar = true;
            this.interfaces[Index].Titulo = txtTitulo.Text;
            SalvarInformacoes();
            Index++;
            if (Index == interfaces.Count)
            {
                this.Close();
            }
            else
            {
                LimparForm();
                LoadComponents();
                SetVisibleButtons();
            }
        }

        private void btnNaoUtilizar_Click(object sender, EventArgs e)
        {
            this.interfaces[Index].Gerar = false;

            Index++;
            if (Index == interfaces.Count)
            {
                this.Close();
            }
            else
            {
                LimparForm();
                LoadComponents();
                SetVisibleButtons();
            }
        }

        private void grvCamposGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                #region     .....:::::     CARREGA MASCARAS     :::::.....

                DataGridViewCell cell = grvCamposGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                String valor = cell.Value.ToString();
                cell.Value = String.Empty;
                DataGridViewComboBoxCell columnMascara = (DataGridViewComboBoxCell)cell;
                columnMascara.Items.Clear();
                columnMascara.Items.Add("Selecione...");

                switch (interfaces[Index].CamposGrid[e.RowIndex].TipoCampo)
                {
                    case WTextBox.ETypeMaskTextField.Money:
                        columnMascara.Items.Add("100,00");
                        columnMascara.Items.Add("100,0000");
                        break;
                    case WTextBox.ETypeMaskTextField.Date:
                        columnMascara.Items.Add("31/01/2001");
                        columnMascara.Items.Add("2001/01/31");
                        break;
                }

                columnMascara.Value = valor;

                #endregion
            }
        }

        private void grvForm_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                #region     .....:::::     CARREGA COLUNAS     :::::.....

                DataGridViewCell cell = grvForm.Rows[e.RowIndex].Cells[e.ColumnIndex];
                String valor = cell.Value.ToString();
                cell.Value = String.Empty;
                DataGridViewComboBoxCell columnValor = (DataGridViewComboBoxCell)cell;
                columnValor.Items.Clear();
                columnValor.Items.Add("Selecione...");

                if (interfaces[Index].CamposForm[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira != null)
                {
                    //foreach (PropriedadeVO prop in interfaces[Index].CamposForm[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades.Where(x => !x.Coluna.IsNullable))
                    foreach (PropriedadeVO prop in interfaces[Index].CamposForm[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades)
                    {
                        columnValor.Items.Add(prop.Coluna.Nome);
                    }

                }
                else
                {
                    valor = "Selecione...";
                }

                columnValor.Value = valor;

                #endregion
            }
            else if (e.ColumnIndex == 4)
            {
                #region     .....:::::     CARREGA COLUNAS     :::::.....

                DataGridViewCell cell = grvForm.Rows[e.RowIndex].Cells[e.ColumnIndex];
                String valor = cell.Value.ToString();
                cell.Value = String.Empty;
                DataGridViewComboBoxCell columnMostrado = (DataGridViewComboBoxCell)cell;
                columnMostrado.Items.Clear();
                columnMostrado.Items.Add("Selecione...");

                if (interfaces[Index].CamposForm[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira != null)
                {
                    //foreach (PropriedadeVO prop in interfaces[Index].CamposForm[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades.Where(x => !x.Coluna.IsNullable))
                    foreach (PropriedadeVO prop in interfaces[Index].CamposForm[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades)
                    {
                        columnMostrado.Items.Add(prop.Coluna.Nome);
                    }

                }
                else
                {
                    valor = "Selecione...";
                }

                columnMostrado.Value = valor;

                #endregion
            }
        }

        private void grvPesquisa_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                #region     .....:::::     CARREGA COLUNAS     :::::.....

                DataGridViewCell cell = grvPesquisa.Rows[e.RowIndex].Cells[e.ColumnIndex];
                String valor = cell.Value.ToString();
                cell.Value = String.Empty;
                DataGridViewComboBoxCell columnValor = (DataGridViewComboBoxCell)cell;
                columnValor.Items.Clear();
                columnValor.Items.Add("Selecione...");

                if (interfaces[Index].CamposPesquisa[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira != null)
                {
                    //foreach (PropriedadeVO prop in interfaces[Index].CamposPesquisa[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades.Where(x => !x.Coluna.IsNullable))
                    foreach (PropriedadeVO prop in interfaces[Index].CamposPesquisa[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades)
                    {
                        columnValor.Items.Add(prop.Coluna.Nome);
                    }

                }
                else
                {
                    valor = "Selecione...";
                }

                columnValor.Value = valor;

                #endregion
            }
            else if (e.ColumnIndex == 4)
            {
                #region     .....:::::     CARREGA COLUNAS     :::::.....

                DataGridViewCell cell = grvPesquisa.Rows[e.RowIndex].Cells[e.ColumnIndex];
                String valor = cell.Value.ToString();
                cell.Value = String.Empty;
                DataGridViewComboBoxCell columnMostrado = (DataGridViewComboBoxCell)cell;
                columnMostrado.Items.Clear();
                columnMostrado.Items.Add("Selecione...");

                if (interfaces[Index].CamposPesquisa[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira != null)
                {
                    //foreach (PropriedadeVO prop in interfaces[Index].CamposPesquisa[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades.Where(x => !x.Coluna.IsNullable))
                    foreach (PropriedadeVO prop in interfaces[Index].CamposPesquisa[e.RowIndex].Propriedade.Coluna.ChaveEstrangeira.Classe.Propriedades)
                    {
                        columnMostrado.Items.Add(prop.Coluna.Nome);
                    }

                }
                else
                {
                    valor = "Selecione...";
                }

                columnMostrado.Value = valor;

                #endregion
            }
        }

        #endregion

        #region     .....:::::     METODOS     :::::.....

        private void SetVisibleButtons()
        {
            if((Index + 1) == this.interfaces.Count)
                btnProximo.Text = "Concluir";
            btnVoltar.Visible = Index != 0;
        }

        private void LimparForm()
        {
            this.txtTitulo.Text = String.Empty;
            
            while (this.grvPesquisa.Rows.Count > 0)
                this.grvPesquisa.Rows.RemoveAt(0);

            while (this.grvCamposGrid.Rows.Count > 0)
                this.grvCamposGrid.Rows.RemoveAt(0);

            while (this.grvForm.Rows.Count > 0)
                this.grvForm.Rows.RemoveAt(0);
        }

        private void LoadComponents()
        {
            #region     .....:::::     CARREGA TIPOS DE COMPONENTE     :::::.....

            DataGridViewComboBoxColumn columnTipoComponent = grvPesquisa.Columns["columnTipoComponent"] as DataGridViewComboBoxColumn;
            DataGridViewComboBoxColumn columnTipoComponentForm = grvForm.Columns["columnTipoComponentForm"] as DataGridViewComboBoxColumn;

            columnTipoComponent.Items.Add("TextBox");
            columnTipoComponent.Items.Add("DropDownList");
            columnTipoComponent.Items.Add("RadioButtonList");
            columnTipoComponent.Items.Add("CheckBox");
            columnTipoComponent.Items.Add("Label");

            columnTipoComponentForm.Items.Add("TextBox");
            columnTipoComponentForm.Items.Add("DropDownList");
            columnTipoComponentForm.Items.Add("RadioButtonList");
            columnTipoComponentForm.Items.Add("CheckBox");
            columnTipoComponentForm.Items.Add("Label");

            #endregion

            #region     .....:::::     CARREGA TIPOS DE CAMPO     :::::.....

            DataGridViewComboBoxColumn columnTipoCampo = grvPesquisa.Columns["columnTipoCampo"] as DataGridViewComboBoxColumn;
            DataGridViewComboBoxColumn columnTipoCampoForm = grvForm.Columns["columnTipoCampoForm"] as DataGridViewComboBoxColumn;
            foreach(String strEnum in Enum.GetNames(typeof(WTextBox.ETypeMaskTextField)))
            {
                columnTipoCampo.Items.Add(strEnum);
                columnTipoCampoForm.Items.Add(strEnum);
            }

            #endregion

            #region     .....:::::     CARREGA MASCARAS     :::::.....

            DataGridViewComboBoxColumn columnMascara = grvCamposGrid.Columns["columnMascaraGrid"] as DataGridViewComboBoxColumn;
            columnMascara.Items.Add("Selecione...");

            #endregion

            #region     .....:::::     CARREGA CAMPO VALOR     :::::.....

            DataGridViewComboBoxColumn columnValor = grvPesquisa.Columns["columnValueField"] as DataGridViewComboBoxColumn;
            columnValor.Items.Add("Selecione...");

            DataGridViewComboBoxColumn columnValorForm = grvForm.Columns["columnValueFieldForm"] as DataGridViewComboBoxColumn;
            columnValorForm.Items.Add("Selecione...");

            #endregion

            #region     .....:::::     CARREGA CAMPO MOSTRADO     :::::.....

            DataGridViewComboBoxColumn columnDisplay = grvPesquisa.Columns["columnDisplayField"] as DataGridViewComboBoxColumn;
            columnDisplay.Items.Add("Selecione...");

            DataGridViewComboBoxColumn columnDisplayForm = grvForm.Columns["columnDisplayFieldForm"] as DataGridViewComboBoxColumn;
            columnDisplayForm.Items.Add("Selecione...");

            #endregion

            foreach (CampoVO campo in interfaces[Index].CamposPesquisa)
            {
                grvPesquisa.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = grvPesquisa.Rows[grvPesquisa.Rows.Count - 1];
                row.Cells["columnCampo"].Value = campo.Propriedade.Coluna.Nome;
                row.Cells["columnMostrar"].Value = false;
                row.Cells["columnTipoComponent"].Value = campo.Propriedade.Coluna.ChaveEstrangeira != null ? "DropDownList" : "TextBox";
                row.Cells["columnTipoCampo"].Value = campo.Propriedade.ToTypeMaskTextField().ToString();
                row.Cells["columnObrigatorio"].Value = false;
                row.Cells["columnValueField"].Value = "Selecione...";
                row.Cells["columnDisplayField"].Value = "Selecione..."; 
            }

            foreach (CampoVO campo in interfaces[Index].CamposGrid)
            {
                grvCamposGrid.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = grvCamposGrid.Rows[grvCamposGrid.Rows.Count - 1];
                row.Cells["columnCampoGrid"].Value = campo.Propriedade.Coluna.Nome;
                row.Cells["columnMostrarGrid"].Value = true;
                row.Cells["columnMascaraGrid"].Value = "Selecione...";
            }

            foreach (CampoVO campo in interfaces[Index].CamposForm)
            {
                grvForm.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = grvForm.Rows[grvForm.Rows.Count - 1];
                row.Cells["columnCampoForm"].Value = campo.Propriedade.Coluna.Nome;
                row.Cells["columnMostrarForm"].Value = true;
                row.Cells["columnTipoComponentForm"].Value = campo.Propriedade.Coluna.ChaveEstrangeira != null ? "DropDownList" : "TextBox";
                row.Cells["columnTipoCampoForm"].Value = campo.Propriedade.ToTypeMaskTextField().ToString();
                row.Cells["columnObrigatorioForm"].Value = !campo.Propriedade.Coluna.IsNullable;
                row.Cells["columnValueFieldForm"].Value = "Selecione...";
                row.Cells["columnDisplayFieldForm"].Value = "Selecione..."; 
            }

            txtTitulo.Text = interfaces[Index].Titulo;
        }

        private void SalvarInformacoes()
        {
            InterfaceVO tela = this.interfaces[Index];

            for(int i = 0; i < grvPesquisa.Rows.Count; i++)
            {
                DataGridViewRow row = grvPesquisa.Rows[i];
                CampoVO campo = tela.CamposPesquisa[i];
                campo.Caption = row.Cells["columnCampo"].Value.ToString();
                campo.Gerar = Convert.ToBoolean(row.Cells["columnMostrar"].Value);
                campo.TipoComponent = row.Cells["columnTipoComponent"].Value.ToString().ToTypeComponent();
                campo.TipoCampo = (WTextBox.ETypeMaskTextField)Enum.Parse(typeof(WTextBox.ETypeMaskTextField), row.Cells["columnTipoCampo"].Value.ToString());
                campo.Obrigatorio = Convert.ToBoolean(row.Cells["columnObrigatorio"].Value);
                if(campo.TipoComponent == typeof(WDropDownList) || campo.TipoComponent == typeof(WRadioButtonList))
                {
                    if (row.Cells["columnValueField"].Value.ToString() != "Selecione...")
                    {
                        campo.ValueField = tela.Classe.Propriedades.Find(delegate(PropriedadeVO p) { return p.Coluna.Nome == row.Cells["columnValueField"].Value.ToString(); }).Nome;
                    }
                    if (row.Cells["columnDisplayField"].Value.ToString() != "Selecione...")
                    {
                        campo.DisplayField = tela.Classe.Propriedades.Find(delegate(PropriedadeVO p) { return p.Coluna.Nome == row.Cells["columnDisplayField"].Value.ToString(); }).Nome;
                    }
                }
            }

            for (int i = 0; i < grvCamposGrid.Rows.Count; i++)
            {
                DataGridViewRow row = grvCamposGrid.Rows[i];
                CampoVO campo = tela.CamposGrid[i];
                campo.Caption = row.Cells["columnCampoGrid"].Value.ToString();
                campo.Gerar = Convert.ToBoolean(row.Cells["columnMostrarGrid"].Value);
                campo.Mascara = row.Cells["columnMascaraGrid"].Value.ToString().ToMaskGrid();
            }

            for (int i = 0; i < grvForm.Rows.Count; i++)
            {
                DataGridViewRow row = grvForm.Rows[i];
                CampoVO campo = tela.CamposForm[i];
                campo.Caption = row.Cells["columnCampoForm"].Value.ToString();
                campo.Gerar = Convert.ToBoolean(row.Cells["columnMostrarForm"].Value);
                campo.TipoComponent = row.Cells["columnTipoComponentForm"].Value.ToString().ToTypeComponent();
                campo.TipoCampo = (WTextBox.ETypeMaskTextField)Enum.Parse(typeof(WTextBox.ETypeMaskTextField), row.Cells["columnTipoCampoForm"].Value.ToString());
                campo.Obrigatorio = Convert.ToBoolean(row.Cells["columnObrigatorioForm"].Value);

                if (campo.TipoComponent == typeof(WDropDownList) || campo.TipoComponent == typeof(WRadioButtonList))
                {
                    if (row.Cells["columnValueFieldForm"].Value.ToString() != "Selecione...")
                    {
                        campo.ValueField = tela.Classe.Propriedades.Find(delegate(PropriedadeVO p) { return p.Coluna.Nome == row.Cells["columnValueFieldForm"].Value.ToString(); }).Nome;
                    }
                    if (row.Cells["columnDisplayFieldForm"].Value.ToString() != "Selecione...")
                    {
                        campo.DisplayField = tela.Classe.Propriedades.Find(delegate(PropriedadeVO p) { return p.Coluna.Nome == row.Cells["columnDisplayFieldForm"].Value.ToString(); }).Nome;
                    }
                }
            }
        }

        #endregion

    }
}
