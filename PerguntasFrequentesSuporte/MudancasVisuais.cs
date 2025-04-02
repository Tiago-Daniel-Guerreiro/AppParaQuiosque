using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using BibliotecaAuxiliarForms.UI.MudancasVisuais;
using BibliotecaAuxiliarForms.Ficheiros;
using BibliotecaAuxiliarForms.Utilidades.Forms;
using BibliotecaAuxiliarForms.Utilidades.Matematica;

namespace PerguntasFrequentesSuporte
{
    public static class Configuracao_AtualizacaoForms
    {
        private static AppConfig config = AcederConfig.ConfigAtual;
        public static void AplicarConfiguracoesAoForm(Form form)
        {
            if (form == null)
                return;
            TemaVisual TemaAtual = config.VisualAplicacao.Tema[config.IndiceTemaAtual];


            form.Text = config.VisualAplicacao.Titulo;  // Aplica o título da janela
            form.Opacity = config.VisualAplicacao.OpacidadeDoPrograma; // Define a opacidade
            form.ShowInTaskbar = config.VisualAplicacao.AparecerNaBarraDeTarefas; // Define se aparece na barra de tarefas

            if (!config.VisualAplicacao.JanelasMoveis && !config.VisualAplicacao.JanelasRedimensionaveis)
                form.FormBorderStyle = FormBorderStyle.None;  // Se a janela não pode ser movida nem redimensionada, remove a borda completamente.
            else if (!config.VisualAplicacao.JanelasRedimensionaveis)
                form.FormBorderStyle = FormBorderStyle.FixedSingle;  // Se só não pode ser redimensionada, mantém a barra de título, mas bloqueia o redimensionamento.
            else
                form.FormBorderStyle = FormBorderStyle.Sizable; // Se pode ser redimensionada, mantém o estilo padrão de janelas redimensionáveis.

            if (!config.VisualAplicacao.JanelasMoveis)  // Define se a janela pode ser movida
            {
                form.ControlBox = false;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
            }
            if (config.VisualAplicacao.UsaIcone) // Aplica ícone se configurado
            {
                if (File.Exists(config.VisualAplicacao.CaminhoIcone))
                    form.Icon = new Icon(config.VisualAplicacao.CaminhoIcone);
            }

            foreach (Control control in form.Controls)
            {
                if (control is Button botao)
                {
                    string tipoBotao = (string)botao.Tag;
                    VisualBotoesPorJanela aparenciaBotao = new();
                    Dictionary<string, VisualBotoesPorJanela> botoes = null;
                    bool temaEncontrado = false;

                    if (!string.IsNullOrWhiteSpace(tipoBotao))
                    {
                        if (TemaAtual.VisualBotoes.ContainsKey(form.Name))
                        {
                            botoes = TemaAtual.VisualBotoes[form.Name];

                            if (botoes.ContainsKey(tipoBotao))
                            {
                                aparenciaBotao = botoes[tipoBotao];
                                temaEncontrado = true;
                            }
                        }
                    }

                    if (temaEncontrado)
                        AplicarEstiloAoBotao(botao, aparenciaBotao);
                    else if (botoes != null && botoes.ContainsKey("Default"))
                        AplicarEstiloAoBotao(botao, botoes["Default"]);
                }
            }

            if (form.Name == "Menu")  // Define a cor de fundo do form
            {
                form.Size = config.VisualAplicacao.TamanhoJanelas.ObterTamanho("Menu");
                form.BackColor = TemaAtual.VisualDoMenu.CorFundoMenu;
                form.ControlBox = false;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.FormBorderStyle = FormBorderStyle.None;
                if (TemaAtual.VisualDoMenu.MenuTransparente)
                    form.BackColor = form.TransparencyKey;

                if (TemaAtual.VisualDoMenu.UsarImagemDeFundoMenu)
                {
                    if (File.Exists(TemaAtual.VisualDoMenu.CaminhoImagemDeFundoMenu))
                    {
                        form.BackgroundImage = Image.FromFile(TemaAtual.VisualDoMenu.CaminhoImagemDeFundoMenu);
                        form.BackgroundImageLayout = ImageLayout.Stretch; // Ajusta a imagem ao tamanho do formulário
                    }
                }

                if (form is Menu menu)
                    AplicarAjustesVisuaisMenu(menu);
            }
            else
            {
                form.Size = config.VisualAplicacao.TamanhoJanelas.ObterTamanho(form.Name);
                form.BackColor = TemaAtual.CorFundo;

                form.ArredondarBorda(0,
                 TemaAtual.TamanhoDoContraste,
                 TemaAtual.CorDoContraste);

            }

            if (form is PassoAPasso passoAPasso)
                AplicarConfiguracoesPassoAPasso(passoAPasso);
        }
        public static void AplicarConfiguracoesPassoAPasso(PassoAPasso passoAPasso)
        {
            if (passoAPasso == null || config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso == null)
            {
                MessageBox.Show("Erro: Configuração do Passo a Passo não está disponível.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = 0;
            if (passoAPasso.Tag != null && int.TryParse(passoAPasso.Tag.ToString(), out int resultado))
                id = resultado;

            if (id < 0 || id >= config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso.Count)
            {
                MessageBox.Show($"Erro: O índice {id} não existe em Config_PassoAPasso.Dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passoAPasso.Close();
                return;
            }

            string caminhoImagens = Path.Combine(Ficheiros.Caminho, "ImagensPassoAPasso", id.ToString());

            if (!Directory.Exists(caminhoImagens))
            {
                MessageBox.Show($"A pasta {caminhoImagens} não existe. Por favor, carregue as imagens necessárias para essa pasta.",
                    "Pasta não encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passoAPasso.Close();
                return;
            }

            // Obtém a configuração específica
            ConfigPassoAPasso ConfigPassoAPassoAtual = config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso[id];
            passoAPasso.Text = ConfigPassoAPassoAtual.Titulo;

            Image[][] ListaImagens = new Image[ConfigPassoAPassoAtual.Imagens.Length][];

            // Itera sobre os botões ordenados por número
            foreach (Button btn in Utilidades_Forms.ObterBotoesOrdenadosDeForms(passoAPasso))
            {
                int? numero = OperacoesComuns.ExtrairNumeroFinal(btn.Name); // Extrai o número do nome do botão

                if (numero.HasValue && ConfigPassoAPassoAtual.Imagens.Length > numero.Value)
                {
                    FicheiroImagem ficheiroImagem = ConfigPassoAPassoAtual.Imagens[numero.Value];
                    btn.Text = ficheiroImagem.NomeCompletoFicheiro; // Define o texto do botão com o nome completo

                    // Obtém as imagens com base no caminho da pasta das imagens + o nome do ficheiro que contem o formato
                    string[] arquivos = Directory.GetFiles(
                        caminhoImagens,
                        ficheiroImagem.NomeCompletoFicheiro );

                    ListaImagens[numero.Value] = GestorFicheiros.ObterImagensDeFicheiros(arquivos, new[] { ficheiroImagem.Imagem }); 
                }
            }

            // Atualiza a ListaTotal, se necessário
            if (passoAPasso.ListaTotal == null || passoAPasso.ListaTotal.Length != ListaImagens.Length)
            {
                passoAPasso.ListaTotal = ListaImagens;
            }
            else
            {
                for (int i = 0; i < ListaImagens.Length; i++)
                {
                    // Verifica se o tamanho das sublistas está sincronizado
                    if (passoAPasso.ListaTotal[i]?.Length != ListaImagens[i]?.Length)
                    {
                        passoAPasso.ListaTotal = ListaImagens;
                        break;
                    }
                }
            }

            AtualizarInterfacePassoAPasso(passoAPasso);  // Atualiza a interface
        }
        private static void AtualizarInterfacePassoAPasso(PassoAPasso passoAPasso)
        {
            int quantidadeImagens = passoAPasso.ListaTotal.Length, colunasAtivas = 0;
            bool precisaReconstruir = false, temImagens = false;

            foreach (Button btn in Utilidades_Forms.ObterBotoesOrdenadosDeForms(passoAPasso))
            {
                int? numero = OperacoesComuns.ExtrairNumeroFinal(btn.Name);
                if (numero.HasValue)
                {
                    if (quantidadeImagens > numero.Value)
                    {
                        if (passoAPasso.ListaTotal[numero.Value] != null)
                        {
                            if (passoAPasso.ListaTotal[numero.Value].Length > 0)
                            {
                                temImagens = true;
                            }
                        }
                    }

                    if (btn.Visible != temImagens) // Só marca reconstrução se houver mudança na visibilidade
                        precisaReconstruir = true;

                    btn.Visible = temImagens;

                    if (temImagens)
                        btn.Text = config.ConfiguracaoAplicacao.ConfiguracoesPassoAPasso[Convert.ToInt32(passoAPasso.Tag)].Imagens[numero.Value].NomeBotao;
                }
            }

            if (precisaReconstruir)  // Reconfigura o layout do painel de botões
            {
                passoAPasso.PanelBtns.Controls.Clear();

                foreach (Button btn in Utilidades_Forms.ObterBotoesOrdenadosDeForms(passoAPasso))
                {
                    if (btn.Visible)
                        passoAPasso.PanelBtns.Controls.Add(btn, colunasAtivas++, 0);
                }

                passoAPasso.PanelBtns.Controls.Add(passoAPasso.txtPasso, colunasAtivas++, 0);
                passoAPasso.PanelBtns.ColumnCount = colunasAtivas;

                passoAPasso.PanelBtns.ColumnStyles.Clear();

                for (int i = 0; i < colunasAtivas - 1; i++)
                {
                    passoAPasso.PanelBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / colunasAtivas));
                }
                passoAPasso.PanelBtns.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            passoAPasso.AtualizarImagem();
        }
        public static void AplicarAjustesVisuaisMenu(Menu menu)
        {
            if (menu == null)
                return;

            AplicarAjustesVisuaisBtnSair_EsconderMostrar(menu);

            ConfigGeral ConfigAtualMenu = config.ConfiguracaoAplicacao;

            // Obter os botões ordenados com base no número final
            Button[] arrayBotoes = Utilidades_Forms.ObterBotoesOrdenadosDeForms(menu);

            int linhaAtual = 0, colunaAtual = 0;
            int espacamento = arrayBotoes[0].Margin.Left * 2;
            int posX = espacamento, posY = espacamento;

            if (ConfigAtualMenu.QuantidadeBotoes == 0 || ConfigAtualMenu.BotoesPorLinha == 0)
            {
                foreach (Button btn in arrayBotoes)
                {
                    btn.Visible = false;
                    btn.Enabled = false;
                }
                return;
            }

            int numeroLinhas = ConfigAtualMenu.QuantidadeBotoes / ConfigAtualMenu.BotoesPorLinha;
            if (ConfigAtualMenu.QuantidadeBotoes % ConfigAtualMenu.BotoesPorLinha != 0)
                numeroLinhas++;

            int larguraDisponivel = (menu.ClientSize.Width * ConfigAtualMenu.PercentagemEcraParaBotoesLargura) / 100;
            int alturaDisponivel = (menu.ClientSize.Height * ConfigAtualMenu.PercentagemEcraParaBotoesAltura) / 100;

            int larguraBotao = (larguraDisponivel - (ConfigAtualMenu.BotoesPorLinha + 1) * espacamento) / ConfigAtualMenu.BotoesPorLinha;
            int alturaBotao = (alturaDisponivel - (numeroLinhas + 1) * espacamento) / numeroLinhas;

            for (int i = 0; i < ConfigAtualMenu.QuantidadeBotoes && i < arrayBotoes.Length; i++)
            {
                if (i >= ConfigAtualMenu.ConfiguracoesBotoesMenu.Count)
                {
                    arrayBotoes[i].Visible = false;
                    arrayBotoes[i].Enabled = false;
                    continue;
                }

                arrayBotoes[i].SendToBack();
                arrayBotoes[i].Size = new Size(larguraBotao, alturaBotao);
                arrayBotoes[i].Location = new Point(posX, posY);
                arrayBotoes[i].Visible = true;
                arrayBotoes[i].Enabled = true;
                arrayBotoes[i].Text = ConfigAtualMenu.ConfiguracoesBotoesMenu[i].Nome;

                colunaAtual++;
                if (colunaAtual >= ConfigAtualMenu.BotoesPorLinha)
                {
                    colunaAtual = 0;
                    linhaAtual++;
                    posY += alturaBotao + espacamento;
                    posX = espacamento;
                }
                else
                {
                    posX += larguraBotao + espacamento;
                }
            }

            // Esconde os botões fora da quantidade definida
            for (int i = ConfigAtualMenu.QuantidadeBotoes; i < arrayBotoes.Length; i++)
            {
                arrayBotoes[i].Visible = false;
                arrayBotoes[i].Enabled = false;
            }

            if (config.VisualAplicacao.ProgramaNoTrayMenu)
                _ = new TrayIcon(menu);

            menu.AutoScroll = false;
        }
        public static void AplicarAjustesVisuaisBtnSair_EsconderMostrar(Menu menu)
        {
            if (menu == null)
                return;

            int margem = menu.BtnMudarEsconder_MostrarMenu.Margin.Left; // Ou um valor fixo, tipo 30

            Size tamanho = config.VisualAplicacao.TamanhoJanelas.ObterTamanho("Botoes_Sair_EsconderMostrar");

            menu.BtnSair.Size = tamanho;
            menu.BtnMudarEsconder_MostrarMenu.Size = tamanho;

            bool esquerda = config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.PosicaoEsquerda;
            bool superior = config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.PosicaoSuperior;

            ColocarBotaoNoCanto(menu.BtnMudarEsconder_MostrarMenu, superior, esquerda, margem);
            ColocarBotaoNoCanto(menu.BtnSair, superior, !esquerda, margem);

            menu.BtnMudarEsconder_MostrarMenu.UseVisualStyleBackColor = false;
            menu.BtnSair.UseVisualStyleBackColor = false;

            menu.BtnMudarEsconder_MostrarMenu.Visible = config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.BotaoEsconder_MostrarEsconderVisivel;
            menu.BtnSair.Visible = config.ConfiguracaoAplicacao.ConfiguracoesSair_Mostrar.BtnSairVisivel;
        }
        private static void ColocarBotaoNoCanto(Button botao, bool superior, bool esquerda, int margem)
        {
            if (botao.Parent == null)
                return; 

            Control container = botao.Parent;

            int x, y;

            if (esquerda)
                x = margem;
            else
                x = container.Width - botao.Width - margem;

            if (superior)
                y = margem;
            else
                y = container.Height - botao.Height - margem;

            botao.Location = new Point(x, y);
        }
        public static void AtualizarTudo()
        {
            List<Form> formularios = new();
            foreach (Form form in Application.OpenForms)
            {
                formularios.Add(form);
            }

            foreach (Form form in formularios)
            {
                AplicarConfiguracoesAoForm(form);
            }
        }
        private static void AplicarEstiloAoBotao(Button botao, VisualBotoesPorJanela aparencia)
        {
            botao.BackColor = aparencia.CorFundo;
            botao.ForeColor = aparencia.CorTexto;
            botao.Font = aparencia.Fonte;
            botao.FlatStyle = FlatStyle.Flat;
            botao.UseVisualStyleBackColor = false;
            botao.TabStop = false;
            botao.Margin = new Padding(
                aparencia.Margem,
                aparencia.Margem,
                aparencia.Margem,
                aparencia.Margem);

            botao.ArredondarBorda(
                aparencia.IntensidadeArredondarBorda,
                aparencia.TamanhoContrasteBorda,
                aparencia.CorContrasteBorda
            );
        }

    }
}
