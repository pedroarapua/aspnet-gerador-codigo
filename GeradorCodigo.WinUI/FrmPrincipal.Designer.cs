namespace GeradorCodigo.WinUI
{
    partial class FrmPrincipal
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
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.gerarProjetosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGerarProjetos = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGerarInterfaces = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gerarProjetosToolStripMenuItem});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Size = new System.Drawing.Size(784, 24);
            this.menuPrincipal.TabIndex = 0;
            this.menuPrincipal.Text = "menuStrip1";
            // 
            // gerarProjetosToolStripMenuItem
            // 
            this.gerarProjetosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGerarProjetos,
            this.menuGerarInterfaces});
            this.gerarProjetosToolStripMenuItem.Name = "gerarProjetosToolStripMenuItem";
            this.gerarProjetosToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.gerarProjetosToolStripMenuItem.Text = "Arquivo";
            // 
            // menuGerarProjetos
            // 
            this.menuGerarProjetos.Name = "menuGerarProjetos";
            this.menuGerarProjetos.Size = new System.Drawing.Size(156, 22);
            this.menuGerarProjetos.Text = "Gerar Projetos";
            this.menuGerarProjetos.Click += new System.EventHandler(this.gerarProjetosToolStripMenuItem1_Click);
            // 
            // menuGerarInterfaces
            // 
            this.menuGerarInterfaces.Name = "menuGerarInterfaces";
            this.menuGerarInterfaces.Size = new System.Drawing.Size(156, 22);
            this.menuGerarInterfaces.Text = "Gerar Interfaces";
            this.menuGerarInterfaces.Click += new System.EventHandler(this.menuGerarInterfaces_Click);
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.menuPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuPrincipal;
            this.Name = "FrmPrincipal";
            this.Text = "Gerador de Código";
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem gerarProjetosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuGerarProjetos;
        private System.Windows.Forms.ToolStripMenuItem menuGerarInterfaces;
    }
}