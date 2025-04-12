using BibliotecaAuxiliarForms.Utilidades.Forms;
using BibliotecaAuxiliarForms.Utilidades.Matematica;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PerguntasFrequentesSuporte
{
    public partial class PassoAPasso : Form
    {
        public PassoAPasso()
        {
            InitializeComponent();
        }
        public Image[][] ListaTotal;// Array 0=Btn / 1=Passo
        public int CategoriaAtual = new();
        public int PassoAtual = new();

        private void btnProximo_Click(object sender, EventArgs e)
        {
            if (!VerificarImagensExistentes())
                return;

            if (PassoAtual + 1 < ListaTotal[CategoriaAtual].Length)
                PassoAtual++;
            AtualizarImagem();
        }
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (!VerificarImagensExistentes())
                return;

            if (CategoriaAtual > 0)
                PassoAtual--;
            AtualizarImagem();
        }
        private void Reiniciar(int? dispositivo)
        {
            if (!VerificarImagensExistentes())
                return;

            if (dispositivo.HasValue) // Verifica se dispositivo contém um valor
            {
                CategoriaAtual = dispositivo.Value; // Usa .Value para obter o int
                PassoAtual = 0;
                AtualizarImagem();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        public void AtualizarImagem()
        {
            if (!VerificarImagensExistentes())
                return;

            int total = ListaTotal[CategoriaAtual].Length;

            bool mostrarControlo = total > 1;

            btnAnterior.Visible = mostrarControlo;
            btnProximo.Visible = mostrarControlo;
            txtPasso.Visible = mostrarControlo;

            btnAnterior.Enabled = PassoAtual > 0;
            btnProximo.Enabled = PassoAtual < total - 1;

            picBox.Image = ListaTotal[CategoriaAtual][PassoAtual];

            if (mostrarControlo)
                txtPasso.Text = $"{PassoAtual + 1}/{total}";
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (!VerificarImagensExistentes())
                return;

            if (sender is Button btn && btn.Name.Length > 0)
                Reiniciar(OperacoesComuns.ExtrairNumeroFinal(btn.Name));
        }
        private bool VerificarImagensExistentes()
        {
            if (ListaTotal == null || CategoriaAtual < 0 || CategoriaAtual >= ListaTotal.Length)
            {
                MensagemUtility.MostrarMensagem("Não foram encontradas imagens.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                return false;
            }

            if (ListaTotal[CategoriaAtual] == null || ListaTotal[CategoriaAtual].Length == 0)
            {
                MensagemUtility.MostrarMensagem("Não existem imagens para esta categoria.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                return false;
            }

            // Remover imagens nulas com loop simples
            List<Image> imagensValidas = new List<Image>();
            for (int i = 0; i < ListaTotal[CategoriaAtual].Length; i++)
            {
                if (ListaTotal[CategoriaAtual][i] != null)
                    imagensValidas.Add(ListaTotal[CategoriaAtual][i]);
            }

            if (imagensValidas.Count == 0)
            {
                MensagemUtility.MostrarMensagem("Todas as imagens desta categoria são inválidas.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                return false;
            }

            ListaTotal[CategoriaAtual] = imagensValidas.ToArray();
            return true;
        }
        private void PassoAPasso_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;  // Impede o fecho
                Hide();                 // Apenas esconde
            }
        }
    }
}
