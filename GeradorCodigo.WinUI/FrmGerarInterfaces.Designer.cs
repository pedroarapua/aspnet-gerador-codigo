namespace GeradorCodigo.WinUI
{
    partial class FrmGerarInterfaces
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtTitulo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnProximo = new System.Windows.Forms.Button();
            this.btnVoltar = new System.Windows.Forms.Button();
            this.btnNaoUtilizar = new System.Windows.Forms.Button();
            this.pnlBusca = new System.Windows.Forms.Panel();
            this.grvPesquisa = new System.Windows.Forms.DataGridView();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.grvCamposGrid = new System.Windows.Forms.DataGridView();
            this.columnCampoGrid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMostrarGrid = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnMascaraGrid = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.pnlFormulario = new System.Windows.Forms.Panel();
            this.grvForm = new System.Windows.Forms.DataGridView();
            this.columnCampoForm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMostrarForm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnTipoComponentForm = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnValueFieldForm = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnDisplayFieldForm = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnTipoCampoForm = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnObrigatorioForm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnCampo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMostrar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnTipoComponent = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnValueField = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnDisplayField = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnTipoCampo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnObrigatorio = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlBusca.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvPesquisa)).BeginInit();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvCamposGrid)).BeginInit();
            this.pnlFormulario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvForm)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(22, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(89, 13);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Título da Página:";
            // 
            // txtTitulo
            // 
            this.txtTitulo.Location = new System.Drawing.Point(111, 13);
            this.txtTitulo.Name = "txtTitulo";
            this.txtTitulo.Size = new System.Drawing.Size(644, 20);
            this.txtTitulo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Campos Pesquisa";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 267);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Campos do Grid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 464);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Campos do Formulário";
            // 
            // btnProximo
            // 
            this.btnProximo.Location = new System.Drawing.Point(680, 698);
            this.btnProximo.Name = "btnProximo";
            this.btnProximo.Size = new System.Drawing.Size(75, 23);
            this.btnProximo.TabIndex = 5;
            this.btnProximo.Text = "Próximo";
            this.btnProximo.UseVisualStyleBackColor = true;
            this.btnProximo.Click += new System.EventHandler(this.btnProximo_Click);
            // 
            // btnVoltar
            // 
            this.btnVoltar.Location = new System.Drawing.Point(599, 698);
            this.btnVoltar.Name = "btnVoltar";
            this.btnVoltar.Size = new System.Drawing.Size(75, 23);
            this.btnVoltar.TabIndex = 6;
            this.btnVoltar.Text = "Voltar";
            this.btnVoltar.UseVisualStyleBackColor = true;
            this.btnVoltar.Click += new System.EventHandler(this.btnVoltar_Click);
            // 
            // btnNaoUtilizar
            // 
            this.btnNaoUtilizar.Location = new System.Drawing.Point(28, 697);
            this.btnNaoUtilizar.Name = "btnNaoUtilizar";
            this.btnNaoUtilizar.Size = new System.Drawing.Size(123, 23);
            this.btnNaoUtilizar.TabIndex = 7;
            this.btnNaoUtilizar.Text = "Não Gerar Formulário";
            this.btnNaoUtilizar.UseVisualStyleBackColor = true;
            this.btnNaoUtilizar.Click += new System.EventHandler(this.btnNaoUtilizar_Click);
            // 
            // pnlBusca
            // 
            this.pnlBusca.Controls.Add(this.grvPesquisa);
            this.pnlBusca.Location = new System.Drawing.Point(25, 54);
            this.pnlBusca.Name = "pnlBusca";
            this.pnlBusca.Size = new System.Drawing.Size(730, 203);
            this.pnlBusca.TabIndex = 0;
            // 
            // grvPesquisa
            // 
            this.grvPesquisa.AllowUserToAddRows = false;
            this.grvPesquisa.AllowUserToDeleteRows = false;
            this.grvPesquisa.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grvPesquisa.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnCampo,
            this.columnMostrar,
            this.columnTipoComponent,
            this.columnValueField,
            this.columnDisplayField,
            this.columnTipoCampo,
            this.columnObrigatorio});
            this.grvPesquisa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grvPesquisa.Location = new System.Drawing.Point(0, 0);
            this.grvPesquisa.Name = "grvPesquisa";
            this.grvPesquisa.Size = new System.Drawing.Size(730, 203);
            this.grvPesquisa.TabIndex = 0;
            this.grvPesquisa.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grvPesquisa_CellClick);
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.grvCamposGrid);
            this.pnlGrid.Location = new System.Drawing.Point(28, 283);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(542, 178);
            this.pnlGrid.TabIndex = 1;
            // 
            // grvCamposGrid
            // 
            this.grvCamposGrid.AllowUserToAddRows = false;
            this.grvCamposGrid.AllowUserToDeleteRows = false;
            this.grvCamposGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grvCamposGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnCampoGrid,
            this.columnMostrarGrid,
            this.columnMascaraGrid});
            this.grvCamposGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grvCamposGrid.Location = new System.Drawing.Point(0, 0);
            this.grvCamposGrid.Name = "grvCamposGrid";
            this.grvCamposGrid.Size = new System.Drawing.Size(542, 178);
            this.grvCamposGrid.TabIndex = 1;
            this.grvCamposGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grvCamposGrid_CellClick);
            // 
            // columnCampoGrid
            // 
            this.columnCampoGrid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnCampoGrid.HeaderText = "Campo";
            this.columnCampoGrid.Name = "columnCampoGrid";
            this.columnCampoGrid.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // columnMostrarGrid
            // 
            this.columnMostrarGrid.HeaderText = "Mostrar?";
            this.columnMostrarGrid.Name = "columnMostrarGrid";
            this.columnMostrarGrid.Width = 50;
            // 
            // columnMascaraGrid
            // 
            this.columnMascaraGrid.HeaderText = "Mascara";
            this.columnMascaraGrid.Name = "columnMascaraGrid";
            this.columnMascaraGrid.Width = 120;
            // 
            // pnlFormulario
            // 
            this.pnlFormulario.Controls.Add(this.grvForm);
            this.pnlFormulario.Location = new System.Drawing.Point(25, 480);
            this.pnlFormulario.Name = "pnlFormulario";
            this.pnlFormulario.Size = new System.Drawing.Size(730, 203);
            this.pnlFormulario.TabIndex = 2;
            // 
            // grvForm
            // 
            this.grvForm.AllowUserToAddRows = false;
            this.grvForm.AllowUserToDeleteRows = false;
            this.grvForm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grvForm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnCampoForm,
            this.columnMostrarForm,
            this.columnTipoComponentForm,
            this.columnValueFieldForm,
            this.columnDisplayFieldForm,
            this.columnTipoCampoForm,
            this.columnObrigatorioForm});
            this.grvForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grvForm.Location = new System.Drawing.Point(0, 0);
            this.grvForm.Name = "grvForm";
            this.grvForm.Size = new System.Drawing.Size(730, 203);
            this.grvForm.TabIndex = 1;
            this.grvForm.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grvForm_CellClick);
            // 
            // columnCampoForm
            // 
            this.columnCampoForm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnCampoForm.HeaderText = "Campo";
            this.columnCampoForm.Name = "columnCampoForm";
            this.columnCampoForm.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // columnMostrarForm
            // 
            this.columnMostrarForm.HeaderText = "Mostrar?";
            this.columnMostrarForm.Name = "columnMostrarForm";
            this.columnMostrarForm.Width = 50;
            // 
            // columnTipoComponentForm
            // 
            this.columnTipoComponentForm.HeaderText = "Tipo de Componente";
            this.columnTipoComponentForm.Name = "columnTipoComponentForm";
            this.columnTipoComponentForm.Width = 120;
            // 
            // columnValueFieldForm
            // 
            this.columnValueFieldForm.HeaderText = "Campo de Valor";
            this.columnValueFieldForm.Name = "columnValueFieldForm";
            // 
            // columnDisplayFieldForm
            // 
            this.columnDisplayFieldForm.HeaderText = "Campo Mostrado";
            this.columnDisplayFieldForm.Name = "columnDisplayFieldForm";
            // 
            // columnTipoCampoForm
            // 
            this.columnTipoCampoForm.HeaderText = "Tipo Campo";
            this.columnTipoCampoForm.Name = "columnTipoCampoForm";
            // 
            // columnObrigatorioForm
            // 
            this.columnObrigatorioForm.HeaderText = "Obrigatório?";
            this.columnObrigatorioForm.Name = "columnObrigatorioForm";
            this.columnObrigatorioForm.Width = 70;
            // 
            // columnCampo
            // 
            this.columnCampo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnCampo.HeaderText = "Campo";
            this.columnCampo.Name = "columnCampo";
            this.columnCampo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // columnMostrar
            // 
            this.columnMostrar.HeaderText = "Mostrar?";
            this.columnMostrar.Name = "columnMostrar";
            this.columnMostrar.Width = 50;
            // 
            // columnTipoComponent
            // 
            this.columnTipoComponent.HeaderText = "Tipo de Componente";
            this.columnTipoComponent.Name = "columnTipoComponent";
            this.columnTipoComponent.Width = 120;
            // 
            // columnValueField
            // 
            this.columnValueField.HeaderText = "Campo Valor";
            this.columnValueField.Name = "columnValueField";
            // 
            // columnDisplayField
            // 
            this.columnDisplayField.HeaderText = "Campo Mostrado";
            this.columnDisplayField.Name = "columnDisplayField";
            // 
            // columnTipoCampo
            // 
            this.columnTipoCampo.HeaderText = "Tipo Campo";
            this.columnTipoCampo.Name = "columnTipoCampo";
            // 
            // columnObrigatorio
            // 
            this.columnObrigatorio.HeaderText = "Obrigatório?";
            this.columnObrigatorio.Name = "columnObrigatorio";
            this.columnObrigatorio.Width = 70;
            // 
            // FrmGerarInterfaces
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(801, 562);
            this.Controls.Add(this.btnNaoUtilizar);
            this.Controls.Add(this.btnVoltar);
            this.Controls.Add(this.btnProximo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTitulo);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pnlFormulario);
            this.Controls.Add(this.pnlGrid);
            this.Controls.Add(this.pnlBusca);
            this.MinimizeBox = false;
            this.Name = "FrmGerarInterfaces";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gerar Interfaces";
            this.pnlBusca.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grvPesquisa)).EndInit();
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grvCamposGrid)).EndInit();
            this.pnlFormulario.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grvForm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBusca;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TextBox txtTitulo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlFormulario;
        private System.Windows.Forms.Button btnProximo;
        private System.Windows.Forms.Button btnVoltar;
        private System.Windows.Forms.Button btnNaoUtilizar;
        private System.Windows.Forms.DataGridView grvPesquisa;
        private System.Windows.Forms.DataGridView grvCamposGrid;
        private System.Windows.Forms.DataGridView grvForm;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCampoGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnMostrarGrid;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnMascaraGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCampo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnMostrar;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnTipoComponent;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnValueField;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnDisplayField;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnTipoCampo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnObrigatorio;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCampoForm;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnMostrarForm;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnTipoComponentForm;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnValueFieldForm;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnDisplayFieldForm;
        private System.Windows.Forms.DataGridViewComboBoxColumn columnTipoCampoForm;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnObrigatorioForm;
    }
}