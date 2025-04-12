
namespace PerguntasFrequentesSuporte
{
    partial class PassoAPasso
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassoAPasso));
            picBox = new System.Windows.Forms.PictureBox();
            btnClose = new System.Windows.Forms.Button();
            PanelPrincipal = new System.Windows.Forms.TableLayoutPanel();
            PanelPassoAnterior_Proximo = new System.Windows.Forms.TableLayoutPanel();
            btnAnterior = new System.Windows.Forms.Button();
            btnProximo = new System.Windows.Forms.Button();
            PanelBtns = new System.Windows.Forms.TableLayoutPanel();
            Btn2 = new System.Windows.Forms.Button();
            Btn1 = new System.Windows.Forms.Button();
            Btn0 = new System.Windows.Forms.Button();
            txtPasso = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)picBox).BeginInit();
            PanelPrincipal.SuspendLayout();
            PanelPassoAnterior_Proximo.SuspendLayout();
            PanelBtns.SuspendLayout();
            SuspendLayout();
            // 
            // picBox
            // 
            picBox.Dock = System.Windows.Forms.DockStyle.Fill;
            picBox.ErrorImage = (System.Drawing.Image)resources.GetObject("picBox.ErrorImage");
            picBox.InitialImage = (System.Drawing.Image)resources.GetObject("picBox.InitialImage");
            picBox.Location = new System.Drawing.Point(3, 87);
            picBox.Name = "picBox";
            picBox.Size = new System.Drawing.Size(332, 338);
            picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            picBox.TabIndex = 4;
            picBox.TabStop = false;
            // 
            // btnClose
            // 
            btnClose.AutoSize = true;
            btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            btnClose.Location = new System.Drawing.Point(3, 431);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(332, 30);
            btnClose.TabIndex = 8;
            btnClose.Tag = "Botao Sair";
            btnClose.Text = "Fechar";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // PanelPrincipal
            // 
            PanelPrincipal.ColumnCount = 1;
            PanelPrincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            PanelPrincipal.Controls.Add(btnClose, 0, 3);
            PanelPrincipal.Controls.Add(picBox, 0, 2);
            PanelPrincipal.Controls.Add(PanelPassoAnterior_Proximo, 0, 1);
            PanelPrincipal.Controls.Add(PanelBtns, 0, 0);
            PanelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            PanelPrincipal.Location = new System.Drawing.Point(0, 0);
            PanelPrincipal.Name = "PanelPrincipal";
            PanelPrincipal.RowCount = 4;
            PanelPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.226985F));
            PanelPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.226985F));
            PanelPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 74.16444F));
            PanelPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.381588F));
            PanelPrincipal.Size = new System.Drawing.Size(338, 464);
            PanelPrincipal.TabIndex = 9;
            // 
            // PanelPassoAnterior_Proximo
            // 
            PanelPassoAnterior_Proximo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            PanelPassoAnterior_Proximo.ColumnCount = 2;
            PanelPassoAnterior_Proximo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            PanelPassoAnterior_Proximo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            PanelPassoAnterior_Proximo.Controls.Add(btnAnterior, 0, 0);
            PanelPassoAnterior_Proximo.Controls.Add(btnProximo, 1, 0);
            PanelPassoAnterior_Proximo.Dock = System.Windows.Forms.DockStyle.Fill;
            PanelPassoAnterior_Proximo.Location = new System.Drawing.Point(3, 45);
            PanelPassoAnterior_Proximo.Name = "PanelPassoAnterior_Proximo";
            PanelPassoAnterior_Proximo.RowCount = 1;
            PanelPassoAnterior_Proximo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            PanelPassoAnterior_Proximo.Size = new System.Drawing.Size(332, 36);
            PanelPassoAnterior_Proximo.TabIndex = 9;
            // 
            // btnAnterior
            // 
            btnAnterior.AutoSize = true;
            btnAnterior.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAnterior.Dock = System.Windows.Forms.DockStyle.Fill;
            btnAnterior.Location = new System.Drawing.Point(3, 3);
            btnAnterior.Name = "btnAnterior";
            btnAnterior.Size = new System.Drawing.Size(160, 30);
            btnAnterior.TabIndex = 6;
            btnAnterior.Tag = "Botao passar";
            btnAnterior.Text = "Passo Anterior";
            btnAnterior.UseVisualStyleBackColor = true;
            btnAnterior.Click += btnAnterior_Click;
            // 
            // btnProximo
            // 
            btnProximo.AutoSize = true;
            btnProximo.Cursor = System.Windows.Forms.Cursors.Hand;
            btnProximo.Dock = System.Windows.Forms.DockStyle.Fill;
            btnProximo.Location = new System.Drawing.Point(169, 3);
            btnProximo.Name = "btnProximo";
            btnProximo.Size = new System.Drawing.Size(160, 30);
            btnProximo.TabIndex = 5;
            btnProximo.Tag = "Botao passar";
            btnProximo.Text = "Proximo Passo";
            btnProximo.UseVisualStyleBackColor = true;
            btnProximo.Click += btnProximo_Click;
            // 
            // PanelBtns
            // 
            PanelBtns.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            PanelBtns.ColumnCount = 4;
            PanelBtns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            PanelBtns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            PanelBtns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            PanelBtns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            PanelBtns.Controls.Add(Btn2, 2, 0);
            PanelBtns.Controls.Add(Btn1, 1, 0);
            PanelBtns.Controls.Add(Btn0, 0, 0);
            PanelBtns.Controls.Add(txtPasso, 3, 0);
            PanelBtns.Dock = System.Windows.Forms.DockStyle.Fill;
            PanelBtns.Location = new System.Drawing.Point(3, 3);
            PanelBtns.Name = "PanelBtns";
            PanelBtns.RowCount = 1;
            PanelBtns.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            PanelBtns.Size = new System.Drawing.Size(332, 36);
            PanelBtns.TabIndex = 10;
            // 
            // Btn2
            // 
            Btn2.AutoSize = true;
            Btn2.Cursor = System.Windows.Forms.Cursors.Hand;
            Btn2.Dock = System.Windows.Forms.DockStyle.Fill;
            Btn2.Location = new System.Drawing.Point(200, 3);
            Btn2.Name = "Btn2";
            Btn2.Size = new System.Drawing.Size(92, 30);
            Btn2.TabIndex = 3;
            Btn2.Tag = "Botao de Categoria";
            Btn2.Text = "Botão 2";
            Btn2.UseVisualStyleBackColor = true;
            Btn2.Click += Btn_Click;
            // 
            // Btn1
            // 
            Btn1.AutoSize = true;
            Btn1.Cursor = System.Windows.Forms.Cursors.Hand;
            Btn1.Dock = System.Windows.Forms.DockStyle.Fill;
            Btn1.Location = new System.Drawing.Point(102, 3);
            Btn1.Name = "Btn1";
            Btn1.Size = new System.Drawing.Size(92, 30);
            Btn1.TabIndex = 1;
            Btn1.Tag = "Botao de Categoria";
            Btn1.Text = "Botão 1";
            Btn1.UseVisualStyleBackColor = true;
            Btn1.Click += Btn_Click;
            // 
            // Btn0
            // 
            Btn0.AutoSize = true;
            Btn0.Cursor = System.Windows.Forms.Cursors.Hand;
            Btn0.Dock = System.Windows.Forms.DockStyle.Fill;
            Btn0.Location = new System.Drawing.Point(3, 3);
            Btn0.Name = "Btn0";
            Btn0.Size = new System.Drawing.Size(93, 30);
            Btn0.TabIndex = 0;
            Btn0.Tag = "Botao de Categoria";
            Btn0.Text = "Botão 0";
            Btn0.UseVisualStyleBackColor = true;
            Btn0.Click += Btn_Click;
            // 
            // txtPasso
            // 
            txtPasso.AutoSize = true;
            txtPasso.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            txtPasso.Location = new System.Drawing.Point(298, 0);
            txtPasso.Name = "txtPasso";
            txtPasso.Size = new System.Drawing.Size(29, 20);
            txtPasso.TabIndex = 7;
            txtPasso.Tag = "Texto passo atual";
            txtPasso.Text = "?/?";
            // 
            // PassoAPasso
            // 
            ClientSize = new System.Drawing.Size(338, 464);
            ControlBox = false;
            Controls.Add(PanelPrincipal);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Location = new System.Drawing.Point(20, 10);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(263, 411);
            Name = "PassoAPasso";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Explicação sobre o Wifi";
            TopMost = true;
            FormClosing += PassoAPasso_FormClosing;
            ((System.ComponentModel.ISupportInitialize)picBox).EndInit();
            PanelPrincipal.ResumeLayout(false);
            PanelPrincipal.PerformLayout();
            PanelPassoAnterior_Proximo.ResumeLayout(false);
            PanelPassoAnterior_Proximo.PerformLayout();
            PanelBtns.ResumeLayout(false);
            PanelBtns.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.PictureBox picBox;
        public System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.TableLayoutPanel PanelPrincipal;
        public System.Windows.Forms.TableLayoutPanel PanelBtns;
        public System.Windows.Forms.Button Btn2;
        public System.Windows.Forms.Button Btn1;
        public System.Windows.Forms.Button Btn0;
        public System.Windows.Forms.TableLayoutPanel PanelPassoAnterior_Proximo;
        public System.Windows.Forms.Button btnAnterior;
        public System.Windows.Forms.Button btnProximo;
        public System.Windows.Forms.Label txtPasso;
    }
}

