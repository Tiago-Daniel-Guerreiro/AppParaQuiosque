using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Linq;
using BibliotecaAuxiliarForms.Utilidades.Matematica;
using System.Text.Json.Serialization;

// Talvez várias abas de botões
// Por colocar segurança para fechar Programa e abrir Config 

namespace PerguntasFrequentesSuporte
{
    //---------------------------
    // Tamanhos iniciais para janelas
    //---------------------------
    public class TamanhoInicialJanelas
    {
        [Description("Dicionário que armazena os tamanhos das janelas")]
        public Dictionary<string, Size> Tamanhos { get; set; } = new Dictionary<string, Size>
        {
            { "Default", new Size(400, 300) },
            { "Botoes_Sair_EsconderMostrar", new Size(240, 120) },
            { "PassoAPasso", new Size(390, 540) },
            { "Configuracoes", new Size(800, 550) },
            { "Menu", Screen.PrimaryScreen.Bounds.Size } // Usa o tamanho total do ecrã
        };
        public TamanhoInicialJanelas()  // Construtor: valida com os tamanhos mínimos definidos
        {
            Validate();
        }
        public void Validate()
        {
            if (Tamanhos == null)
                Tamanhos = new Dictionary<string, Size>();

            Dictionary<string, Size> MinSizes = new()
            {
                { "Default", new Size(200, 150) },
                { "Configuracoes", new Size(600, 400) },
                { "Menu", new Size(800, 600) },
                { "PassoAPasso", new Size(350, 500) }
            };

            Size maxSize = new(1920, 1080);

            foreach (string key in Tamanhos.Keys.ToList())
            {
                Size tamanhoAtual = Tamanhos[key];
                Size minSize = MinSizes.ContainsKey(key) ? MinSizes[key] : MinSizes["Default"];

                int larguraCorrigida = Math.Max(minSize.Width, Math.Min(tamanhoAtual.Width, maxSize.Width));
                int alturaCorrigida = Math.Max(minSize.Height, Math.Min(tamanhoAtual.Height, maxSize.Height));

                Tamanhos[key] = new Size(larguraCorrigida, alturaCorrigida);
            }
        }
        public Size ObterTamanho(string nomeJanela)
        {
            if (nomeJanela == "Menu")
                return Screen.PrimaryScreen.Bounds.Size;

            if (Tamanhos.ContainsKey(nomeJanela))
                return Tamanhos[nomeJanela];

            return Tamanhos["Default"]; // Retorna o tamanho padrão
        }
    }
    //---------------------------
    // Aparência dos Controlo (por exemplo, botões)
    //---------------------------
    public class VisualBotoesPorJanela
    {
        [Description("Fonte associada ao Forms.  (O atributo pode não ser usado em certos casos)")]
        public Font Fonte { get; set; } = SystemFonts.DefaultFont;

