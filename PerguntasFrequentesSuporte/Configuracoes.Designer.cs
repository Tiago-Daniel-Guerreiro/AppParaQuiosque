
namespace PerguntasFrequentesSuporte
{
    partial class Configuracoes
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
            listBoxDetalhes = new System.Windows.Forms.ListBox();
            btnClose = new System.Windows.Forms.Button();
            TreeViewConfig = new System.Windows.Forms.TreeView();
            btnSalvar = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            btnRenomearPerfil = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            btnCarregarPadrao = new System.Windows.Forms.Button();
            btnResetarAlteracoes = new System.Windows.Forms.Button();
            tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            label2 = new System.Windows.Forms.Label();
            comboBoxTema = new System.Windows.Forms.ComboBox();
            tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            comboBoxPerfis = new System.Windows.Forms.ComboBox();
            tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            btnApagarPerfil = new System.Windows.Forms.Button();
            btnNovoPerfil = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxDetalhes
            // 
            listBoxDetalhes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBoxDetalhes.Dock = System.Windows.Forms.DockStyle.Fill;
            listBoxDetalhes.FormattingEnabled = true;
            listBoxDetalhes.ItemHeight = 15;
            listBoxDetalhes.Location = new System.Drawing.Point(305, 13);
            listBoxDetalhes.Name = "listBoxDetalhes";
            listBoxDetalhes.Size = new System.Drawing.Size(460, 301);
            listBoxDetalhes.TabIndex = 0;
            listBoxDetalhes.Tag = "lista detalhes";
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(12, 708);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 23);
            btnClose.TabIndex = 4;
            btnClose.Text = "Sair";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // TreeViewConfig
            // 
            TreeViewConfig.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TreeViewConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            TreeViewConfig.Location = new System.Drawing.Point(13, 13);
            TreeViewConfig.Name = "TreeViewConfig";
            TreeViewConfig.PathSeparator = ".";
            TreeViewConfig.Size = new System.Drawing.Size(286, 301);
            TreeViewConfig.TabIndex = 5;
            TreeViewConfig.Tag = "Arvore Configs";
            TreeViewConfig.AfterSelect += TreeViewConfig_AfterSelect;
            TreeViewConfig.DoubleClick += TreeViewConfig_DoubleClick;
            // 
            // btnSalvar
            // 
            btnSalvar.Location = new System.Drawing.Point(93, 708);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new System.Drawing.Size(75, 23);
            btnSalvar.TabIndex = 6;
            btnSalvar.Text = "Salvar alterações";
            btnSalvar.UseVisualStyleBackColor = true;
            btnSalvar.Click += BtnSalvar_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.7777786F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 72.22222F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(784, 511);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 4;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel4.Controls.Add(button2, 1, 0);
            tableLayoutPanel4.Controls.Add(button1, 0, 0);
            tableLayoutPanel4.Controls.Add(btnResetarAlteracoes, 3, 0);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(3, 464);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new System.Drawing.Size(778, 44);
            tableLayoutPanel4.TabIndex = 8;
            // 
            // btnRenomearPerfil
            // 
            btnRenomearPerfil.Dock = System.Windows.Forms.DockStyle.Fill;
            btnRenomearPerfil.Location = new System.Drawing.Point(3, 3);
            btnRenomearPerfil.Name = "btnRenomearPerfil";
            btnRenomearPerfil.Size = new System.Drawing.Size(174, 49);
            btnRenomearPerfil.TabIndex = 9;
            btnRenomearPerfil.Tag = "Botao de Salvar/Sair";
            btnRenomearPerfil.Text = "Renomear Perfil";
            btnRenomearPerfil.UseVisualStyleBackColor = true;
            btnRenomearPerfil.Click += btnRenomearPerfil_Click;
            // 
            // button2
            // 
            button2.Dock = System.Windows.Forms.DockStyle.Fill;
            button2.Location = new System.Drawing.Point(200, 6);
            button2.Margin = new System.Windows.Forms.Padding(6);
            button2.Name = "button2";
            button2.Padding = new System.Windows.Forms.Padding(3);
            button2.Size = new System.Drawing.Size(182, 32);
            button2.TabIndex = 4;
            button2.Tag = "Botao de Salvar/Sair";
            button2.Text = "Cancelar";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnClose_Click;
            // 
            // button1
            // 
            button1.Dock = System.Windows.Forms.DockStyle.Fill;
            button1.Location = new System.Drawing.Point(6, 6);
            button1.Margin = new System.Windows.Forms.Padding(6);
            button1.Name = "button1";
            button1.Padding = new System.Windows.Forms.Padding(3);
            button1.Size = new System.Drawing.Size(182, 32);
            button1.TabIndex = 3;
            button1.Tag = "Botao de Salvar/Sair";
            button1.Text = "Confirmar Alterações";
            button1.UseVisualStyleBackColor = true;
            button1.Click += BtnSalvar_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.65031F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.34969F));
            tableLayoutPanel2.Controls.Add(listBoxDetalhes, 1, 0);
            tableLayoutPanel2.Controls.Add(TreeViewConfig, 0, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 131);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(10);
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(778, 327);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.8277626F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.1722374F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel8, 1, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel6, 0, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel7, 1, 0);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new System.Drawing.Size(778, 122);
            tableLayoutPanel3.TabIndex = 2;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 2;
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.Controls.Add(btnRenomearPerfil, 0, 0);
            tableLayoutPanel8.Controls.Add(btnCarregarPadrao, 1, 0);
            tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel8.Location = new System.Drawing.Point(414, 64);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel8.Size = new System.Drawing.Size(361, 55);
            tableLayoutPanel8.TabIndex = 8;
            // 
            // btnCarregarPadrao
            // 
            btnCarregarPadrao.Location = new System.Drawing.Point(183, 3);
            btnCarregarPadrao.Name = "btnCarregarPadrao";
            btnCarregarPadrao.Size = new System.Drawing.Size(175, 49);
            btnCarregarPadrao.TabIndex = 3;
            btnCarregarPadrao.Tag = "Botao de Salvar/Sair";
            btnCarregarPadrao.Text = "Carregar Padrão";
            btnCarregarPadrao.UseVisualStyleBackColor = true;
            btnCarregarPadrao.Click += btnCarregarPadrao_Click;
            // 
            // btnResetarAlteracoes
            // 
            btnResetarAlteracoes.Dock = System.Windows.Forms.DockStyle.Fill;
            btnResetarAlteracoes.Location = new System.Drawing.Point(585, 3);
            btnResetarAlteracoes.Name = "btnResetarAlteracoes";
            btnResetarAlteracoes.Size = new System.Drawing.Size(190, 38);
            btnResetarAlteracoes.TabIndex = 2;
            btnResetarAlteracoes.Tag = "Botao de Salvar/Sair";
            btnResetarAlteracoes.Text = "Cancelar Alterações";
            btnResetarAlteracoes.UseVisualStyleBackColor = true;
            btnResetarAlteracoes.Click += btnResetarAlteracoes_Click;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(label2, 0, 0);
            tableLayoutPanel6.Controls.Add(comboBoxTema, 0, 1);
            tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel6.Location = new System.Drawing.Point(3, 64);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new System.Drawing.Size(405, 55);
            tableLayoutPanel6.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(67, 15);
            label2.TabIndex = 3;
            label2.Text = "Tema atual:";
            // 
            // comboBoxTema
            // 
            comboBoxTema.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBoxTema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxTema.FormattingEnabled = true;
            comboBoxTema.Location = new System.Drawing.Point(3, 30);
            comboBoxTema.Name = "comboBoxTema";
            comboBoxTema.Size = new System.Drawing.Size(399, 23);
            comboBoxTema.TabIndex = 2;
            comboBoxTema.Tag = "Botao de Salvar/Sair";
            comboBoxTema.SelectedIndexChanged += comboBoxTema_SelectedIndexChanged;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(label1, 0, 0);
            tableLayoutPanel5.Controls.Add(comboBoxPerfis, 0, 1);
            tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 3;
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel5.Size = new System.Drawing.Size(405, 55);
            tableLayoutPanel5.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 15);
            label1.TabIndex = 3;
            label1.Text = "Perfil atual:";
            // 
            // comboBoxPerfis
            // 
            comboBoxPerfis.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBoxPerfis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxPerfis.FormattingEnabled = true;
            comboBoxPerfis.Location = new System.Drawing.Point(3, 20);
            comboBoxPerfis.Name = "comboBoxPerfis";
            comboBoxPerfis.Size = new System.Drawing.Size(399, 23);
            comboBoxPerfis.TabIndex = 2;
            comboBoxPerfis.Tag = "Botao de Salvar/Sair";
            comboBoxPerfis.SelectedIndexChanged += comboBoxPerfis_SelectedIndexChanged;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.Controls.Add(btnApagarPerfil, 1, 0);
            tableLayoutPanel7.Controls.Add(btnNovoPerfil, 0, 0);
            tableLayoutPanel7.Location = new System.Drawing.Point(414, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel7.Size = new System.Drawing.Size(361, 55);
            tableLayoutPanel7.TabIndex = 6;
            // 
            // btnApagarPerfil
            // 
            btnApagarPerfil.Dock = System.Windows.Forms.DockStyle.Fill;
            btnApagarPerfil.Location = new System.Drawing.Point(183, 3);
            btnApagarPerfil.Name = "btnApagarPerfil";
            btnApagarPerfil.Size = new System.Drawing.Size(175, 49);
            btnApagarPerfil.TabIndex = 1;
            btnApagarPerfil.Tag = "Botao de Salvar/Sair";
            btnApagarPerfil.Text = "Apagar Perfil";
            btnApagarPerfil.UseVisualStyleBackColor = true;
            btnApagarPerfil.Click += btnApagarPerfil_Click;
            // 
            // btnNovoPerfil
            // 
            btnNovoPerfil.Dock = System.Windows.Forms.DockStyle.Fill;
            btnNovoPerfil.Location = new System.Drawing.Point(3, 3);
            btnNovoPerfil.Name = "btnNovoPerfil";
            btnNovoPerfil.Size = new System.Drawing.Size(174, 49);
            btnNovoPerfil.TabIndex = 0;
            btnNovoPerfil.Tag = "Botao de Salvar/Sair";
            btnNovoPerfil.Text = "Novo Perfil";
            btnNovoPerfil.UseVisualStyleBackColor = true;
            btnNovoPerfil.Click += btnNovoPerfil_Click;
            // 
            // Configuracoes
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(784, 511);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(btnSalvar);
            Controls.Add(btnClose);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(614, 349);
            Name = "Configuracoes";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Configuracoes";
            TopMost = true;
            FormClosing += Configuracoes_FormClosing;
            Load += Configuracoes_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ListBox listBoxDetalhes;
        public System.Windows.Forms.ListBox listBoxConfig;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TreeView TreeViewConfig;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxPerfis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxTema;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Button btnCarregarPadrao;
        private System.Windows.Forms.Button btnResetarAlteracoes;
        private System.Windows.Forms.Button btnApagarPerfil;
        private System.Windows.Forms.Button btnNovoPerfil;
        private System.Windows.Forms.Button btnRenomearPerfil;
    }
}
