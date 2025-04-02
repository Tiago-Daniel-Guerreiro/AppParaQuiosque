using System;
using System.Drawing;
using System.Windows.Forms;
using BibliotecaAuxiliarForms.Utilidades.Matematica;
using BibliotecaAuxiliarForms.Utilidades.Forms;

namespace PerguntasFrequentesSuporte
{
    public partial class PassoAPasso : Form
    {
        public PassoAPasso()
        {
            InitializeComponent();
        }
        public Image[][] ListaTotal;// Array 0=Btn / 1=Passo
        public int CategoriaAtual = new int();
        public int PassoAtual = new int(); 

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
        public void AtualizarImagem() // Atualiza a imagem exibida
        {
            bool temMultiplasImagens = ListaTotal[CategoriaAtual].Length > 1;
            btnAnterior.Enabled = !(temMultiplasImagens && PassoAtual > 0);
            btnProximo.Enabled = !(temMultiplasImagens && PassoAtual < ListaTotal[CategoriaAtual].Length - 1);

            picBox.Image = ListaTotal[CategoriaAtual][PassoAtual];
            txtPasso.Text = $"{PassoAtual+1}/{ListaTotal[CategoriaAtual].Length}";
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
            if (ListaTotal == null)
                return false;

            MensagemUtility.MostrarMensagem("Não foram encrontradas imagens.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
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