        [Description("Margem que o Control vai ter de espaçamento. (Pode não funcionar em certos casos)")]
        public int Margem { get; set; } = 0;
        [Description("Intensidade das curvas. (Pode não funcionar em certos casos)" +
            "\n\n" +
            "Opções:" +
            "\n\t0 = Sem arredondamenton " +
            "\n\t1 = Muito ligeiro" +
            "\n\t2 = Ligeiramente curvo" +
            "\n\t3 = Curvatura moderada" +
            "\n\t4 = Curvas aceitáveis" +
            "\n\t5 = Curvas visíveis" +
            "\n\t6 = Muito arredondado" +
            "\n\t7 = Circular se for quadrado" +
            "\n\t8 = Forçar círculo (pode cortar o texto)")]
        public int IntensidadeArredondarBorda { get; set; } = 0;
        [Description("Tamanho que o contraste vai ter á volta do controlo. (O atributo pode não ser usado em certos casos)")]
        public int TamanhoContrasteBorda { get; set; } = 2;
        [Description("Cor que o contraste vai ter á volta do controlo, se for igual á cor de fundo não se notará,  normalmente é igual á cor do texto. (O atributo pode não ser usado em certos casos)")]
        public Color CorContrasteBorda { get; set; } = Color.Black;
        [Description("Cor que o Texto do controlo, vai ter, não deve ser igual á cor de fundo.  (O atributo pode não ser usado em certos casos)")]
        public Color CorTexto { get; set; } = Color.Black;
        [Description("Cor que o fundo do controlo vai ter, não deve ser igual á cor do texto.  (O atributo pode não ser usado em certos casos)")]
        public Color CorFundo { get; set; } = Color.White;
        public void Validate()
        {
            if (Fonte == null)
                Fonte = SystemFonts.DefaultFont;

            if (Fonte.Size < 6 || Fonte.Size > 100)
                Fonte = new Font(Fonte.FontFamily, SystemFonts.DefaultFont.Size, Fonte.Style);

            Margem = Math.Max(0, Math.Min(Margem, 80));
            TamanhoContrasteBorda = Math.Max(0, Math.Min(TamanhoContrasteBorda, 200));
            IntensidadeArredondarBorda = Math.Max(0, Math.Min(IntensidadeArredondarBorda, 8));

            if (CorFundo.IsEmpty)
                CorFundo = Color.White;
            if (CorTexto.IsEmpty)
                CorTexto = Color.Black;
            if (CorContrasteBorda.IsEmpty)
                CorContrasteBorda = Color.Black;

            if (CorFundo == CorTexto)
                CorTexto = (CorFundo == Color.White) ? Color.Black : Color.White;
        }
    }
    //---------------------------
    // Aparência geral (ex.: temas)
    //---------------------------
    public class TemaVisual
    {
        [Description("Nome do Tema.")]
        public string NomeTema { get; set; } = "Default";
        [Description("Indica a cor de fundo dos forms")]
        public Color CorFundo { get; set; } = Color.White;
        [Description("Indica a cor que o contraste/borda dos forms")]
        public Color CorDoContraste { get; set; } = Color.Black;
        [Description("Indica o tamanho do contraste/borda para os forms, coloque uma cor diferente do fundo, caso queria que não aparesa é só colocar tamanho 0")]
        public int TamanhoDoContraste { get; set; } = 3;
        [Description("Define a aparencia do menu, se é transparente, se tem imagem de fundo, etc")]
        public TemaMenu VisualDoMenu = new();
        [Description("Cada form pode ter vários tipos de botões, e vários botões podem compartilhar a mesma aparência.")]
        public Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> VisualBotoes { get; set; } = new();
        public TemaVisual()
        {
            VisualDoMenu = new TemaMenu();
            VisualDoMenu.Validate();

            VisualBotoes = new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>
            {
                { "Default", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Default", new VisualBotoesPorJanela() }
                    }
                },
                { "Configuracoes", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Escolha perfil", new VisualBotoesPorJanela() },
                        { "lista detalhes", new VisualBotoesPorJanela() },
                        { "Arvore Configs", new VisualBotoesPorJanela() },
                        { "Botao de Salvar/Sair", new VisualBotoesPorJanela() }
                    }
                },
                { "Menu", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Botao de Acao", new VisualBotoesPorJanela() },
                        { "Botao de Sair ou Esconder/Mostrar", new VisualBotoesPorJanela() }
                    }
                },
                { "PassoAPasso", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Botao de Categoria", new VisualBotoesPorJanela() },
                        { "Botao Sair", new VisualBotoesPorJanela() },
                        { "Botao passar", new VisualBotoesPorJanela() },
                        { "Texto passo atual", new VisualBotoesPorJanela() }
                    }
                }
            };
        }
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(NomeTema))
                NomeTema = "Default";

            if (CorFundo.IsEmpty)
                CorFundo = Color.White;

            if (CorDoContraste.IsEmpty)
                CorDoContraste = Color.Black;

            TamanhoDoContraste = Math.Max(0, Math.Min(TamanhoDoContraste, 6));

            if (CorFundo == CorDoContraste)
                CorDoContraste = (CorFundo == Color.White) ? Color.Black : Color.White;

            if (VisualDoMenu == null)
                VisualDoMenu = new TemaMenu();
            VisualDoMenu.Validate();

            if (VisualBotoes == null)
                VisualBotoes = new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>();

            string[] formsPermitidos = ConfiguracoesHelper.ObterForms();

            List<string> formsParaRemover = new();
            foreach (var form in VisualBotoes.Keys)
            {
                if (!formsPermitidos.Contains(form))
                    formsParaRemover.Add(form);
            }

            foreach (var form in formsParaRemover)
                VisualBotoes.Remove(form);

            foreach (var form in formsPermitidos)
            {
                if (!VisualBotoes.ContainsKey(form))
                    VisualBotoes[form] = new Dictionary<string, VisualBotoesPorJanela>();

                string[] botoesPermitidos = ConfiguracoesHelper.ObterBotoes(form);

                List<string> botoesParaRemover = new();
                foreach (var botao in VisualBotoes[form].Keys)
                {
                    if (!botoesPermitidos.Contains(botao))
                        botoesParaRemover.Add(botao);
                }

                foreach (var botao in botoesParaRemover)
                    VisualBotoes[form].Remove(botao);

                foreach (var botao in botoesPermitidos)
                {
                    if (!VisualBotoes[form].ContainsKey(botao))
                        VisualBotoes[form][botao] = new VisualBotoesPorJanela();
                }
            }

            foreach (var form in VisualBotoes.Values)
            {
                if (form == null)
                    continue;

                foreach (var botao in form.Values)
                {
                    if (botao != null)
                        botao.Validate();
                }
            }
        }
        public VisualBotoesPorJanela ObterVisualDoBotao(Form form, string tipoBotao)
        {
            if (form == null || string.IsNullOrWhiteSpace(tipoBotao))
                return new VisualBotoesPorJanela(); // Retorna padrão se os parâmetros forem inválidos

            if (VisualBotoes.TryGetValue(form.Name, out Dictionary<string, VisualBotoesPorJanela> botoesDict))
            {
                if (botoesDict.TryGetValue(tipoBotao, out VisualBotoesPorJanela botao))
                    return botao; // Retorna o botão que corresponde ao tipo
            }

            return new VisualBotoesPorJanela(); // Retorna configuração padrão se não encontrar
        }

        //---------------------------
        // Aparência do menu (ex.: Cor de Fundo, imagem de fundo)
        //---------------------------
    }
    public class TemaMenu
    {
        [Description("Indica se menu usa uma imagem como fundo, o caminho da imagem deve ser válido.")]
        public bool UsarImagemDeFundoMenu { get; set; } = false;
        [Description("Indica a imagem que aparece de fundo do menu, se o caminho for válido e UsarImagemDeFundo estiver ativado.")]
        public string CaminhoImagemDeFundoMenu { get; set; } = "";
        [Description("Indica se o fundo do menu é transparente, se a imagem de fundo estiver ativada isto é ignorado.")]
        public bool MenuTransparente { get; set; } = true; // por acontecer erros se a cor de fundo estiver desligada e se ativar mas não se mudar a cor, é preciso corrigir nesse caso
        [Description("Indica se a cor do fundo do menu, se for transparente isto é ignorado. ")]
        public Color CorFundoMenu { get; set; } = Color.White;

        public void Validate()
        {
            if (CorFundoMenu.IsEmpty)   // Permitir cores vazias
                CorFundoMenu = Color.White;
        }
    }
    //-------------------------------------------//
    //  Definições gerais do Pograma //
    //-------------------------------------------//
    public class AparenciaGeral
    {
        [Description("Nome do programa que aparece na barra de tarefas e em outros locais.")]
        public string Titulo { get; set; } = "Menu de Botões";
        [Description("Bloqueia ou permite que as janelas sejam redimensionaveis pelo utilizador.")]
        public bool JanelasRedimensionaveis { get; set; } = true;
        [Description("Bloqueia ou permite que as janelas sejam movidas pelo utilizador, quando está desativado ele também bloqueia o redimensionamento " +
        "\nSe as janelas não poderem ser redimensionaveis nem movives elas não terão uma Barra de titulo.")]
        public bool JanelasMoveis { get; set; } = true;
        [Description("Define se o programa aparece na barra de tarefas.")]
        public bool AparecerNaBarraDeTarefas { get; set; } = false;
        [Description("Define se o programa usa algum icone diferente do icone padrão.")]
        public bool UsaIcone { get; set; } = false;
        [Description("Caminho do icone usado pela aplicação se o UsaIcone estiver ativado.")]
        public string CaminhoIcone { get; set; } = "";
        [Description("Opacidade do programa, 1 para 100% visivel e 0.1 para muito pouco visivel.")]
        public double OpacidadeDoPrograma { get; set; } = 0.99;
        [Description("Define o programa deve criar um icone no menu de icones da barra de tarefa ou não.\n " +
        "Tray menu = icones de aplicações como aplicações em segundo plano, acesso fácil às funções do sistema, etc. (geralmente no canto direito inferior ao lado do relógio).")]
        public bool ProgramaNoTrayMenu { get; set; } = true;
        [Description("Tamanho inicial das janelas, quando o utlizador redimensiona a janela o valor não é atualizado pois ele é só o tamanho inicial.")]
        public TamanhoInicialJanelas TamanhoJanelas { get; set; } = new TamanhoInicialJanelas();
        [Description("Lista de temas. (Exemplo de temas.: Claro, Escuro, Azul)")]
        public List<TemaVisual> Tema { get; set; } = new List<TemaVisual>();
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                Titulo = "Menu de Botões";

            OpacidadeDoPrograma = Math.Max(0.1, Math.Min(OpacidadeDoPrograma, 1.0));

            if (TamanhoJanelas == null)
                TamanhoJanelas = new TamanhoInicialJanelas();
            TamanhoJanelas.Validate();

            if (Tema == null)
                Tema = new List<TemaVisual>();

            foreach (var tema in Tema)
            {
                if (tema != null)
                    tema.Validate();
            }
        }
    }
    public class FicheiroGenerico
    {
        [Description("Nome do ficheiro (sem o caminho)")]
        public string NomeFicheiro { get; set; } = "";

        [JsonIgnore]
        public string CaminhoRelativo => Path.Combine("Documentos", NomeFicheiro);

        [JsonIgnore]
        public string CaminhoCompleto => Path.Combine(Ficheiros.Caminho, CaminhoRelativo);

        /// <summary>
        /// Copia o ficheiro para a pasta Documentos e guarda apenas o nome
        /// </summary>
        public void AdicionarFicheiro(string caminhoOriginal)
        {
            if (string.IsNullOrWhiteSpace(caminhoOriginal) || !File.Exists(caminhoOriginal))
                return;

            string nome = Path.GetFileName(caminhoOriginal);
            string destino = Path.Combine(Ficheiros.Caminho, "Documentos");

            Directory.CreateDirectory(destino);

            string caminhoDestino = Path.Combine(destino, nome);
            File.Copy(caminhoOriginal, caminhoDestino, overwrite: true);

            NomeFicheiro = nome;
        }

        /// <summary>
        /// Verifica se o ficheiro existe no local esperado
        /// </summary>
        public bool Existe => File.Exists(CaminhoCompleto);
    }
    //---------------------------
    // Dados do Passo a Passo com array fixo de 4 imagens (índices 0 a 3)
    //---------------------------
    public class ConfigPassoAPasso
    {
        [Description("Nome da janela do passo a passo")]
        public string Titulo { get; set; } = "Passo a Passo";

        [Description("Nomes dos botões do Passo a Passo")]
        public string[] NomeBotoes { get; set; } = new string[3];

        public void AdicionarImagensAoBotao(int indiceBotao, string[] ficheiros)
        {
            if (indiceBotao < 0 || indiceBotao >= NomeBotoes.Length || ficheiros == null || ficheiros.Length == 0)
                return;

            string nomeBotao = NomeBotoes[indiceBotao];
            if (string.IsNullOrWhiteSpace(nomeBotao))
                return;

            string pastaDestino = Path.Combine(Ficheiros.Caminho, "Imagens", Titulo, nomeBotao);
            Directory.CreateDirectory(pastaDestino);

            // Normaliza os ficheiros de entrada (caminhos absolutos, minúsculas, sem espaços extra)
            HashSet<string> ficheirosEntrada = ficheiros
                .Select(f => Path.GetFullPath(f).ToLowerInvariant())
                .ToHashSet();

            // Apaga apenas os ficheiros antigos que não estão na nova lista
            try
            {
                foreach (var existente in Directory.GetFiles(pastaDestino))
                {
                    string caminhoNormalizado = Path.GetFullPath(existente).ToLowerInvariant();
                    if (!ficheirosEntrada.Contains(caminhoNormalizado))
                    {
                        try { File.Delete(existente); } catch { }
                    }
                }
            }
            catch { }

            // Copia os ficheiros com nomes sequenciais
            int contador = 1;
            foreach (var origem in ficheiros)
            {
                try
                {
                    string extensao = Path.GetExtension(origem);
                    string nomeFinal = $"Imagem_{contador++}{extensao}";
                    string destino = Path.Combine(pastaDestino, nomeFinal);

                    // Evita copiar para cima de si mesmo
                    if (!string.Equals(Path.GetFullPath(origem), Path.GetFullPath(destino), StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(origem, destino, true);
                    }
                }
                catch { }
            }
        }
        public List<Image> ObterImagensDoBotao(int indiceBotao)
        {
            List<Image> imagens = new();

            if (indiceBotao < 0 || indiceBotao >= NomeBotoes.Length)
                return imagens;

            string nomeBotao = NomeBotoes[indiceBotao];
            string pasta = Path.Combine(Ficheiros.Caminho, "Imagens", Titulo, nomeBotao);

            if (Directory.Exists(pasta))
            {
                foreach (var caminho in Directory.GetFiles(pasta).OrderBy(f => f))
                {
                    try { imagens.Add(Image.FromFile(caminho)); } catch { }
                }
            }

            if (imagens.Count == 0)
            {
                try
                {
                    imagens.Add(Image.FromStream(
                        Assembly.GetExecutingAssembly().GetManifestResourceStream("PerguntasFrequentesSuporte.Erro.png")));
                }
                catch { }
            }

            return imagens;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                Titulo = "Passo a Passo";

            if (NomeBotoes == null || NomeBotoes.Length != 3)
                NomeBotoes = new string[3];
        }
    }
    //---------------------------
    // Dados para o botão "Sair/Mostrar"
    //---------------------------
    public class ConfigSair_Mostrar
    {
        [Description("Define se o botão que de mostrar/esconde o Menu está visivel ou não.")]
        public bool BotaoEsconder_MostrarEsconderVisivel { get; set; } = true;
        [Description("Define se o botão de sair está visivel ou não.")]
        public bool BtnSairVisivel { get; set; } = true;
        [Description("Define se os botões de Sair e o botão de mostrar/esconder o menu estão em cima ou em baixo (em baixo quando desativado).")]
        public bool PosicaoSuperior { get; set; } = false;
        [Description("Define se o botão de mostrar/esconder o menu está a esquerda, o botão de sair fica do lado oposto quando está ativado.")]
        public bool PosicaoEsquerda { get; set; } = true;
        [Description("Textos para o botão que mostra/esconde o menu.\n BotoesVisiveis : Mostrar, BotoesEscondidos : Esconder, FormsAberto : Feche a janela.\n Não se deve mudar o primeiro elemento só a mensagem")]
        public Dictionary<string, string> TextoBotaoMostrarEsconder { get; set; } = new Dictionary<string, string>()
        {
            { "BotoesVisiveis", "Esconder" },
            { "BotoesEscondidos", "Mostrar" },
            { "FormsAberto", "Feche a janela." }
        };
        public void Validate()
        {
            if (TextoBotaoMostrarEsconder == null)
                TextoBotaoMostrarEsconder = new Dictionary<string, string>();

            // Chaves corretas conforme a descrição
            List<string> chavesCorretas = new() { "BotoesVisiveis", "BotoesEscondidos", "FormsAberto" };

            // Textos padrão para cada chave
            Dictionary<string, string> textosPadrao = new()
            {
                { "BotoesVisiveis", "Mostrar" },
                { "BotoesEscondidos", "Esconder" },
                { "FormsAberto", "Feche a janela." }
            };

            // Verificar e corrigir chaves incorretas
            List<string> chavesAtuais = TextoBotaoMostrarEsconder.Keys.ToList();
            foreach (string chave in chavesAtuais)
            {
                if (!chavesCorretas.Contains(chave)) // Se a chave não for uma das corretas, apaga-a
                    TextoBotaoMostrarEsconder.Remove(chave);
            }

            foreach (string chaveCorreta in chavesCorretas) // Garantir que todas as chaves corretas existem
            {
                if (!TextoBotaoMostrarEsconder.ContainsKey(chaveCorreta))  // Se faltar alguma chave, adiciona com o texto padrão
                    TextoBotaoMostrarEsconder[chaveCorreta] = textosPadrao[chaveCorreta];
            }
        }
    }
    //---------------------------
    // Função do botão no menu
    //---------------------------
    public class FuncaoBotaoMenu
    {
        [Description("Nome do Botão.")]
        public string Nome { get; set; } = "DefaultBotão";

        [Description("Tipo do botão.\nOpções:\n\tPDF\n\tLINK\n\tFORMS")]
        public string Tipo { get; set; } = "Link";

        [Description("Diretorio ou Link indica o arquivo/link que o botão vai abrir.\n" +
            "\tPara PDF é preenchido automaticamente com base no nome do ficheiro (sem extensão).\n" +
            "\tPara Link é o link do site que se quer abrir.\n" +
            "\tPara Forms pode ser CONFIG, FORMULARIO ou PASSOAPASSO[ID]")]
        public string Diretorio_Link { get; set; } = "";

        [Description("Ficheiro PDF associado ao botão (usado apenas quando Tipo for PDF)")]
        public FicheiroGenerico DocumentoPDF { get; set; } = new();

        public void Validate()
        {
            List<string> tiposPermitidos = new() { "FORMS", "LINK", "PDF" };

            if (string.IsNullOrWhiteSpace(Nome))
                Nome = "Botao Default";

            if (string.IsNullOrWhiteSpace(Tipo) || !tiposPermitidos.Contains(Tipo, StringComparer.OrdinalIgnoreCase))
                Tipo = "LINK";

            if (Tipo.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                if (DocumentoPDF == null)
                    DocumentoPDF = new FicheiroGenerico();

                // Se não tiver ficheiro associado, usar default
                if (string.IsNullOrWhiteSpace(DocumentoPDF.NomeFicheiro))
                    Diretorio_Link = "DocumentoDefault";
                else
                    Diretorio_Link = Path.GetFileNameWithoutExtension(DocumentoPDF.NomeFicheiro);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Diretorio_Link))
                {
                    if (Tipo.Equals("LINK", StringComparison.OrdinalIgnoreCase))
                        Diretorio_Link = "https://example.com";
                    else if (Tipo.Equals("FORMS", StringComparison.OrdinalIgnoreCase))
                        Diretorio_Link = "CONFIG";
                }
            }
        }
    }
    //---------------------------
    // Dados do Menu
    //---------------------------
    public class ConfigGeral
    {
        [Description("Define qual a percentagem da altura do ecrã é que os botões do menu vão usar. " +
        "\nO tamanho dos botões é definida automaticamente com todo o espaço disponivel na largura " +
        "\n(Dica: Definir uma margem alta para que os botões não fiquem muito grandes)")]
        public int PercentagemEcraParaBotoesAltura { get; set; } = 80;
        [Description("Define qual a percentagem da largura do ecrã é que os botões do menu vão usar. " +
        "\nO tamanho dos botões é definida automaticamente com todo o espaço disponivel na largura " +
        "\n(Dica: Definir uma margem alta para que os botões não fiquem muito grandes)")]
        public int PercentagemEcraParaBotoesLargura { get; set; } = 100;
        [Description("Define a quantidade de botões que cada linha vai ter, não pode ter mais que 2 linhas no total")]
        public int BotoesPorLinha { get; set; } = 1
            ;
        [Description("Quantidade de botões que aparecem no menu, podem haver mais botões configurados do que os botões visiveis mas não o contrário")]
        public int QuantidadeBotoes { get; set; } = 1;
        [Description("Configuração de cada botão. Os botões aparecem por ordem que estão na lista")]
        public List<FuncaoBotaoMenu> ConfiguracoesBotoesMenu { get; set; } = new List<FuncaoBotaoMenu>();
        [Description("Configurações do botão de sair e do que mostra/esconde o menu")]
        public ConfigSair_Mostrar ConfiguracoesSair_Mostrar { get; set; } = new ConfigSair_Mostrar();
        [Description("Configurações dos Passo a Passo, pode haver vários passo a passo diferentes no mesmo perfil")]
        public List<ConfigPassoAPasso> ConfiguracoesPassoAPasso { get; set; } = new List<ConfigPassoAPasso>();

        public ConfigGeral()
        {
            ConfiguracoesBotoesMenu = new List<FuncaoBotaoMenu>();
            ConfiguracoesPassoAPasso = new List<ConfigPassoAPasso>();

            // Adiciona um botão padrão
            FuncaoBotaoMenu novoBotao = new();
            novoBotao.Validate();
            ConfiguracoesBotoesMenu.Add(novoBotao);

            // Adiciona um passo a passo padrão
            ConfigPassoAPasso novoPasso = new();
            novoPasso.Validate();
            ConfiguracoesPassoAPasso.Add(novoPasso);

            // Configuração do botão de sair/mostrar
            ConfiguracoesSair_Mostrar = new ConfigSair_Mostrar();
            ConfiguracoesSair_Mostrar.Validate();
        }
        public void Validate()
        {
            ConfiguracoesSair_Mostrar?.Validate();

            PercentagemEcraParaBotoesAltura = Math.Max(10, Math.Min(PercentagemEcraParaBotoesAltura, 100));
            PercentagemEcraParaBotoesLargura = Math.Max(10, Math.Min(PercentagemEcraParaBotoesLargura, 100));
            BotoesPorLinha = Math.Max(0, Math.Min(BotoesPorLinha, 8));
            QuantidadeBotoes = Math.Max(0, Math.Min(QuantidadeBotoes, 8));

            if (ConfiguracoesBotoesMenu == null || ConfiguracoesBotoesMenu.Count == 0)
            {
                FuncaoBotaoMenu novoBotao = new();
                novoBotao.Validate();
                ConfiguracoesBotoesMenu.Add(novoBotao);
            }

            if (ConfiguracoesPassoAPasso != null)
            {
                foreach (ConfigPassoAPasso passo in ConfiguracoesPassoAPasso)
                    passo.Validate();
            }

            if (ConfiguracoesBotoesMenu != null)
            {
                foreach (FuncaoBotaoMenu botao in ConfiguracoesBotoesMenu)
                    botao.Validate();
            }
        }
    }
    //---------------------------------//
    // Configurações do Perfil //
    //---------------------------------//
    public class AppConfig
    {
        public string Perfil { get; set; }
        public int IndiceTemaAtual { get; set; }
        public ConfigGeral ConfiguracaoAplicacao { get; set; }
        public AparenciaGeral VisualAplicacao { get; set; }

        public AppConfig(AparenciaGeral visualAplicacao, ConfigGeral configuracaoAplicacao, string perfil = "Padrao", int indiceTemaAtual = 0)
        {
            Perfil = "Padrao";
            ConfiguracaoAplicacao = new ConfigGeral();
            VisualAplicacao = new AparenciaGeral();

            if (!string.IsNullOrWhiteSpace(perfil))
                Perfil = perfil;

            if (configuracaoAplicacao != null)
                ConfiguracaoAplicacao = configuracaoAplicacao;

            if (visualAplicacao != null)
                VisualAplicacao = visualAplicacao; 
            
            OperacoesComuns.LimitarValor(Minimo:0, Maximo: VisualAplicacao.Tema.Count - 1, Valor: ref indiceTemaAtual);
            IndiceTemaAtual = indiceTemaAtual; 

            Validate();
        }
        public AppConfig(AppConfig config) : this
        (
            config.VisualAplicacao, 
            config.ConfiguracaoAplicacao, 
            config.Perfil, 
            config.IndiceTemaAtual
        ) { }
        public AppConfig() : this
        (
            ConfiguracoesHelper.CriarAparenciaGeralBase(),
            ConfiguracoesHelper.CriarConfigGeralBase(), "Padrao", 0
         )
        { }
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Perfil))
                Perfil = "Padrao";

            if (ConfiguracaoAplicacao != null)
                ConfiguracaoAplicacao.Validate();

            if (VisualAplicacao != null)
                VisualAplicacao.Validate();

            IndiceTemaAtual = Math.Max(0, Math.Min(VisualAplicacao.Tema.Count - 1, IndiceTemaAtual));

        }
    }
    //---------------------------
    // Padrão Singleton para gerir a instância única de Configuracoes
    //---------------------------
    public static class AcederConfig
    {
        private static AppConfig AppConfig_Interno;
        public static AppConfig ConfigAtual
        {
            get
            {
                if (AppConfig_Interno == null)
                    AppConfig_Interno = Ficheiros.IniciarConfiguracoes();
                return AppConfig_Interno;
            }
        }
    }
}