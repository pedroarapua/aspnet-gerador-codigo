namespace GeradorCodigo.WinUI
{
    partial class FrmGeradorProjetos
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
            this.lblServidor = new System.Windows.Forms.Label();
            this.lblDriver = new System.Windows.Forms.Label();
            this.cboDriver = new System.Windows.Forms.ComboBox();
            this.lblBanco = new System.Windows.Forms.Label();
            this.cboBanco = new System.Windows.Forms.ComboBox();
            this.treeTabelasCampos = new System.Windows.Forms.TreeView();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnGerarCodigo = new System.Windows.Forms.Button();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblObrigatorioSenha = new System.Windows.Forms.Label();
            this.lblObrigatorioLogin = new System.Windows.Forms.Label();
            this.chkTipoAutenticacao = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConnectar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSolucao = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSeparadorColuna = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtSeparadorTabela = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPrefixoColuna = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtPrefixoTabela = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSelecionar = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.folderBrowsDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlGerar = new System.Windows.Forms.Panel();
            this.chkGerarInterfaces = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.pnlGerar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblServidor
            // 
            this.lblServidor.AutoSize = true;
            this.lblServidor.Location = new System.Drawing.Point(12, 71);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(49, 13);
            this.lblServidor.TabIndex = 1;
            this.lblServidor.Text = "Servidor:";
            // 
            // lblDriver
            // 
            this.lblDriver.AutoSize = true;
            this.lblDriver.Location = new System.Drawing.Point(17, 45);
            this.lblDriver.Name = "lblDriver";
            this.lblDriver.Size = new System.Drawing.Size(38, 13);
            this.lblDriver.TabIndex = 3;
            this.lblDriver.Text = "Driver:";
            // 
            // cboDriver
            // 
            this.cboDriver.FormattingEnabled = true;
            this.cboDriver.Location = new System.Drawing.Point(61, 45);
            this.cboDriver.Name = "cboDriver";
            this.cboDriver.Size = new System.Drawing.Size(237, 21);
            this.cboDriver.TabIndex = 1;
            // 
            // lblBanco
            // 
            this.lblBanco.AutoSize = true;
            this.lblBanco.Location = new System.Drawing.Point(37, 23);
            this.lblBanco.Name = "lblBanco";
            this.lblBanco.Size = new System.Drawing.Size(41, 13);
            this.lblBanco.TabIndex = 5;
            this.lblBanco.Text = "Banco:";
            // 
            // cboBanco
            // 
            this.cboBanco.Enabled = false;
            this.cboBanco.FormattingEnabled = true;
            this.cboBanco.Location = new System.Drawing.Point(84, 23);
            this.cboBanco.Name = "cboBanco";
            this.cboBanco.Size = new System.Drawing.Size(214, 21);
            this.cboBanco.TabIndex = 7;
            this.cboBanco.SelectionChangeCommitted += new System.EventHandler(this.cboBanco_SelectionChangeCommitted);
            // 
            // treeTabelasCampos
            // 
            this.treeTabelasCampos.CheckBoxes = true;
            this.treeTabelasCampos.Location = new System.Drawing.Point(351, 23);
            this.treeTabelasCampos.Name = "treeTabelasCampos";
            this.treeTabelasCampos.Size = new System.Drawing.Size(421, 484);
            this.treeTabelasCampos.TabIndex = 6;
            this.treeTabelasCampos.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeTabelasCampos_NodeMouseClick);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(354, 7);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(88, 13);
            this.lblTitulo.TabIndex = 7;
            this.lblTitulo.Text = "Tabelas/Campos";
            // 
            // btnGerarCodigo
            // 
            this.btnGerarCodigo.Location = new System.Drawing.Point(109, 251);
            this.btnGerarCodigo.Name = "btnGerarCodigo";
            this.btnGerarCodigo.Size = new System.Drawing.Size(75, 23);
            this.btnGerarCodigo.TabIndex = 16;
            this.btnGerarCodigo.Text = "Gerar";
            this.btnGerarCodigo.UseVisualStyleBackColor = true;
            this.btnGerarCodigo.Click += new System.EventHandler(this.btnGerarCodigo_Click);
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(61, 72);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(237, 20);
            this.txtServidor.TabIndex = 2;
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(25, 120);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(36, 13);
            this.lblLogin.TabIndex = 15;
            this.lblLogin.Text = "Login:";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(61, 117);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(237, 20);
            this.txtLogin.TabIndex = 4;
            // 
            // txtSenha
            // 
            this.txtSenha.Location = new System.Drawing.Point(61, 141);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '*';
            this.txtSenha.Size = new System.Drawing.Size(237, 20);
            this.txtSenha.TabIndex = 5;
            this.txtSenha.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Senha:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblObrigatorioSenha);
            this.panel1.Controls.Add(this.lblObrigatorioLogin);
            this.panel1.Controls.Add(this.chkTipoAutenticacao);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnConnectar);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtServidor);
            this.panel1.Controls.Add(this.txtSenha);
            this.panel1.Controls.Add(this.lblServidor);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblLogin);
            this.panel1.Controls.Add(this.txtLogin);
            this.panel1.Controls.Add(this.lblDriver);
            this.panel1.Controls.Add(this.cboDriver);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(325, 198);
            this.panel1.TabIndex = 19;
            // 
            // lblObrigatorioSenha
            // 
            this.lblObrigatorioSenha.AutoSize = true;
            this.lblObrigatorioSenha.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObrigatorioSenha.ForeColor = System.Drawing.Color.Red;
            this.lblObrigatorioSenha.Location = new System.Drawing.Point(301, 141);
            this.lblObrigatorioSenha.Name = "lblObrigatorioSenha";
            this.lblObrigatorioSenha.Size = new System.Drawing.Size(12, 15);
            this.lblObrigatorioSenha.TabIndex = 26;
            this.lblObrigatorioSenha.Text = "*";
            // 
            // lblObrigatorioLogin
            // 
            this.lblObrigatorioLogin.AutoSize = true;
            this.lblObrigatorioLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObrigatorioLogin.ForeColor = System.Drawing.Color.Red;
            this.lblObrigatorioLogin.Location = new System.Drawing.Point(301, 118);
            this.lblObrigatorioLogin.Name = "lblObrigatorioLogin";
            this.lblObrigatorioLogin.Size = new System.Drawing.Size(12, 15);
            this.lblObrigatorioLogin.TabIndex = 25;
            this.lblObrigatorioLogin.Text = "*";
            // 
            // chkTipoAutenticacao
            // 
            this.chkTipoAutenticacao.AutoSize = true;
            this.chkTipoAutenticacao.Location = new System.Drawing.Point(61, 98);
            this.chkTipoAutenticacao.Name = "chkTipoAutenticacao";
            this.chkTipoAutenticacao.Size = new System.Drawing.Size(144, 17);
            this.chkTipoAutenticacao.TabIndex = 3;
            this.chkTipoAutenticacao.Text = "Windows Autentication ?";
            this.chkTipoAutenticacao.UseVisualStyleBackColor = true;
            this.chkTipoAutenticacao.CheckedChanged += new System.EventHandler(this.chkTipoAutenticacao_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(301, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 15);
            this.label5.TabIndex = 24;
            this.label5.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(301, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 15);
            this.label4.TabIndex = 23;
            this.label4.Text = "*";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(217, 167);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 22;
            this.lblStatus.Text = "Offiline";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(179, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Status:";
            // 
            // btnConnectar
            // 
            this.btnConnectar.Location = new System.Drawing.Point(61, 167);
            this.btnConnectar.Name = "btnConnectar";
            this.btnConnectar.Size = new System.Drawing.Size(87, 23);
            this.btnConnectar.TabIndex = 6;
            this.btnConnectar.Text = "Conectar";
            this.btnConnectar.UseVisualStyleBackColor = true;
            this.btnConnectar.Click += new System.EventHandler(this.btnConnectar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(278, 15);
            this.label2.TabIndex = 19;
            this.label2.Text = "Connectar ao Servidor de Banco de Dados";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(270, 203);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(12, 15);
            this.label13.TabIndex = 27;
            this.label13.Text = "*";
            // 
            // txtSolucao
            // 
            this.txtSolucao.Location = new System.Drawing.Point(109, 202);
            this.txtSolucao.Name = "txtSolucao";
            this.txtSolucao.Size = new System.Drawing.Size(155, 20);
            this.txtSolucao.TabIndex = 14;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 202);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 13);
            this.label12.TabIndex = 36;
            this.label12.Text = "Nome da Solução:";
            // 
            // txtSeparadorColuna
            // 
            this.txtSeparadorColuna.Location = new System.Drawing.Point(138, 166);
            this.txtSeparadorColuna.MaxLength = 1;
            this.txtSeparadorColuna.Name = "txtSeparadorColuna";
            this.txtSeparadorColuna.Size = new System.Drawing.Size(42, 20);
            this.txtSeparadorColuna.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(37, 166);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 13);
            this.label11.TabIndex = 34;
            this.label11.Text = "Separador Coluna:";
            // 
            // txtSeparadorTabela
            // 
            this.txtSeparadorTabela.Location = new System.Drawing.Point(138, 109);
            this.txtSeparadorTabela.MaxLength = 1;
            this.txtSeparadorTabela.Name = "txtSeparadorTabela";
            this.txtSeparadorTabela.Size = new System.Drawing.Size(48, 20);
            this.txtSeparadorTabela.TabIndex = 11;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(37, 112);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 32;
            this.label10.Text = "Separador Tabela:";
            // 
            // txtPrefixoColuna
            // 
            this.txtPrefixoColuna.Location = new System.Drawing.Point(138, 140);
            this.txtPrefixoColuna.MaxLength = 2;
            this.txtPrefixoColuna.Name = "txtPrefixoColuna";
            this.txtPrefixoColuna.Size = new System.Drawing.Size(42, 20);
            this.txtPrefixoColuna.TabIndex = 12;
            this.txtPrefixoColuna.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTextBoxNumeros_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 141);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(126, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Tamanho Prefixo Coluna:";
            // 
            // txtPrefixoTabela
            // 
            this.txtPrefixoTabela.Location = new System.Drawing.Point(138, 82);
            this.txtPrefixoTabela.MaxLength = 2;
            this.txtPrefixoTabela.Name = "txtPrefixoTabela";
            this.txtPrefixoTabela.Size = new System.Drawing.Size(48, 20);
            this.txtPrefixoTabela.TabIndex = 10;
            this.txtPrefixoTabela.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTextBoxNumeros_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Tamanho Prefixo Tabela:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(310, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 15);
            this.label7.TabIndex = 27;
            this.label7.Text = "*";
            // 
            // btnSelecionar
            // 
            this.btnSelecionar.Location = new System.Drawing.Point(238, 51);
            this.btnSelecionar.Name = "btnSelecionar";
            this.btnSelecionar.Size = new System.Drawing.Size(69, 23);
            this.btnSelecionar.TabIndex = 9;
            this.btnSelecionar.Text = "Selecionar";
            this.btnSelecionar.UseVisualStyleBackColor = true;
            this.btnSelecionar.Click += new System.EventHandler(this.btnSelecionar_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(84, 53);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(141, 20);
            this.txtPath.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(46, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Path:";
            // 
            // pnlGerar
            // 
            this.pnlGerar.Controls.Add(this.chkGerarInterfaces);
            this.pnlGerar.Controls.Add(this.label13);
            this.pnlGerar.Controls.Add(this.txtSolucao);
            this.pnlGerar.Controls.Add(this.label12);
            this.pnlGerar.Controls.Add(this.txtSeparadorColuna);
            this.pnlGerar.Controls.Add(this.label11);
            this.pnlGerar.Controls.Add(this.txtSeparadorTabela);
            this.pnlGerar.Controls.Add(this.label10);
            this.pnlGerar.Controls.Add(this.txtPrefixoColuna);
            this.pnlGerar.Controls.Add(this.label9);
            this.pnlGerar.Controls.Add(this.txtPrefixoTabela);
            this.pnlGerar.Controls.Add(this.label8);
            this.pnlGerar.Controls.Add(this.label7);
            this.pnlGerar.Controls.Add(this.btnSelecionar);
            this.pnlGerar.Controls.Add(this.txtPath);
            this.pnlGerar.Controls.Add(this.label6);
            this.pnlGerar.Controls.Add(this.cboBanco);
            this.pnlGerar.Controls.Add(this.lblBanco);
            this.pnlGerar.Controls.Add(this.btnGerarCodigo);
            this.pnlGerar.Enabled = false;
            this.pnlGerar.Location = new System.Drawing.Point(12, 216);
            this.pnlGerar.Name = "pnlGerar";
            this.pnlGerar.Size = new System.Drawing.Size(325, 291);
            this.pnlGerar.TabIndex = 20;
            // 
            // chkGerarInterfaces
            // 
            this.chkGerarInterfaces.AutoSize = true;
            this.chkGerarInterfaces.Location = new System.Drawing.Point(112, 228);
            this.chkGerarInterfaces.Name = "chkGerarInterfaces";
            this.chkGerarInterfaces.Size = new System.Drawing.Size(102, 17);
            this.chkGerarInterfaces.TabIndex = 15;
            this.chkGerarInterfaces.Text = "Gerar Interfaces";
            this.chkGerarInterfaces.UseVisualStyleBackColor = true;
            // 
            // FrmGeradorProjetos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.pnlGerar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.treeTabelasCampos);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGeradorProjetos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gerar Projetos";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlGerar.ResumeLayout(false);
            this.pnlGerar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServidor;
        private System.Windows.Forms.Label lblDriver;
        private System.Windows.Forms.ComboBox cboDriver;
        private System.Windows.Forms.Label lblBanco;
        private System.Windows.Forms.ComboBox cboBanco;
        private System.Windows.Forms.TreeView treeTabelasCampos;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnGerarCodigo;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnConnectar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkTipoAutenticacao;
        private System.Windows.Forms.Label lblObrigatorioSenha;
        private System.Windows.Forms.Label lblObrigatorioLogin;
        private System.Windows.Forms.Button btnSelecionar;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.FolderBrowserDialog folderBrowsDialog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSeparadorTabela;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtPrefixoColuna;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPrefixoTabela;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSeparadorColuna;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtSolucao;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel pnlGerar;
        private System.Windows.Forms.CheckBox chkGerarInterfaces;
    }
}

