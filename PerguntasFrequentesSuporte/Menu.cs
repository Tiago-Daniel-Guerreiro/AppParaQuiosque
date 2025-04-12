using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using BibliotecaAuxiliarForms.Utilidades.Matematica;
using BibliotecaAuxiliarForms.Utilidades;

namespace PerguntasFrequentesSuporte
{
    public partial class Menu : Form
    {
        private readonly AppConfig config;
        private readonly Configuracoes configForm = new();
        private readonly List<PassoAPasso> PassoForms = new();

        public Menu()
        {
            Visible = false;
            InitializeComponent();

            config = AcederConfig.ConfigAtual;  // Inicialização garantida
            // Carregar e iniciar o formulário de configurações
            configForm.Hide();
            configForm.TopLevel = false;
            Controls.Add(configForm);

            for (int i = 0; i < config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso.Count; i++)
            {
                PassoAPasso passoForm = new();
                passoForm.Tag = i;  // Atribui primeiro
                Configuracao_AtualizacaoForms.AplicarConfiguracoesPassoAPasso(passoForm); // Só depois chama o setup
                passoForm.Hide();
                passoForm.TopLevel = false;
                PassoForms.Add(passoForm);
                Controls.Add(passoForm);
            }

            Tag = "BotoesVisiveis";
        }
        private void Menu_Load(object sender, EventArgs e)
        {
            Configuracao_AtualizacaoForms.AtualizarTudo();
            Visible = true;
        }

        public void BtnMudarEsconder_MostrarMenu_Click(object sender, EventArgs e)
        {
            MudarEstadoBtnMostrar_Esconder();
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            GestorEncerramento.ExecutarEncerramento();
        }
        public void btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                int? NumBotao = OperacoesComuns.ExtrairNumeroFinal(btn.Name);
                if (NumBotao.HasValue && NumBotao.Value < config.ConfiguracaoAplicacao.ConfiguracoesBotoesMenu.Count)
                {
                    FuncaoBotaoMenu funcao = config.ConfiguracaoAplicacao.ConfiguracoesBotoesMenu[NumBotao.Value];
                    AcaoDoBotao(funcao);
                }
            }
        }
        public void AcaoDoBotao(FuncaoBotaoMenu funcao)
        {
            bool AbrirForm = funcao.Tipo.ToUpper() == "FORMS";
            MudarEstadoBtnMostrar_Esconder(AbrirForm);

            switch (funcao.Tipo.ToUpper())
            {
                case "PDF":
                    string caminhoPDF = Path.Combine(Ficheiros.Caminho, "Documentos", funcao.DocumentoPDF?.NomeFicheiro ?? "");
                    AbrirPDF(caminhoPDF);
                    break;

                case "LINK":
                    AbrirSite(funcao.Diretorio_Link);
                    break;

                case "FORMS":
                    AbrirForms(funcao.Diretorio_Link);
                    break;
            }
        }
        private void AbrirPDF(string caminhoPDF)
        {
            if (!File.Exists(caminhoPDF))
            {
                MessageBox.Show($"O documento PDF {caminhoPDF} não existe.", "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = caminhoPDF,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao abrir o documento PDF: {ex.Message}");
            }
        }
        private void AbrirSite(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c start {url}",
                CreateNoWindow = true
            });
        }
        private void OcultarBotoesMenu()
        {
            foreach (Control control in Controls)
            {
                if (control is Button)
                {
                    if (control != BtnMudarEsconder_MostrarMenu && control != BtnSair)  // Mantém apenas os botões de alternância e saída visíveis
                        control.Visible = false;
                }
            }
        }
        public void MostrarBotoesMenu()
        {
            foreach (Control control in Controls)
            {
                if (control is Button btn)
                {
                    btn.Visible = btn.Enabled;
                }
            }
        }
        public void AbrirForms(string Diretorio_Link)
        {
            if (Diretorio_Link.ToUpper() == "CONFIG")
                MostrarFormConfiguracoes();
            else if (Diretorio_Link.ToUpper() == "FORMULARIO")
                MostrarFormConfiguracoes();  //AbrirFormsNoMenu(); /// por Fazer formulario
            else
            {
                // Divide a string com base nos caracteres "[" e "]"
                string[] partes = Diretorio_Link.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                int indice;
                if (partes.Length > 1 && partes[0].ToUpper() == "PASSOAPASSO")
                    if (int.TryParse(partes[1], out indice))
                        MostrarPassoAPasso(indice);  // O índice será a parte dentro de [ ]
            }
        }
        public void MostrarFormConfiguracoes()
        {
            configForm.Show();
            configForm.BringToFront();
        }
        public void MostrarPassoAPasso(int indice)
        {
            if (indice >= 0 && indice < PassoForms.Count)
            {
                PassoForms[indice].Show();
                PassoForms[indice].BringToFront();
            }
        }
        public void MudarEstadoBtnMostrar_Esconder(bool AbrirForms = false)
        {
            if ((string)Tag == "BotoesVisiveis")  // Se o estado for "BotoesVisiveis", ocultamos os botões e atualizamos o estado
            {
                OcultarBotoesMenu();

                if (AbrirForms)
                    Tag = "FormsAberto";
                else
                    Tag = "BotoesEscondidos";
            }
            else  //Se os botões não estiverem visiveis vai mostra-los
            {
                MostrarBotoesMenu();
                Tag = "BotoesVisiveis";
            }

            // Atualiza o texto do botão com base no estado
            BtnMudarEsconder_MostrarMenu.Text = config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.TextoBotaoMostrarEsconder[(string)Tag];
        }
        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;  // Impede o fecho
                Hide();                 // Apenas esconde
            }
        }
    }
    public class TrayIcon : BaseTrayIcon
    {
        private readonly Menu menu;

        public TrayIcon(Menu menu)
        {
            if(menu ==null)
                throw new ArgumentNullException(nameof(menu));

            this.menu = menu;

            // Criação fixa da estrutura do menu
            CriarEstruturaMenuInicial(out TreeNode estruturaMenu);
            CriarMapaDeAcoes(out Dictionary<string, Action> acoes);

            IniciarTrayIcon
            (
                estruturaMenu: estruturaMenu,
                acoes: acoes,
                icone: Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                tooltip: "App para Quiosque",
                cores: new CoresDoTrayIcon(corFundo:Color.White, corTexto: Color.Black)
            );
        }
        private void CriarEstruturaMenuInicial(out TreeNode raiz)
        {
            raiz = new();

            // Submenu com botões existentes no Menu
            var botoesNode = new TreeNode("Botões Abertos");

            foreach (Control control in menu.Controls)
            {
                if (control is Button btn && btn.Enabled)
                {
                    if(btn != menu.BtnSair && btn != menu.BtnMudarEsconder_MostrarMenu)
                        botoesNode.Nodes.Add(new TreeNode(btn.Text) { Tag = btn.Text });
                }
            }

            if (botoesNode.Nodes.Count > 0)
                raiz.Nodes.Add(botoesNode);

            raiz.Nodes.Add(new TreeNode("Mostrar/Esconder") { Tag = "mostrar_esconder" });
            raiz.Nodes.Add(new TreeNode("Configurações") { Tag = "config" });
            raiz.Nodes.Add(new TreeNode("Reiniciar") { Tag = "reiniciar" });
            raiz.Nodes.Add(new TreeNode("Sair") { Tag = "sair" });
        }
        private void CriarMapaDeAcoes(out Dictionary<string, Action> acoes)
        {
            acoes = new()
            {
                { "mostrar_esconder", MostrarEsconder },
                { "config", AbrirConfiguracoes },
                { "reiniciar", ReiniciarApp },
                { "sair", Sair }
            };

            // Adiciona ações para os botões com base no Text
            foreach (Control control in menu.Controls)
            {
                if (control is Button btn && btn != menu.BtnSair && btn != menu.BtnMudarEsconder_MostrarMenu)
                {
                    if (!acoes.ContainsKey(btn.Text))
                        acoes[btn.Text] = () => menu.btn_Click(btn, EventArgs.Empty);
                }
            }
        }
        protected override void AoDuploClique() => MostrarEsconder();

        private void MostrarEsconder()
        {
            if (!menu.Visible)
            {
                menu.Show();
                menu.BringToFront();
                menu.Activate();
            }
            else
                menu.Hide();
        }
        private void AbrirConfiguracoes()
        {
            if (!menu.Visible)
                MostrarEsconder();

            menu.AbrirForms("CONFIG");
        }
        private void ReiniciarApp()
        {
            FecharTrayIcon(); // Limpa tudo corretamente
            GestorEncerramento.ExecutarReinicio();
        }
        private void Sair()
        {
            FecharTrayIcon(); // Limpa tudo corretamente
            GestorEncerramento.ExecutarEncerramento();
        }
    }
}