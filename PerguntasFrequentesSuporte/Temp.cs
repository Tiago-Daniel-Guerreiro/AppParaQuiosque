/*
namespace PerguntasFrequentesSuporte
{

    public partial class Configuracoes : Form
    {
        // Lista de itens gerados via reflection (para listagem apenas)
        private List<Item> ItensDaConfig = new List<Item>();

        public Configuracoes()
        {
            InitializeComponent();
        }
        // Desenha os itens da ComboBox de fontes com a própria fonte aplicada
        public void cbFontes_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                string fontName = cbFontes.Items[e.Index].ToString();
                try
                {
                    e.Graphics.DrawString(fontName, new Font(fontName, 11), Brushes.Black, e.Bounds);
                }
                catch
                {
                    e.Graphics.DrawString(fontName, cbFontes.Font, Brushes.Black, e.Bounds);
                }
                e.DrawFocusRectangle();
            }
        }

        // Copia o valor selecionado da listBoxDetalhes para a área de transferência ao clicar
        private void listBoxDetalhes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxDetalhes.SelectedItem != null)
            {
                //Clipboard.SetText(listBoxDetalhes.GetItemText(listBoxDetalhes.SelectedItem));
            }
        }
        private void Configuracoes_Load(object sender, EventArgs e)
        {
            ListarFontes();

            // Exemplo: listando itens a partir de ConfiguracoesGlobais.Config_Menu
            ItensDaConfig = ListagemConfig.ListarJanelaMenu(ConfiguracoesGlobais.Config_Menu);
            listBoxConfig.Items.Clear();
            foreach (var item in ItensDaConfig)
            {
                listBoxConfig.Items.Add(item.Nome);
            }
        }

        // Exemplo de evento para escolher uma cor (usando um ColorDialog)
        private void txtCor_Click(object sender, EventArgs e)
        {
            Color? novaCor = Menu.InputColorValor(txtCores.BackColor);

            if (novaCor.HasValue) // Se o utilizador escolheu uma cor
                txtCores.BackColor = (Color)novaCor;
        }

        private void ListarFontes()
        {
            InstalledFontCollection fontes = new InstalledFontCollection();
            try
            {
                foreach (FontFamily font in fontes.Families)
                {
                    cbFontes.Items.Add(font.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao listar fontes: " + ex.Message);
            }
            if (cbFontes.Items.Count > 2)
                cbFontes.SelectedIndex = 2;
        }


        // Exibe os detalhes do item selecionado no listBoxDetalhes
        private void listBoxConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxConfig.SelectedIndex;
            if (index >= 0 && index < ItensDaConfig.Count)
            {
                Item item = ItensDaConfig[index];

                // Usa reflection para obter o valor atual (para exibição)
                object valorAtual = ListagemConfig.ObterValor(item.Localização);
                string valorTexto = (valorAtual != null) ? valorAtual.ToString() : "NULO";

                listBoxDetalhes.Items.Clear();
                listBoxDetalhes.Items.Add("Nome: " + item.Nome);
                listBoxDetalhes.Items.Add("Tipo: " + item.Tipo);
                listBoxDetalhes.Items.Add("Localização: " + item.Localização);
                listBoxDetalhes.Items.Add("Valor: " + valorTexto);
                listBoxDetalhes.Items.Add("Descrição: " + item.Descrição);
            }
        }

        // Ao dar duplo clique, chama o subprograma de edição manual (sem reflection para escrita)
        private void listBoxConfig_DoubleClick(object sender, EventArgs e)
        {
            int index = listBoxConfig.SelectedIndex;
            if (index < 0 || index >= ItensDaConfig.Count)
                return;

            Item item = ItensDaConfig[index];
            SubprogramaEdicao.EditarPorSwitch(item);
            // Atualiza os detalhes após a edição
            listBoxConfig_SelectedIndexChanged(sender, e);

            MudancasVisuais.AtualizarTudo();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MudancasVisuais.AtualizarTudo();
            Hide();

            foreach (Form form in Application.OpenForms) // Percorrer todas as janelas abertas e identificar cada tipo de janela
            {
                if (form is Menu menu)
                    menu.BtnMudarEsconder_MostrarMenu_Click(sender, e);
            }
        }
    }
    public static class InputBoxTemp
    {
        public static string Show(string prompt, string title, string defaultValue = "")
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = prompt;
            textBox.Text = defaultValue;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancelar";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(10, 10, 380, 20);
            textBox.SetBounds(10, 40, 380, 25);
            buttonOk.SetBounds(220, 80, 80, 30);
            buttonCancel.SetBounds(310, 80, 80, 30);

            form.ClientSize = new Size(400, 120);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.StartPosition = FormStartPosition.CenterScreen;

            // Permite que o form seja um controlo dentro do Menu
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            // Adiciona o form dentro do Menu encontrado nos Forms abertos
            foreach (Form formatual in Application.OpenForms) // Percorrer todas as janelas abertas e identificar cada tipo de janela
            {
                if (formatual is Menu menu)
                {
                    menu.Controls.Add(form);
                    form.Show();
                }
            }

            // Cria um timer para verificar o valor de textBox.Text
            string resultado = null;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Intervalo de 100ms

            timer.Tick += (sender, e) =>
            {
                // Se textBox.Text não estiver vazio ou nulo, para o timer e atribui o valor
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    resultado = textBox.Text;
                    timer.Stop();
                }
            };

            timer.Start();

            // Mantém o Subprograma em execução enquanto o resultado ainda for null
            while (resultado == null)
            {
                Application.DoEvents();
            }

            return resultado;
        }

    }
    public static class ListagemConfig
    {
        // Exemplo de listagem para um objeto JanelaMenu
        public static List<Item> ListarJanelaMenu(JanelaMenu jm, string caminho = "Config_Menu")
        {
            var lista = new List<Item>();
            lista.AddRange(ListarBaseJanela(jm.Base, caminho + ".Base"));
            lista.AddRange(ListarDadosMenu(jm.Dados, caminho + ".Dados"));
            return lista;
        }
        public static List<Item> ListarBaseJanela(BaseJanela baseJanela, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(baseJanela, "Titulo", caminho));
            lista.Add(CriarItem(baseJanela, "Margem", caminho));
            lista.Add(CriarItem(baseJanela, "ExibirBarraTitulo", caminho));
            lista.Add(CriarItem(baseJanela, "AparecerNaBarraDeTarefas", caminho));
            lista.Add(CriarItem(baseJanela, "UsaIcone", caminho));
            lista.Add(CriarItem(baseJanela, "CaminhoIcone", caminho));
            lista.AddRange(ListarFonte(baseJanela.Fonte, caminho + ".Fonte"));
            lista.AddRange(ListarEscala(baseJanela.Escala, caminho + ".Escala"));

            if (baseJanela.TemaVisual != null)
            {
                for (int i = 0; i < baseJanela.TemaVisual.Length; i++)
                {
                    lista.AddRange(ListarAparencia(baseJanela.TemaVisual[i], caminho + $".TemaVisual[{i}]"));
                }
            }
            return lista;
        }
        public static List<Item> ListarDadosMenu(DadosMenu dm, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(dm, "TelaInteira", caminho));
            lista.Add(CriarItem(dm, "BarraPosicaoSuperior", caminho));
            lista.Add(CriarItem(dm, "BarraPosicaoDireita", caminho));
            lista.Add(CriarItem(dm, "TamanhoBotoesFixo", caminho));
            lista.Add(CriarItem(dm, "PercentagemEcra", caminho));
            lista.Add(CriarItem(dm, "AlturaBotoes", caminho));
            lista.Add(CriarItem(dm, "BotoesPorLinha", caminho));
            lista.Add(CriarItem(dm, "QuantidadeBotoes", caminho));

            if (dm.Botoes != null)
            {
                for (int i = 0; i < dm.Botoes.Length; i++)
                {
                    lista.AddRange(ListarFuncaoBotao(dm.Botoes[i], caminho + $".Botoes[{i}]"));
                }
            }
            return lista;
        }
        public static List<Item> ListarFuncaoBotao(FuncaoBotao fb, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(fb, "Nome", caminho));
            lista.Add(CriarItem(fb, "Tipo", caminho));
            lista.Add(CriarItem(fb, "Diretorio_Link", caminho));
            return lista;
        }

        public static List<Item> ListarFonte(Fonte fonte, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(fonte, "Font", caminho));
            lista.Add(CriarItem(fonte, "FonteArray", caminho));
            lista.Add(CriarItem(fonte, "TamanhoMaximo", caminho));
            lista.Add(CriarItem(fonte, "TamanhoMinimo", caminho));
            lista.Add(CriarItem(fonte, "MultiplicadorEscala", caminho));
            return lista;
        }
        public static List<Item> ListarEscala(Escala escala, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(escala, "Fixa", caminho));
            lista.Add(CriarItem(escala, "ValorFixo", caminho));
            lista.Add(CriarItem(escala, "Maxima", caminho));
            lista.Add(CriarItem(escala, "Minima", caminho));
            lista.AddRange(ListarResolucao(escala.Resolucao, caminho + ".Resolucao"));
            return lista;
        }

        public static List<Item> ListarResolucao(Resolucao resolucao, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(resolucao, "Maxima", caminho));
            lista.Add(CriarItem(resolucao, "Minima", caminho));
            return lista;
        }

        public static List<Item> ListarAparencia(Aparencia aparencia, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(aparencia, "NomeTema", caminho));
            lista.Add(CriarItem(aparencia, "UsaImagemDeFundo", caminho));
            lista.Add(CriarItem(aparencia, "CaminhoImagemDeFundo", caminho));
            lista.Add(CriarItem(aparencia, "CorFundo", caminho));
            lista.Add(CriarItem(aparencia, "CorFundoTransparente", caminho));
            lista.Add(CriarItem(aparencia, "IntensidadeArredondarBorda", caminho));
            lista.Add(CriarItem(aparencia, "TamanhoContrasteBorda", caminho));
            lista.Add(CriarItem(aparencia, "CorContrasteBorda", caminho));

            if (aparencia.VisualBotoes != null)
            {
                for (int i = 0; i < aparencia.VisualBotoes.Length; i++)
                {
                    lista.AddRange(ListarAparenciaBotoes(aparencia.VisualBotoes[i], caminho + $".VisualBotoes[{i}]"));
                }
            }
            return lista;
        }

        public static List<Item> ListarAparenciaBotoes(AparenciaBotoes ab, string caminho)
        {
            var lista = new List<Item>();
            lista.Add(CriarItem(ab, "IntensidadeArredondarBorda", caminho));
            lista.Add(CriarItem(ab, "TamanhoContrasteBorda", caminho));
            lista.Add(CriarItem(ab, "CorContrasteBorda", caminho));
            lista.Add(CriarItem(ab, "CorTexto", caminho));
            lista.Add(CriarItem(ab, "CorFundo", caminho));
            return lista;
        }

        // Subprograma que cria um Item usando reflection (apenas para leitura)
        public static Item CriarItem(object obj, string nomeMembro, string caminho)
        {
            var type = obj.GetType();
            MemberInfo member = type.GetProperty(nomeMembro) ?? (MemberInfo)type.GetField(nomeMembro);

            string tipoStr = "Desconhecido";
            object valorObj = null;

            if (member is PropertyInfo pi)
            {
                tipoStr = pi.PropertyType.Name;
                valorObj = pi.GetValue(obj);
            }
            else if (member is FieldInfo fi)
            {
                tipoStr = fi.FieldType.Name;
                valorObj = fi.GetValue(obj);
            }

            string valorStr = valorObj != null ? valorObj.ToString() : "";
            string descricao = "Descrição: ";
            var descAttr = (DescriptionAttribute[])member?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descAttr != null && descAttr.Length > 0 && !string.IsNullOrEmpty(descAttr[0].Description))
                descricao += descAttr[0].Description;

            return new Item
            {
                // Aqui, usamos a função FormatMemberName para inserir espaços antes de cada maiúscula (exceto a primeira)
                Nome = FormatMemberName(nomeMembro),
                Tipo = tipoStr,
                Descrição = descricao,
                // O caminho é montado sem os espaços adicionais
                Localização = caminho + "." + nomeMembro,
                Valor = valorStr
            };
        }

        // Função auxiliar que formata o nome (insere espaço antes de cada letra maiúscula, exceto a primeira)
        private static string FormatMemberName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new StringBuilder();
            sb.Append(name[0]); // a primeira letra sem espaço
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                    sb.Append(" ");
                sb.Append(name[i]);
            }
            return sb.ToString();
        }

        // Subprograma para obter o valor dado um caminho (suporta indexadores, como FonteArray[2])
        public static object ObterValor(string caminho)
        {
            Type staticType = typeof(ConfiguracoesGlobais);
            if (string.IsNullOrWhiteSpace(caminho)) return null;

            string[] parts = caminho.Split('.');
            object currentValue = null;
            Type currentType = staticType;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                int? arrayIndex = null;
                string memberName = part;

                int bracketPos = part.IndexOf('[');
                int bracketEnd = part.IndexOf(']');
                if (bracketPos > 0 && bracketEnd > bracketPos)
                {
                    memberName = part.Substring(0, bracketPos);
                    string indexStr = part.Substring(bracketPos + 1, bracketEnd - bracketPos - 1);
                    if (int.TryParse(indexStr, out int idx))
                        arrayIndex = idx;
                }

                if (currentValue == null)
                {
                    MemberInfo memberInfo = currentType.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static)
                                             ?? (MemberInfo)currentType.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
                    if (memberInfo == null) return null;

                    if (memberInfo is PropertyInfo prop)
                    {
                        currentValue = prop.GetValue(null, null);
                        currentType = prop.PropertyType;
                    }
                    else if (memberInfo is FieldInfo field)
                    {
                        currentValue = field.GetValue(null);
                        currentType = field.FieldType;
                    }
                }
                else
                {
                    MemberInfo memberInfo = currentValue.GetType().GetProperty(memberName)
                                             ?? (MemberInfo)currentValue.GetType().GetField(memberName);
                    if (memberInfo == null) return null;

                    if (memberInfo is PropertyInfo prop)
                    {
                        currentValue = prop.GetValue(currentValue, null);
                        currentType = prop.PropertyType;
                    }
                    else if (memberInfo is FieldInfo field)
                    {
                        currentValue = field.GetValue(currentValue);
                        currentType = field.FieldType;
                    }
                }

                if (arrayIndex.HasValue && currentValue != null)
                {
                    if (currentValue is Array arr)
                    {
                        if (arrayIndex.Value >= 0 && arrayIndex.Value < arr.Length)
                        {
                            currentValue = arr.GetValue(arrayIndex.Value);
                            currentType = currentValue?.GetType();
                        }
                        else return null;
                    }
                    else if (currentValue is IList list)
                    {
                        if (arrayIndex.Value >= 0 && arrayIndex.Value < list.Count)
                        {
                            currentValue = list[arrayIndex.Value];
                            currentType = currentValue?.GetType();
                        }
                        else return null;
                    }
                }
            }
            return currentValue;
        }
    }
    public static class SubprogramaEdicao
    {
        public static void EditarPorSwitch(Item item)
        {
            string input;
            switch (item.Localização)
            {
                // Configuração Principal
                case "Config_Principal.Base.Titulo":
                    input = InputBoxTemp.Show("Digite o novo título:", "Editar Título", ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Titulo);
                    if (!string.IsNullOrEmpty(input))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base;
                        baseTemp.Titulo = input;
                        ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base = baseTemp;
                    }
                    break;

                case "Config_Principal.Base.Margem":
                    input = InputBoxTemp.Show("Digite a nova margem:", "Editar Margem", ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Margem.ToString());
                    if (double.TryParse(input, out double margem))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base;
                        baseTemp.Margem = margem;
                        ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base = baseTemp;
                    }
                    break;

                case "Config_Principal.Base.ExibirBarraTitulo":
                    input = InputBoxTemp.Show("Exibir Barra de Título? (true/false):", "Editar", ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.ExibirBarraTitulo.ToString());
                    if (bool.TryParse(input, out bool exibirBarraTitulo))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base;
                        baseTemp.ExibirBarraTitulo = exibirBarraTitulo;
                        ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base = baseTemp;
                    }
                    break;

                // Configuração do Menu
                case "Config_Menu.Base.Titulo":
                    input = InputBoxTemp.Show("Digite o novo título:", "Editar Título", ConfiguracoesGlobais.Config_Menu.Base.Titulo);
                    if (!string.IsNullOrEmpty(input))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        baseTemp.Titulo = input;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                    }
                    break;

                case "Config_Menu.Base.Margem":
                    input = InputBoxTemp.Show("Digite a nova margem:", "Editar Margem", ConfiguracoesGlobais.Config_Menu.Base.Margem.ToString());
                    if (double.TryParse(input, out double margemMenu))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        baseTemp.Margem = margemMenu;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                    }
                    break;

                // Configuração da Fonte
                case "Config_Menu.Base.Fonte.TamanhoMaximo":
                    input = InputBoxTemp.Show("Digite o novo tamanho máximo da fonte:", "Editar Fonte", ConfiguracoesGlobais.Config_Menu.Base.Fonte.TamanhoMaximo.ToString());
                    if (double.TryParse(input, out double tamanhoMax))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        var fonteTemp = baseTemp.Fonte;
                        fonteTemp.TamanhoMaximo = tamanhoMax;
                        baseTemp.Fonte = fonteTemp;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                    }
                    break;

                case "Config_Menu.Base.Fonte.Font":
                    input = InputBoxTemp.Show("Digite a nova fonte (Nome,Size,Style):", "Editar Fonte", ConfiguracoesGlobais.Config_Menu.Base.Fonte.Font.ToString());
                    Font novaFonte = TentarParseFont(input);
                    if (novaFonte != null)
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        var fonteTemp = baseTemp.Fonte;
                        fonteTemp.Font = novaFonte;
                        baseTemp.Fonte = fonteTemp;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                    }
                    break;

                // Configuração da Escala
                case "Config_Menu.Base.Escala.Fixa":
                    input = InputBoxTemp.Show("A escala é fixa? (true/false):", "Editar Escala", ConfiguracoesGlobais.Config_Menu.Base.Escala.Fixa.ToString());
                    if (bool.TryParse(input, out bool fixa))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        var escalaTemp = baseTemp.Escala;
                        escalaTemp.Fixa = fixa;
                        baseTemp.Escala = escalaTemp;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                    }
                    break;

                case "Config_Menu.Base.Escala.ValorFixo":
                    input = InputBoxTemp.Show("Digite o novo valor fixo da escala:", "Editar Escala", ConfiguracoesGlobais.Config_Menu.Base.Escala.ValorFixo.ToString());
                    if (double.TryParse(input, out double valorFixo))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        var escalaTemp = baseTemp.Escala;
                        escalaTemp.ValorFixo = valorFixo;
                        baseTemp.Escala = escalaTemp;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                    }
                    break;

                // Configuração da Aparência
                case "Config_Menu.Base.TemaVisual[0].CorFundo":
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        var temaTemp = baseTemp.TemaVisual[0];
                        var temaTemp2 = baseTemp.TemaVisual[1];
                        // Chamar o subprograma para abrir o seletor de cores personalizado
                        Color novaCor = Menu.InputColorValor(baseTemp.TemaVisual[0].CorFundo);

                        temaTemp.CorFundo = novaCor;
                        temaTemp2.CorFundo = novaCor;

                        baseTemp.TemaVisual[0] = temaTemp;
                        baseTemp.TemaVisual[1] = temaTemp2;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;
                        MudancasVisuais.AtualizarTudo();
                    }
                    break;

                // Configuração da Aparência
                case "Config_Menu.Base.TemaVisual[0].CorFundoTransparente":
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Base;
                        var temaTemp = baseTemp.TemaVisual[0];
                        var temaTemp2 = baseTemp.TemaVisual[1];

                        // Alternar entre true e false
                        bool novoEstado = !temaTemp.CorFundoTransparente;

                        temaTemp.CorFundoTransparente = novoEstado;
                        temaTemp2.CorFundoTransparente = novoEstado;

                        baseTemp.TemaVisual[0] = temaTemp;
                        baseTemp.TemaVisual[1] = temaTemp2;
                        ConfiguracoesGlobais.Config_Menu.Base = baseTemp;

                        MudancasVisuais.AtualizarTudo();
                    }
                    break;


                // Configuração de Botões
                case "Config_Menu.Dados.Botoes[0].Nome":
                    input = InputBoxTemp.Show("Digite o novo nome do botão:", "Editar Botão", ConfiguracoesGlobais.Config_Menu.Dados.Botoes[0].Nome);
                    if (!string.IsNullOrEmpty(input))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Dados;
                        var botaoTemp = baseTemp.Botoes[0];
                        botaoTemp.Nome = input;
                        baseTemp.Botoes[0] = botaoTemp;
                        ConfiguracoesGlobais.Config_Menu.Dados = baseTemp;
                    }
                    break;

                case "Config_Menu.Dados.Botoes[0].Tipo":
                    input = InputBoxTemp.Show("Digite o novo tipo do botão:", "Editar Botão", ConfiguracoesGlobais.Config_Menu.Dados.Botoes[0].Tipo);
                    if (!string.IsNullOrEmpty(input))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Dados;
                        var botaoTemp = baseTemp.Botoes[0];
                        botaoTemp.Tipo = input;
                        baseTemp.Botoes[0] = botaoTemp;
                        ConfiguracoesGlobais.Config_Menu.Dados = baseTemp;
                    }
                    break;

                case "Config_Menu.Dados.Botoes[0].Diretorio_Link":
                    input = InputBoxTemp.Show("Digite o novo link do botão:", "Editar Botão", ConfiguracoesGlobais.Config_Menu.Dados.Botoes[0].Diretorio_Link);
                    if (!string.IsNullOrEmpty(input))
                    {
                        var baseTemp = ConfiguracoesGlobais.Config_Menu.Dados;
                        var botaoTemp = baseTemp.Botoes[0];
                        botaoTemp.Diretorio_Link = input;
                        baseTemp.Botoes[0] = botaoTemp;
                        ConfiguracoesGlobais.Config_Menu.Dados = baseTemp;
                    }
                    break;

                default:
                    MessageBox.Show("Campo ainda não implementado:\n" + item.Localização);
                    break;
            }
        }

        // Subprograma auxiliar para converter uma string em um objeto Font (ex.: "Arial,12,Bold")
        private static Font TentarParseFont(string texto)
        {
            try
            {
                string[] parts = texto.Split(',');
                if (parts.Length < 2) return null;
                string nome = parts[0];
                float tamanho = float.Parse(parts[1]);
                FontStyle estilo = FontStyle.Regular;
                if (parts.Length >= 3)
                {
                    estilo = (FontStyle)Enum.Parse(typeof(FontStyle), parts[2], true);
                }
                return new Font(nome, tamanho, estilo);
            }
            catch
            {
                return null;
            }
        }
    }
    public class Item
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Descrição { get; set; }
        public string Localização { get; set; }
        public string Valor { get; set; }
    }
}

*/


/*
public static void SalvarConfiguracoes()
{
    VerificarECriarPastas();
    string caminhoFicheiro = ConfiguracoesGlobais.Caminho+"\\Config.txt";
    try
    {
        // Atualiza as fontes para serialização (trabalhando com variáveis locais)
        Fonte fontePrincipal = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Fonte;
        AtualizarFontParaArray(ref fontePrincipal);
        ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Fonte = fontePrincipal;

        Fonte fonteMenu = ConfiguracoesGlobais.Config_Menu.Base.Fonte;
        AtualizarFontParaArray(ref fonteMenu);
        ConfiguracoesGlobais.Config_Menu.Base.Fonte = fonteMenu;


        Fonte fontePasso = ConfiguracoesGlobais.Config_PassoAPasso.Base.Fonte;
        AtualizarFontParaArray(ref fontePasso);
        ConfiguracoesGlobais.Config_PassoAPasso.Base.Fonte = fontePasso;
        ConfiguracoesJson config = new ConfiguracoesJson
        {
            Perfil = ConfiguracoesGlobais.Perfil,
            Id_Perfil = ConfiguracoesGlobais.Id_Perfil,
            IndiceTemaAtual = ConfiguracoesGlobais.IndiceTemaAtual,
            Caminho = ConfiguracoesGlobais.Caminho,
            Config_BtnMudarEsconder_MostrarMenu = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu,
            Config_Menu = ConfiguracoesGlobais.Config_Menu,
            Config_PassoAPasso = ConfiguracoesGlobais.Config_PassoAPasso
        };

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = EscreverVariasveisVaziasNoJson ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = true,
            IncludeFields = true
        };

        if (File.Exists(caminhoFicheiro))
            File.Delete(caminhoFicheiro);

        string json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(caminhoFicheiro, json);

        MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erro ao salvar as configurações: " + ex.Message);
    }
}
public static void CarregarConfiguracoes()
{
    string caminhoFicheiro = ConfiguracoesGlobais.Caminho + @"\Config.txt";
    try
    {
        if (!File.Exists(caminhoFicheiro))
        {
            CarregarConfiguracoesPadrao();
            MessageBox.Show("Ficheiro de configuração não encontrado. Configurações padrão carregadas.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SalvarConfiguracoes();
            return;
        }

        string json = File.ReadAllText(caminhoFicheiro);
        JsonSerializerOptions opcoes = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };
        // Adiciona o conversor para Color
        opcoes.Converters.Add(new ColorJsonConverter());

        ConfiguracoesJson configCarregada = JsonSerializer.Deserialize<ConfiguracoesJson>(json, opcoes);
        // Primeiro, carrega as configurações padrão
        CarregarConfiguracoesPadrao();
        if (configCarregada != null)
        {
            if (!string.IsNullOrEmpty(configCarregada.Perfil))
                ConfiguracoesGlobais.Perfil = configCarregada.Perfil;

            AtualizarConfigPrincipal(configCarregada.Config_BtnMudarEsconder_MostrarMenu);
            AtualizarConfigMenu(configCarregada.Config_Menu);
            AtualizarConfigPassoAPasso(configCarregada.Config_PassoAPasso);
        }

        ReconstruirFonte(ref ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Fonte);
        ReconstruirFonte(ref ConfiguracoesGlobais.Config_Menu.Base.Fonte);
        ReconstruirFonte(ref ConfiguracoesGlobais.Config_PassoAPasso.Base.Fonte);

        //MessageBox.Show("Configurações carregadas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erro ao carregar as configurações: " + ex.Message);
    }
}

// Outros Subprograma (SalvarConfiguracoes, CarregarConfiguracoesPadrao, etc.) permanecem inalterados

// Conversor customizado para System.Drawing.Color
public static void CarregarConfiguracoesPadrao()
{
    // CONFIGURAÇÃO PADRÃO PARA A JANELA PRINCIPAL
    ConfiguracoesGlobais.Perfil = "Padrao";
    ConfiguracoesGlobais.IndiceTemaAtual = 0;
    ConfiguracoesGlobais.Id_Perfil = 0;
    ConfiguracoesGlobais.Caminho = Application.StartupPath + @"Perfil\" + ConfiguracoesGlobais.Id_Perfil ;
    VerificarECriarPastas();

    ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu = new BtnMudarEsconder_MostrarMenu
    {
        Base = new BaseJanela
        {
            Titulo = "Janela Principal",
            Margem = 2,
            ExibirBarraTitulo = true,
            AparecerNaBarraDeTarefas = true,
            UsaIcone = false,
            CaminhoIcone = null,
            Fonte = new Fonte
            {
                Font = new Font("Arial", 15, FontStyle.Bold),
                TamanhoMaximo = 15.5,
                TamanhoMinimo = 11,
            },
            Escala = new Escala
            {
                Fixa = false,
                ValorFixo = 0.9,
                Maxima = 0.9,
                Minima = 0.6,
                MultiplicadorEscalaFonte = 0,
                Resolucao = new Resolucao
                {
                    Maxima = new int[] { 1920, 1080 },
                    Minima = new int[] { 800, 480 }
                },
            },
            TemaVisual = new Aparencia[]
            {
                new Aparencia // Tema Claro
                {
                    NomeTema = "Claro",
                    CorFundo = Color.Red,
                    CorFundoTransparente = true,
                    CaminhoImagemDeFundo = null,
                    CorContrasteBorda = Color.Black,
                    TamanhoContrasteBorda = 5,
                    IntensidadeArredondarBorda = 5,
                    VisualBotoes = new AparenciaBotoes[]
                    {
                        new AparenciaBotoes
                        {
                            CorFundo = Color.White,
                            CorTexto = Color.Black,
                            CorContrasteBorda = Color.Black,
                            TamanhoContrasteBorda = 5,
                            IntensidadeArredondarBorda = 5,
                        }
                    }
                },
                new Aparencia // Tema Escuro
                {
                    NomeTema = "Escuro",
                    CorFundo = Color.Red,
                    CorFundoTransparente = true,
                    CaminhoImagemDeFundo = null,
                    CorContrasteBorda = Color.White,
                    TamanhoContrasteBorda = 5,
                    IntensidadeArredondarBorda = 5,
                    VisualBotoes = new AparenciaBotoes[]
                    {
                        new AparenciaBotoes
                        {
                            CorFundo = Color.Black,
                            CorTexto = Color.White,
                            CorContrasteBorda = Color.White,
                            TamanhoContrasteBorda = 5,
                            IntensidadeArredondarBorda = 5,
                        }
                    }
                }
            }
        },
        Dados = new DadosBtnMudarEsconder_MostrarMenu
        {
            PosicaoSuperior = false,
            PosicaoEsquerda = true,
            TextoA = "Mostrar",
            TextoB = "Esconder",
            TextoC = "Fecha a janela"
        }
    };

    // CONFIGURAÇÃO PADRÃO PARA O MENU
    ConfiguracoesGlobais.Config_Menu = new JanelaMenu
    {
        Base = new BaseJanela
        {
            Titulo = "Menu",
            ExibirBarraTitulo = false,
            AparecerNaBarraDeTarefas = false,
            UsaIcone = false,
            CaminhoIcone = null,
            Margem = 20,
            Fonte = new Fonte
            {
                Font = new Font("Arial", 30, FontStyle.Bold),
                TamanhoMaximo = 40,
                TamanhoMinimo = 10,
            },
            Escala = new Escala
            {
                Fixa = false,
                ValorFixo = 0.7,
                Maxima = 0.7,
                Minima = 1.5,
                MultiplicadorEscalaFonte = 1.3,
                Resolucao = new Resolucao
                {
                    Maxima = new int[] { 1920, 1080 },
                    Minima = new int[] { 800, 480 }
                },
            },
            TemaVisual = new Aparencia[]
            {
                new Aparencia // Tema Claro para o Menu
                {
                    NomeTema = "Claro",
                    CorFundo = Color.Fuchsia,
                    CorFundoTransparente = true,
                    CaminhoImagemDeFundo = null,
                    CorContrasteBorda = Color.White,
                    TamanhoContrasteBorda = 0,
                    IntensidadeArredondarBorda = 0,
                    VisualBotoes = new AparenciaBotoes[]
                    {
                        new AparenciaBotoes
                        {
                            CorFundo = Color.White,
                            CorTexto = Color.Black,
                            CorContrasteBorda = Color.Black,
                            TamanhoContrasteBorda = 3,
                            IntensidadeArredondarBorda = 5,
                        }
                    }
                },
                new Aparencia // Tema Escuro para o Menu
                {
                    NomeTema = "Escuro",
                    CorFundo = Color.Fuchsia,
                    CorFundoTransparente = true,
                    CaminhoImagemDeFundo = null,
                    CorContrasteBorda = Color.Fuchsia,
                    TamanhoContrasteBorda = 1,
                    IntensidadeArredondarBorda = 1,
                    VisualBotoes = new AparenciaBotoes[]
                    {
                        new AparenciaBotoes
                        {
                            CorFundo = Color.Black,
                            CorTexto = Color.White,
                            CorContrasteBorda = Color.White,
                            TamanhoContrasteBorda = 5,
                            IntensidadeArredondarBorda = 5,
                        }
                    }
                }
            }
        },
        Dados = new DadosMenu
        {
            BotoesSempreVisiveis = false,
            TelaInteira = true,
            BararPosicaoSuperior = false,
            BarraPosicaoDireita = false,
            TamanhoBotoesFixo = false,
            PercentagemEcra = 75,
            AlturaBotoes = 72,
            BotoesPorLinha = 4,
            QuantidadeBotoes = 5,
            Botoes = new FuncaoBotao[]
            {
                new FuncaoBotao
                {
                    Nome = "Explicação \nWifi",
                    Tipo = "Forms",
                    Diretorio_Link = "PassoAPasso[0]",
                },
                new FuncaoBotao
                {
                    Nome = "Esqueci-me \nda Senha",
                    Tipo = "Link",
                    Diretorio_Link = "https://web.novalaw.unl.pt/Help.asp",
                },
                new FuncaoBotao
                {
                    Nome = "Office \n365",
                    Tipo = "PDF",
                    Diretorio_Link = "AjudaOffice.pdf",
                },
                new FuncaoBotao
                {
                    Nome = "Retirar \nSenha",
                    Tipo = "Link",
                    Diretorio_Link = "https://hub.novalaw.pt/senhas?lang=pt",
                },
                new FuncaoBotao
                {
                    Nome = "Abrir\nConfigurações",
                    Tipo = "Forms",
                    Diretorio_Link = "Config",
                }
            }
        }
    };
    // CONFIGURAÇÃO PADRÃO PARA O PASSO A PASSO – cria um array com um único elemento
    ConfiguracoesGlobais.Config_PassoAPasso = new JanelaPassoAPasso
    {
        Base = new BaseJanela
        {
            Titulo = "PassoAPasso",
            Margem = 2.3,
            ExibirBarraTitulo = false,
            AparecerNaBarraDeTarefas = false,
            UsaIcone = false,
            CaminhoIcone = null,
            Fonte = new Fonte
            {
                Font = new Font("Arial", 18, FontStyle.Bold),
                TamanhoMaximo = 23,
                TamanhoMinimo = 8,
            },
            Escala = new Escala
            {
                Fixa = false,
                ValorFixo = 1.1,
                Maxima = 2.1,
                Minima = 1.1,
                MultiplicadorEscalaFonte = 0,
                Resolucao = new Resolucao
                {
                    Maxima = new int[] { 1920, 1080 },
                    Minima = new int[] { 800, 480 },
                },
            },
            TemaVisual = new Aparencia[]
            {
                new Aparencia  // Tema Claro para Passo a Passo
                {
                    NomeTema = "Claro",
                    CorFundo = Color.White,
                    CorFundoTransparente = false,
                    CaminhoImagemDeFundo = null,
                    CorContrasteBorda = Color.Black,
                    TamanhoContrasteBorda = 1,
                    IntensidadeArredondarBorda = 5,
                    VisualBotoes = new AparenciaBotoes[]
                    {
                        new AparenciaBotoes
                        {
                            CorFundo = Color.White,
                            CorTexto = Color.Black,
                            CorContrasteBorda = Color.Black,
                            TamanhoContrasteBorda = 4,
                            IntensidadeArredondarBorda = 7,
                        },
                        new AparenciaBotoes
                        {
                            CorFundo = Color.White,
                            CorTexto = Color.Black,
                            CorContrasteBorda = Color.Black,
                            TamanhoContrasteBorda = 0,
                            IntensidadeArredondarBorda = 0,
                        }
                    }
                },
                new Aparencia  // Tema Escuro para Passo a Passo
                {
                    NomeTema = "Escuro",
                    CorFundo = Color.Red,
                    CorFundoTransparente = false,
                    CaminhoImagemDeFundo = null,
                    CorContrasteBorda = Color.White,
                    TamanhoContrasteBorda = 1,
                    IntensidadeArredondarBorda = 5,
                    VisualBotoes = new AparenciaBotoes[]
                    {
                        new AparenciaBotoes
                        {
                            CorFundo = Color.Black,
                            CorTexto = Color.White,
                            CorContrasteBorda = Color.White,
                            TamanhoContrasteBorda = 4,
                            IntensidadeArredondarBorda = 7,
                        },
                        new AparenciaBotoes
                        {
                            CorFundo = Color.Black,
                            CorTexto = Color.White,
                            CorContrasteBorda = Color.White,
                            TamanhoContrasteBorda = 0,
                            IntensidadeArredondarBorda = 0,
                        }
                    }
                }
            }
        },
        Dados = new DadosPassoAPasso[]
        {
            new DadosPassoAPasso
            {
                Titulo = "Configurar Wifi",
                Imagens0 = new string[] { "Windows","Win", "jpg" },
                Imagens1 = new string[] { "MacBook","Mac", "png" },
                Imagens2 = new string[] { "Android","Adr", "jpg" },
            }
        }
    };

    // Ajusta os VisualBotoes para o MENU
    AparenciaBotoes tema1 = ConfiguracoesGlobais.Config_Menu.Base.TemaVisual[0].VisualBotoes[0];
    AparenciaBotoes tema2 = ConfiguracoesGlobais.Config_Menu.Base.TemaVisual[1].VisualBotoes[0];
    ConfiguracoesGlobais.Config_Menu.Base.TemaVisual[0].VisualBotoes = new AparenciaBotoes[ConfiguracoesGlobais.Config_Menu.Dados.QuantidadeBotoes];
    ConfiguracoesGlobais.Config_Menu.Base.TemaVisual[1].VisualBotoes = new AparenciaBotoes[ConfiguracoesGlobais.Config_Menu.Dados.QuantidadeBotoes];
    for (int i = 0; i < ConfiguracoesGlobais.Config_Menu.Dados.QuantidadeBotoes; i++)
    {
        ConfiguracoesGlobais.Config_Menu.Base.TemaVisual[0].VisualBotoes[i] = tema1;
        ConfiguracoesGlobais.Config_Menu.Base.TemaVisual[1].VisualBotoes[i] = tema2;
    }
}
// Atualizações – se algum valor for carregado no JSON, atualiza os campos da configuração atual
private static void AtualizarConfigPrincipal(BtnMudarEsconder_MostrarMenu BtnMudarEsconder_MostrarMenuCarregado)
{
    if (BtnMudarEsconder_MostrarMenuCarregado != null)
        ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu = BtnMudarEsconder_MostrarMenuCarregado;
}
private static void AtualizarConfigMenu(JanelaMenu menuCarregado)
{
    if (menuCarregado != null)
        ConfiguracoesGlobais.Config_Menu = menuCarregado;
}
private static void AtualizarConfigPassoAPasso(JanelaPassoAPasso passosCarregados)
{
    if (passosCarregados != null && passosCarregados.Dados.Length > 0)
        ConfiguracoesGlobais.Config_PassoAPasso = passosCarregados;
}
// Subprograma para converter e reconstruir a fonte (usando o novo struct Fonte)
private static void AtualizarFontParaArray(ref Fonte fonte)
{
    try
    {
        if (fonte.Font != null)
        {
            fonte.FonteArray = new string[3];
            fonte.FonteArray[0] = fonte.Font.Name;
            fonte.FonteArray[1] = fonte.Font.Size.ToString(CultureInfo.InvariantCulture);
            fonte.FonteArray[2] = fonte.Font.Style.ToString();
        }
        else
            fonte.FonteArray = new string[] { "Arial", "12", "Regular" };
    }
    catch
    {
        fonte.FonteArray = new string[] { "Arial", "12", "Regular" };
    }
}
private static void ReconstruirFonte(ref Fonte fonte)
{
    try
    {
        if (fonte.FonteArray != null && fonte.FonteArray.Length >= 3)
        {
            string nome = fonte.FonteArray[0].Trim();
            float tamanho = float.Parse(fonte.FonteArray[1], CultureInfo.InvariantCulture);
            FontStyle estilo = (FontStyle)Enum.Parse(typeof(FontStyle), fonte.FonteArray[2].Trim());
            fonte.Font = new Font(nome, tamanho, estilo);
        }
        else
            throw new Exception("FonteArray inválido.");
    }
    catch
    {
        fonte.Font = SystemFonts.DefaultFont;
        fonte.FonteArray = new string[]
        {
            fonte.Font.Name,
            fonte.Font.Size.ToString(CultureInfo.InvariantCulture),
            fonte.Font.Style.ToString()
        };
    }
}
*/


/*
public static class ValidacoesConfig
{
    public static Fonte ValidarFonte(Fonte fonte)
    {
        // Se a fonte não estiver definida, usa a fonte padrão do sistema.
        if (fonte.Font == null)
            fonte.Font = SystemFonts.DefaultFont;
        // Se o array de fonte estiver nulo ou vazio, usa um valor padrão.
        if (fonte.FonteArray == null || fonte.FonteArray.Length != 3)
            fonte.FonteArray = new string[] { "Arial", "12", "Regular" };
        // Define valores padrão para os tamanhos, se inválidos.
        if (fonte.TamanhoMaximo <= 0)
            fonte.TamanhoMaximo = 20; // Exemplo de valor padrão
        if (fonte.TamanhoMinimo <= 0)
            fonte.TamanhoMinimo = 8;
        if (fonte.MultiplicadorEscala <= 0)
            fonte.MultiplicadorEscala = 1.0;
        return fonte;
    }
    public static Escala ValidarEscala(Escala escala)
    {
        // Se a escala for fixa e o valor fixo for inválido, define 1.0.
        if (escala.Fixa && escala.ValorFixo <= 0)
            escala.ValorFixo = 1.0;
        if (escala.Maxima <= 0)
            escala.Maxima = 1.0;
        if (escala.Minima <= 0)
            escala.Minima = 0.5;
        // Valida a resolução.
        escala.Resolucao = ValidarResolucao(escala.Resolucao);
        return escala;
    }
    public static Resolucao ValidarResolucao(Resolucao resolucao)
    {
        // Espera-se que os arrays tenham dois elementos: [largura, altura].
        if (resolucao.Maxima == null || resolucao.Maxima.Length < 2)
            resolucao.Maxima = new int[] { 1920, 1080 };
        if (resolucao.Minima == null || resolucao.Minima.Length < 2)
            resolucao.Minima = new int[] { 800, 480 };
        return resolucao;
    }
    public static Aparencia ValidarAparencia(Aparencia aparencia)
    {
        if (string.IsNullOrEmpty(aparencia.NomeTema))
            aparencia.NomeTema = "Padrao";
        if (aparencia.CaminhoImagemDeFundo == null)
            aparencia.CaminhoImagemDeFundo = "";
        if (aparencia.CorFundo == Color.Empty)
            aparencia.CorFundo = Color.White;
        if (aparencia.TamanhoContrasteBorda <= 0)
            aparencia.TamanhoContrasteBorda = 1;
        // Valida os VisualBotoes; se nulo ou vazio, cria um array com um valor padrão.
        if (aparencia.VisualBotoes == null || aparencia.VisualBotoes.Length == 0)
            aparencia.VisualBotoes = new AparenciaBotoes[] { ValidarAparenciaBotoes(new AparenciaBotoes()) };
        else
        {
            for (int i = 0; i < aparencia.VisualBotoes.Length; i++)
            {
                aparencia.VisualBotoes[i] = ValidarAparenciaBotoes(aparencia.VisualBotoes[i]);
            }
        }
        return aparencia;
    }
    public static AparenciaBotoes ValidarAparenciaBotoes(AparenciaBotoes ab)
    {
        if (ab.TamanhoContrasteBorda <= 0)
            ab.TamanhoContrasteBorda = 1;
        if (ab.CorContrasteBorda == Color.Empty)
            ab.CorContrasteBorda = Color.Black;
        if (ab.CorTexto == Color.Empty)
            ab.CorTexto = Color.Black;
        if (ab.CorFundo == Color.Empty)
            ab.CorFundo = Color.White;
        return ab;
    }
    public static FuncaoBotao ValidarFuncaoBotao(FuncaoBotao fb)
    {
        if (string.IsNullOrEmpty(fb.Nome))
            fb.Nome = "Botao";
        if (string.IsNullOrEmpty(fb.Tipo))
            fb.Tipo = "Tipo";
        if (string.IsNullOrEmpty(fb.Diretorio_Link))
            fb.Diretorio_Link = "";
        return fb;
    }
    public static BaseJanela ValidarBaseJanela(BaseJanela baseJanela)
    {
        if (string.IsNullOrEmpty(baseJanela.Titulo))
            baseJanela.Titulo = "Janela";
        if (baseJanela.Margem <= 0)
            baseJanela.Margem = 0;
        if (baseJanela.CaminhoIcone == null)
            baseJanela.CaminhoIcone = "";
        baseJanela.Fonte = ValidarFonte(baseJanela.Fonte);
        baseJanela.Escala = ValidarEscala(baseJanela.Escala);
        if (baseJanela.TemaVisual == null || baseJanela.TemaVisual.Length == 0)
            baseJanela.TemaVisual = new Aparencia[] { ValidarAparencia(new Aparencia()) };
        else
        {
            for (int i = 0; i < baseJanela.TemaVisual.Length; i++)
            {
                baseJanela.TemaVisual[i] = ValidarAparencia(baseJanela.TemaVisual[i]);
            }
        }
        return baseJanela;
    }
    public static DadosBtnMudarEsconder_MostrarMenu ValidarDadosPrincipal(DadosBtnMudarEsconder_MostrarMenu dp)
    {
        if (string.IsNullOrEmpty(dp.TextoA))
            dp.TextoA = "TextoA";
        if (string.IsNullOrEmpty(dp.TextoB))
            dp.TextoB = "TextoB";
        if (string.IsNullOrEmpty(dp.TextoC))
            dp.TextoC = "TextoC";
        return dp;
    }
    public static DadosMenu ValidarDadosMenu(DadosMenu dm)
    {
        if (dm.PercentagemEcra <= 0)
            dm.PercentagemEcra = 100;
        if (dm.AlturaBotoes <= 0)
            dm.AlturaBotoes = 50;
        if (dm.BotoesPorLinha <= 0)
            dm.BotoesPorLinha = 1;
        if (dm.QuantidadeBotoes <= 0)
            dm.QuantidadeBotoes = 1;
        if (dm.Botoes == null || dm.Botoes.Length == 0)
            dm.Botoes = new FuncaoBotao[] { ValidarFuncaoBotao(new FuncaoBotao()) };
        else
        {
            for (int i = 0; i < dm.Botoes.Length; i++)
            {
                dm.Botoes[i] = ValidarFuncaoBotao(dm.Botoes[i]);
            }
        }
        return dm;
    }
    public static DadosPassoAPasso ValidarDadosPassoAPasso(DadosPassoAPasso dp)
    {
        if (dp.CorTexto == Color.Empty)
            dp.CorTexto = Color.Black;
        if (dp.Imagens1 == null)
            dp.Imagens1 = new string[0];
        if (dp.Imagens2 == null)
            dp.Imagens2 = new string[0];
        if (dp.Imagens3 == null)
            dp.Imagens3 = new string[0];
        return dp;
    }
}
*/
/*
        // Para o Passo a Passo, como agora temos um único objeto com um array de Dados:
        public static void ConfigurarPassoAPasso(PassoAPasso passoAPasso, int indice)
        {
            if (ConfiguracoesGlobais.Menu.PassoAPasso == null ||
                ConfiguracoesGlobais.Menu.PassoAPasso == null ||
                indice < 0 || indice >= ConfiguracoesGlobais.Menu.PassoAPasso.Count-1)
            {
                MessageBox.Show($"Erro: O índice {indice} não existe em Config_PassoAPasso.Dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passoAPasso.Close();
                return;
            }
            AjustesVisuaisPassoAPasso(passoAPasso);
        }
        public static void AjustesVisuaisPassoAPasso(PassoAPasso form)
        {
            if (form is PassoAPasso passoAPasso)
            {
                int id = 0; // O ID do dado a editar é armazenado em passoAPasso.Tag (como índice)
                if (passoAPasso.Tag != null && int.TryParse(passoAPasso.Tag.ToString(), out int resultado))
                    id = resultado;

                if (ConfiguracoesGlobais.Menu.PassoAPasso == null ||
                    ConfiguracoesGlobais.Menu.PassoAPasso == null ||
                    id < 0 || id >= ConfiguracoesGlobais.Menu.PassoAPasso.Count - 1)
                {
                    MessageBox.Show($"Erro: O índice {id} não existe em Config_PassoAPasso.Dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    passoAPasso.Close();
                    return;
                }

                // Obtém o dado específico e usa a Base da JanelaPassoAPasso para os ajustes
                PassoAPasso_Dados ConfigPassoAPassoAtual = ConfiguracoesGlobais.Menu.PassoAPasso[id];
                form.Text = ConfigPassoAPassoAtual.Titulo;

                foreach (Control control in passoAPasso.Controls)
                {
                    if (control is Button)
                    {
                        switch (control.Name) // ultimo caracter
                        {
                            case "btn0": control.Text = ConfigPassoAPassoAtual.Imagens[0].NomeCompleto; break;
                            case "btn1": control.Text = ConfigPassoAPassoAtual.Imagens[1].NomeCompleto; break;
                            case "btn2": control.Text = ConfigPassoAPassoAtual.Imagens[2].NomeCompleto; break;
                            default: break; 
                        }
                    }

                }

                // Dentro do Subprograma AjustesVisuaisMenuPassoAPasso(PassoAPasso form)
                string caminhoImagens = Path.Combine(FicheirosJson.Caminho, "ImagensPassoAPasso", id.ToString());
                if (!Directory.Exists(caminhoImagens))
                {
                    MessageBox.Show($"A pasta {caminhoImagens} não existe. Por favor, carregue as imagens necessárias para essa pasta.",
                        "Pasta não encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passoAPasso.Close();
                    return;
                }

                // Obter os arquivos que correspondem aos padrões definidos em Dados
                Image[][] ListaImagens = new Image[][]
                {
                    SalvarImagens(Directory.GetFiles(caminhoImagens,ConfigPassoAPassoAtual.Imagens[0].NomeFicheiro + "." + ConfigPassoAPassoAtual.Imagens[0].FormatoFicheiro)),
                    SalvarImagens(Directory.GetFiles(caminhoImagens,ConfigPassoAPassoAtual.Imagens[1].NomeFicheiro + "." + ConfigPassoAPassoAtual.Imagens[1].FormatoFicheiro)),
                    SalvarImagens(Directory.GetFiles(caminhoImagens,ConfigPassoAPassoAtual.Imagens[2].NomeFicheiro + "." + ConfigPassoAPassoAtual.Imagens[2].FormatoFicheiro))
                };
                // Verificar se as imagens já estão carregadas (comparando apenas a quantidade, 
                // podendo ser ampliada para comparar os nomes se necessário)
                bool recarregar = true;
                if (passoAPasso.ListaTotal != null &&
                    passoAPasso.ListaTotal.Length == 3 &&
                    passoAPasso.ListaTotal[0].Length == ListaImagens[0].Length &&
                    passoAPasso.ListaTotal[1].Length == ListaImagens[1].Length &&
                    passoAPasso.ListaTotal[2].Length == ListaImagens[2].Length)
                {
                    recarregar = false;
                }

                if (recarregar)
                    passoAPasso.ListaTotal = ListaImagens;
                passoAPasso.CarregarImagens();
            }
        }
        public static Image[] SalvarImagens(string[] arquivos)
        {
            Image[] imagens = new Image[arquivos.Length];
            for (int i = 0; i < arquivos.Length; i++)
                imagens[i] = Image.FromFile(arquivos[i]);
            return imagens;
        }
        public static void AjustesVisuaisBotoesMenu(Menu menu)
        {
            if (menu == null)
                return;

            AparenciaControlos tema = ConfigManager.Instancia.Config.Aparencia.TemaVisual[ConfigManager.Instancia.Config.IndiceTemaAtual].ObterAparenciaDoBotao(menu, (string)menu.BtnMudarEsconder_MostrarMenu.Tag)];

            if (menu.BtnMudarEsconder_MostrarMenu != null)
            {
                if (TamanhoInicialBotao == Size.Empty)
                    TamanhoInicialBotao = menu.BtnMudarEsconder_MostrarMenu.Size;
                menu.BtnMudarEsconder_MostrarMenu.Size = new Size(
                    (int)(TamanhoInicialBotao.Width * escala),
                    (int)(TamanhoInicialBotao.Height * escala)
                );
                menu.BtnMudarEsconder_MostrarMenu.BackColor = tema.CorFundo; ///
                menu.BtnMudarEsconder_MostrarMenu.ForeColor = tema.CorTexto;
                menu.BtnMudarEsconder_MostrarMenu.Font = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Fonte.Font;
                int posX = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Dados.PosicaoEsquerda
                    ? Screen.PrimaryScreen.WorkingArea.Right - menu.BtnMudarEsconder_MostrarMenu.Width - margem
                    : Screen.PrimaryScreen.WorkingArea.Left + margem;
                int posY = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Dados.PosicaoSuperior
                    ? Screen.PrimaryScreen.WorkingArea.Top + margem
                    : Screen.PrimaryScreen.WorkingArea.Bottom - menu.BtnMudarEsconder_MostrarMenu.Height - margem;
                menu.BtnMudarEsconder_MostrarMenu.Location = new Point(posX, posY);
                menu.BtnMudarEsconder_MostrarMenu.ArredondarBorda(tema.IntensidadeArredondarBorda, tema.TamanhoContrasteBorda, tema.CorContrasteBorda);
            }

            if (menu.BtnSair != null && menu.BtnMudarEsconder_MostrarMenu != null)
            {
                menu.BtnSair.Size = menu.BtnMudarEsconder_MostrarMenu.Size;
                menu.BtnSair.BackColor = tema.CorFundo;
                menu.BtnSair.ForeColor = tema.CorTexto;
                menu.BtnSair.Font = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Base.Fonte.Font;
                int posXSair = ConfiguracoesGlobais.Config_BtnMudarEsconder_MostrarMenu.Dados.PosicaoEsquerda
                    ? Screen.PrimaryScreen.WorkingArea.Left + margem
                    : Screen.PrimaryScreen.WorkingArea.Right - menu.BtnSair.Width - margem;
                menu.BtnSair.Location = new Point(posXSair, menu.BtnMudarEsconder_MostrarMenu.Location.Y);
                menu.BtnSair.ArredondarBorda(tema.IntensidadeArredondarBorda, tema.TamanhoContrasteBorda, tema.CorContrasteBorda);
            }
        }
        public static void AjustesVisuaisMenu(Menu form)
        {
            if (form is Menu menu)
            {
                AparenciaControlos tema = ConfigManager.Instancia.Config.Aparencia.TemaVisual[ConfigManager.Instancia.Config.IndiceTemaAtual].ObterAparenciaDoBotao(menu, (string)menu.btn0.Tag);

                // Use os botões definidos em Dados
                Button[] arrayBotoes = new Button[] { menu.btn0, menu.btn1, menu.btn2, menu.btn3, menu.btn4, menu.btn5, menu.btn6, menu.btn7 }; // Fazer isto identificar os botões que acabam com um numero
                int quantidadeBotoesConfigurada = ConfiguracoesGlobais.Config_Menu.Dados.QuantidadeBotoes;
                if (quantidadeBotoesConfigurada > arrayBotoes.Length)
                    quantidadeBotoesConfigurada = arrayBotoes.Length;

                int botoesPorLinha = Math.Min(ConfiguracoesGlobais.Config_Menu.Dados.BotoesPorLinha, quantidadeBotoesConfigurada);
                if (botoesPorLinha == 0)
                    botoesPorLinha = 1;

                int numeroLinhas = Math.Min((int)Math.Ceiling((double)quantidadeBotoesConfigurada / botoesPorLinha), 4);
                int espacamento = (int)(ConfiguracoesGlobais.Config_Menu.Base.Margem * escala);
                int larguraBotao, alturaBotao;

                int alturaDisponivel = menu.ClientSize.Height - (numeroLinhas + 1) * espacamento;
                int alturaBotaoMaxima = alturaDisponivel / numeroLinhas;
                if (alturaBotaoMaxima <= 0 || espacamento > menu.ClientSize.Height / 4)
                {
                    espacamento = Math.Max(5, menu.ClientSize.Height / (2 * numeroLinhas));
                    alturaDisponivel = menu.ClientSize.Height - (numeroLinhas + 1) * espacamento;
                }
                alturaBotaoMaxima = alturaDisponivel / numeroLinhas;

                if (ConfiguracoesGlobais.Config_Menu.Dados.TamanhoBotoesFixo)
                {
                    alturaBotao = Math.Min((int)(ConfiguracoesGlobais.Config_Menu.Dados.AlturaBotoes * escala), alturaBotaoMaxima);
                    larguraBotao = (menu.ClientSize.Width - espacamento * (botoesPorLinha + 1)) / botoesPorLinha;
                }
                else
                {
                    int alturaAreaBotoes = menu.ClientSize.Height * ConfiguracoesGlobais.Config_Menu.Dados.PercentagemEcra / 100;
                    alturaBotao = (numeroLinhas == 1) ? alturaAreaBotoes : (alturaAreaBotoes - espacamento * (numeroLinhas + 1)) / numeroLinhas;
                    larguraBotao = (menu.ClientSize.Width - espacamento * (botoesPorLinha + 1)) / botoesPorLinha;
                }

                int alturaTotalBotoes = (alturaBotao + espacamento) * numeroLinhas;
                if (alturaTotalBotoes > menu.ClientSize.Height)
                    alturaBotao = (menu.ClientSize.Height - (numeroLinhas + 1) * espacamento) / numeroLinhas;

                int margemVertical = 0;
                int linhaAtual = 0, colunaAtual = 0;
                for (int i = 0; i < quantidadeBotoesConfigurada; i++)
                {
                    int posicaoX = espacamento + colunaAtual * (larguraBotao + espacamento);
                    int posicaoY = margemVertical + espacamento + linhaAtual * (alturaBotao + espacamento);
                    arrayBotoes[i].Size = new Size(larguraBotao, alturaBotao);
                    arrayBotoes[i].Location = new Point(posicaoX, posicaoY);
                    arrayBotoes[i].Visible = true;
                    colunaAtual++;
                    if (colunaAtual >= botoesPorLinha)
                    {
                        colunaAtual = 0;
                        linhaAtual++;
                    }
                }

                //menu.TransparencyKey = temaMenu.CorFundoTransparente;
                //menu.BackColor = temaMenu.CorFundo;

                for (int i = 0; i < arrayBotoes.Length; i++)
                {
                    if (i < quantidadeBotoesConfigurada)
                        arrayBotoes[i].Text = ConfiguracoesGlobais.Config_Menu.Dados.Botoes[i].Nome;
                    else
                        arrayBotoes[i].Visible = false;
                }
                AjustesVisuaisBotoesMenu(menu);
            }
        }
*/

/* COnfigurações 25/2, nova e antiga, antes de tentar converter a tree node em classe no fim
using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Collections;

namespace PerguntasFrequentesSuporte
{
    public partial class Configuracoes : Form 
        // Erro que não dá para editar listas ou arrays porque não encontra o pai,
        // quando se acede ao valor de alguma sub arvore o caminho fica apenas como subArvore.Variável e não como Arvore.SubArvore.Variável
        // falta fazer apresentar dicionários, onde cada chave aparece como um subvalor e o valor que se edita é o da valor associado á chave, e nunca se edita a chave em si
    {
        public Configuracoes()
        {
            InitializeComponent(); // Deve ser a primeira instrução do construtor

        }
        public class NodeMetadata
        {
            public object Valor { get; set; }             // Valor atual da propriedade
            public PropertyInfo Propriedade { get; set; } // Referência à propriedade, se aplicável
            public string Descricao { get; set; }
            public object Pai { get; set; }               // Objeto pai (AppConfig, Dicionário, Lista)
            public string NomeOriginal { get; set; }      // Nome da propriedade ou item
        }
        private void TreeViewConfig_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is NodeMetadata metadata)
            {
                object novoValor = AbrirInputBoxPorTipo(metadata); // Usa o InputBox correto com base no tipo

                if (novoValor != null && !novoValor.Equals(metadata.Valor))
                {
                    // Atualizar o objeto na classe de configuração
                    if (metadata.Propriedade != null)
                    {
                        metadata.Propriedade.SetValue(metadata.Pai, novoValor);
                    }
                    else if (metadata.Pai is IDictionary dictPai)
                    {
                        dictPai[metadata.NomeOriginal] = novoValor;
                    }
                    else if (metadata.Pai is IList listPai)
                    {
                        int index = int.Parse(metadata.NomeOriginal.Replace("Item ", ""));
                        listPai[index] = novoValor;
                    }

                    // Atualizar o nó da TreeView
                    metadata.Valor = novoValor;
                    e.Node.Tag = metadata; // Atualiza a referência
                    e.Node.Text = ObterTextoParaNo(metadata); // Função para atualizar o texto
                }
            }
        }
        private object AbrirInputBoxPorTipo(NodeMetadata metadata)
        {
            Type tipo = metadata.Valor.GetType();

            switch (Type.GetTypeCode(tipo))
            {
                case TypeCode.Int32:
                    return AbrirInputBoxInt32((int)metadata.Valor);
                case TypeCode.Int64:
                    return AbrirInputBoxInt64((long)metadata.Valor);
                case TypeCode.Double:
                    return AbrirInputBoxDouble((double)metadata.Valor);
                case TypeCode.String:
                    return AbrirInputBoxString(metadata.Valor.ToString());
                case TypeCode.Boolean:
                    return AbrirInputBoxBoolean((bool)metadata.Valor);
                case TypeCode.Object:
                    if (metadata.Valor is Color cor)
                        return InputBoxColor.Show($"Introduza o novo valor para a cor {metadata.NomeOriginal}",cor);
                    if (metadata.Valor is Font fonte)
                        return InputBoxFont.Show($"Introduza o novo valor para a fonte {metadata.NomeOriginal}",fonte);
                    break;
            }

            MessageBox.Show($"O tipo {tipo.Name} não é suportado para edição.");
            return null;
        }
        private int AbrirInputBoxInt32(int valorAtual)
        {
            string input = Interaction.InputBox("Novo valor (Int32):", "Editar Valor", valorAtual.ToString());
            return int.TryParse(input, out int resultado) ? resultado : valorAtual;
        }
        private long AbrirInputBoxInt64(long valorAtual)
        {
            string input = Interaction.InputBox("Novo valor (Int64):", "Editar Valor", valorAtual.ToString());
            return long.TryParse(input, out long resultado) ? resultado : valorAtual;
        }
        private double AbrirInputBoxDouble(double valorAtual)
        {
            string input = Interaction.InputBox("Novo valor (Double):", "Editar Valor", valorAtual.ToString());
            return double.TryParse(input, out double resultado) ? resultado : valorAtual;
        }
        private string AbrirInputBoxString(string valorAtual)
        {
            return Interaction.InputBox("Novo texto:", "Editar Valor", valorAtual);
        }
        private bool AbrirInputBoxBoolean(bool valorAtual)
        {
            DialogResult resultado = MessageBox.Show("Ativar valor?", "Editar Booleano", MessageBoxButtons.YesNo);
            return resultado == DialogResult.Yes;
        }
        private string ObterTextoParaNo(NodeMetadata metadata)
        {
            if (metadata.Valor is Color cor)
            {
                string nomeCor = cor.IsNamedColor ? cor.Name : "";
                return $"{metadata.NomeOriginal}: R:{cor.R} G:{cor.G} B:{cor.B} {(string.IsNullOrWhiteSpace(nomeCor) ? "" : $"({nomeCor})")}";
            }

            if (metadata.Valor is Font fonte)
                return $"{metadata.NomeOriginal}: Estilo: {fonte.Style} | Tamanho: {fonte.Size} | Família: {fonte.FontFamily.Name}";

            return $"{metadata.NomeOriginal}: {metadata.Valor}";
        }

        // Supondo que tens uma variável global para a configuração:
        private AppConfig appConfigInstance;

        // Evento de carregamento do formulário
        private void Configuracoes_Load(object sender, EventArgs e)
        {
            // Carrega a configuração a partir de um ficheiro ou cria um novo objeto padrão
            appConfigInstance = AcederConfig.ConfigAtual.AppConfig;

            // Popula a TreeView com a configuração
            PopularTreeView(appConfigInstance, TreeViewConfig);

            // Se tens um ComboBox para selecionar temas, por exemplo:
            comboBox1.Items.Clear();
            foreach (var tema in appConfigInstance.VisualAplicacao.Tema)
            {
                comboBox1.Items.Add(tema.NomeTema);
            }
            // Seleciona o tema atual no ComboBox (caso haja)
            if (appConfigInstance.IndiceTemaAtual < comboBox1.Items.Count)
                comboBox1.SelectedIndex = appConfigInstance.IndiceTemaAtual;
        }
        // Evento disparado quando um nó é selecionado na TreeView
        private void TreeViewConfig_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBoxDetalhes.Items.Clear();

            if (e.Node.Tag is NodeMetadata metadata)
            {
                listBoxDetalhes.Items.Add("Nome: " + metadata.NomeOriginal);

                if (metadata.Valor != null)
                    listBoxDetalhes.Items.Add("Valor: " + metadata.Valor.ToString());
                else
                    listBoxDetalhes.Items.Add("Valor: null");

                if (metadata.Propriedade != null)
                {
                    listBoxDetalhes.Items.Add("Tipo: " + metadata.Propriedade.PropertyType.Name);

                    var descAttr = metadata.Propriedade.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (descAttr != null)
                        listBoxDetalhes.Items.Add("Descrição: " + descAttr.Description);
                }

                // Se for uma subclasse, mostrar as suas propriedades como detalhes
                if (metadata.Valor != null)
                {
                    Type valorType = metadata.Valor.GetType();
                    if (!valorType.IsPrimitive && !(metadata.Valor is string))
                    {
                        PropertyInfo[] propriedades = valorType.GetProperties();
                        foreach (var prop in propriedades)
                        {
                            object valorSub = prop.GetValue(metadata.Valor);
                            if (valorSub != null)
                                listBoxDetalhes.Items.Add(prop.Name + ": " + valorSub.ToString());
                            else
                                listBoxDetalhes.Items.Add(prop.Name + ": null");
                        }
                    }
                }
            }
        }
        public void PopularTreeView(AppConfig config, TreeView treeView)
        {
            treeView.Nodes.Clear();
            TreeNode rootNode = new TreeNode("Configuração da Aplicação");

            foreach (PropertyInfo prop in config.GetType().GetProperties())
            {
                object valor = prop.GetValue(config);
                string nomeFormatado = FormatarNome(prop.Name);

                TreeNode node = new TreeNode(nomeFormatado);
                node.Tag = new NodeMetadata
                {
                    Valor = valor,
                    Propriedade = prop,
                    Descricao = ObterDescricao(prop),
                    NomeOriginal = prop.Name,
                    Pai = config
                };

                // Se o valor for uma coleção (lista/dicionário)
                if (valor is IDictionary dict)
                {
                    foreach (DictionaryEntry entry in dict)
                    {
                        TreeNode filho = new TreeNode($"{entry.Key}: {entry.Value}");
                        filho.Tag = new NodeMetadata
                        {
                            Valor = entry.Value,
                            NomeOriginal = entry.Key.ToString(),
                            Pai = valor
                        };
                        node.Nodes.Add(filho);
                    }
                }
                else if (valor is IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        TreeNode filho = new TreeNode($"Item {i}: {list[i]}");
                        filho.Tag = new NodeMetadata
                        {
                            Valor = list[i],
                            NomeOriginal = $"Item {i}",
                            Pai = valor
                        };
                        node.Nodes.Add(filho);
                    }
                }

                rootNode.Nodes.Add(node);
            }

            treeView.Nodes.Add(rootNode);
        }

        //CopiarNomesDosNos(treeView);
        private void CopiarNomesDosNos(TreeView treeView)
        {
            List<string> nomesDosNos = new List<string>();

            foreach (TreeNode node in treeView.Nodes)
            {
                ListarNomesRecursivamente(node, nomesDosNos);
            }

            // Concatena todos os nomes separados por nova linha
            string nomesConcatenados = string.Join(Environment.NewLine, nomesDosNos);

            // Copia para a área de transferência
            Clipboard.SetText(nomesConcatenados);

            // Exibe uma mensagem de confirmação
            MessageBox.Show("Os nomes dos nós foram copiados para a área de transferência.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ListarNomesRecursivamente(TreeNode node, List<string> nomes)
        {
            if (node == null)
                return;

            // Adiciona o nome do nó atual à lista
            nomes.Add(node.Text);

            // Percorre recursivamente os nós filhos
            foreach (TreeNode childNode in node.Nodes)
            {
                ListarNomesRecursivamente(childNode, nomes);
            }
        }
        private bool PropriedadePermitida(object valor)
        {
            if (valor == null)
                return false;

            Type tipo = valor.GetType();

           
            if (valor is IList || valor is IDictionary)  // Permite listas e dicionários, mas ignora propriedades internas como SyncRoot
                return true;

            List<Type> tiposPermitidos = new List<Type>
            {
                typeof(AppConfig),
                typeof(ConfigGeral),
                typeof(TamanhoInicialJanelas),
                typeof(TemaVisual),
                typeof(VisualBotoesPorJanela),
                typeof(TemaMenu),
                typeof(AparenciaGeral),
                typeof(FicheiroImagem),
                typeof(ConfigPassoAPasso),
                typeof(ConfigSair_Mostrar),
                typeof(FuncaoBotaoMenu)
            };

            if (tiposPermitidos.Contains(tipo)) // Verifica se o tipo está na lista de permitidos
                return true;

            return false;
        }
        private string FormatarNome(string nomeOriginal)
        {
            if (string.IsNullOrEmpty(nomeOriginal))
                return "";

            string resultado = "";
            for (int i = 0; i < nomeOriginal.Length; i++)
            {
                char c = nomeOriginal[i];
                if (char.IsUpper(c) && i > 0)
                    resultado += " ";
                resultado += c;
            }

            // Mantém a primeira letra maiúscula e o restante minúsculo
            return char.ToUpper(resultado[0]) + resultado.Substring(1).ToLower();
        }

        private string ObterDescricao(PropertyInfo prop)
        {
            // Tenta obter o atributo Description, se existir
            var descAttr = prop.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descAttr != null ? descAttr.Description : "";
        }
        // Evento de duplo clique na TreeView
        private void TreeViewConfig_DoubleClick(object sender, EventArgs e)
        {
            // Se houver um nó selecionado, chamamos o método de edição (já definido anteriormente)
            if (TreeViewConfig.SelectedNode != null)
            {
                // Reaproveita a lógica de edição definida no NodeMouseDoubleClick
                TreeViewConfig_DoubleClick(sender, new TreeNodeMouseClickEventArgs(TreeViewConfig.SelectedNode, MouseButtons.Left, 2, 0, 0));
            }
        }

        // Evento do botão Salvar (BtnSalvar_Click)
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            Ficheiros.IdPerfilAtual++;
            // Environment.SetEnvironmentVariable("CaminhoSalvamento", Caminho, EnvironmentVariableTarget.User);
            Ficheiros.SalvarConfig(Ficheiros.IdPerfilAtual, appConfigInstance);
            Environment.SetEnvironmentVariable("IdPerfilAtual", Ficheiros.IdPerfilAtual.ToString(), EnvironmentVariableTarget.User);
            Application.Restart();
        }

        // Evento do botão Close para fechar a janela
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Evento do ComboBox (ex.: para seleção de temas)
        private void comboBox1_Click(object sender, EventArgs e)
        {
            // Exemplo: Abrir uma caixa de diálogo ou atualizar algo dependendo do tema selecionado.
            // Aqui pode ser implementado a lógica para trocar o tema visual.
            MessageBox.Show("ComboBox clicado. Aqui podes implementar a lógica para selecionar outro tema.");
        }
}
}

        /* Tentativa de Subclasses
        static AppConfig ConfiguracoesGlobais = AcederConfig.ConfigAtual.AppConfig;
        private ConfigGeral tempConfig;
        private AparenciaGeral tempAparencia;
        private static Dictionary<string, string[]> escolhasPreDefinidas = new Dictionary<string, string[]>
        {
            { "FontStyle", new[] { "Regular", "Bold", "Italic", "Underline", "Strikeout" } }
        };
        public Configuracoes()
        {
            InitializeComponent();
            // Configuração dos controles (TreeView, ListBox, Botões) deve ser feita no designer ou aqui
            // this.Load += Configuracoes_Load;
        }
        private void Configuracoes_Load(object sender, EventArgs e)
        {
            tempConfig = ConfiguracoesGlobais.ConfiguracaoAplicacao;
            tempAparencia = ConfiguracoesGlobais.VisualAplicacao;

            //tempConfig_Btn.Base.Fonte = InputBoxFont.Show("Selecione a fonte", tempConfig_Btn.Base.Fonte);

            // Constrói a TreeView a partir dos objetos temporários
            TreeViewConfig.Nodes.Clear();

            TreeNode nodeBtn = new TreeNode("ConfiguracaoAplicacao");
            nodeBtn.Tag = new ConfigItem { Localizacao = "ConfiguracaoAplicacao", Objeto = tempConfig };
            TreeViewConfig.Nodes.Add(nodeBtn);
            AdicionarMembrosRecursivo(tempConfig, nodeBtn);

            TreeNode nodeMenu = new TreeNode("VisualAplicacao");
            nodeMenu.Tag = new ConfigItem { Localizacao = "VisualAplicacao", Objeto = tempAparencia };
            TreeViewConfig.Nodes.Add(nodeMenu);
            AdicionarMembrosRecursivo(tempAparencia, nodeMenu);

            TreeViewConfig.CollapseAll();
        }

        // Se o membro for indesejado (para evitar "lixo")
        private bool ShouldSkipMember(MemberInfo member)
        {
            string name = member.Name.ToLowerInvariant();
            if (name.Contains("syncroot") ||
                name.Contains("longlength") ||
                name == "length" ||
                name == "rank" ||
                name == "isreadonly" ||
                name == "isfixedsize" ||
                name == "issynchronized" ||
                name.EndsWith("value__"))
            {
                return true;
            }
            return false;
        }

        // Subprograma recursivo para adicionar membros à TreeView

        private void AdicionarMembrosRecursivo(object Objeto, TreeNode parent)
        {
            if (Objeto == null)
                return;

            Type tipo = Objeto.GetType();

            // Se o objeto for uma lista ou array, cria um nó e adiciona cada elemento
            if (Objeto is IEnumerable<object> lista)
            {
                TreeNode listaNode = new TreeNode($"{FormatMemberName(parent.Text)} (Lista)");
                listaNode.Tag = new ConfigItem { Localizacao = parent.Text, Objeto = Objeto, MemberInfo = null };
                parent.Nodes.Add(listaNode);

                int index = 0;
                foreach (var item in lista)
                {
                    TreeNode itemNode = new TreeNode($"[{index}] {item}");
                    itemNode.Tag = new ConfigItem { Localizacao = $"{parent.Text}[{index}]", Objeto = item, MemberInfo = null };
                    listaNode.Nodes.Add(itemNode);

                    // Se o item da lista for um objeto complexo, chama a recursão
                    if (item != null && !item.GetType().IsPrimitive && item.GetType() != typeof(string) && !(item is Color))
                        AdicionarMembrosRecursivo(item, itemNode);

                    index++;
                }
                return;
            }

            // Se for um objeto do tipo Fonte, trata-o como folha (mostrar apenas FonteArray)
            if (Objeto is Font)
                return;  // Exibe somente o FonteArray

            // Se for do tipo Color, trata-o como folha
            if (Objeto is Color)
                return;
            if (Objeto.GetType().IsGenericType && Objeto.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return;

            // Processa propriedades
            foreach (PropertyInfo prop in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShouldSkipMember(prop))
                    continue;

                object valor = null;
                if (prop.GetIndexParameters().Length == 0) // Garante que não é uma propriedade indexadora
                {
                    try { valor = prop.GetValue(Objeto); } catch { MessageBox.Show(prop.Name); }
                }

                TreeNode node = new TreeNode($"{FormatMemberName(prop.Name)}");
                node.Tag = new ConfigItem { Localizacao = parent.Text + "." + prop.Name, Objeto = valor, MemberInfo = prop };
                parent.Nodes.Add(node);

                // Se o valor for complexo, chama a recursão
                if (valor != null && !prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string) && !(valor is Color))
                    AdicionarMembrosRecursivo(valor, node);
            }

            // Processa campos
            foreach (FieldInfo field in tipo.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ShouldSkipMember(field))
                    continue;

                object valor = null;
                try { valor = field.GetValue(Objeto); } catch { }

                TreeNode node = new TreeNode($"{FormatMemberName(field.Name)}");
                node.Tag = new ConfigItem { Localizacao = parent.Text + "." + field.Name, Objeto = valor, MemberInfo = field };
                parent.Nodes.Add(node);

                if (valor != null && !field.FieldType.IsPrimitive && field.FieldType != typeof(string) && !(valor is Color))
                    AdicionarMembrosRecursivo(valor, node);
            }
        }

        // Formata o nome inserindo espaços antes das letras maiúsculas
        private string FormatMemberName(string name)
        {
            return name;
            return Regex.Replace(name, "(\\B[A-Z])", " $1");
        }

        // Formata o tipo para uma nomenclatura mais amigável (trata arrays)
        private string FormatTipo(string tipo)
        {
            // Se for array, processa o elemento e acrescenta "- Array"
            if (tipo.EndsWith("[]"))
            {
                string tipoBase = tipo.Substring(0, tipo.Length - 2);
                return $"{FormatTipo(tipoBase)} - Array";
            }

            // Remove prefixos indesejados (como "System." ou o namespace do projeto)
            if (tipo.StartsWith("System."))
                tipo = tipo.Substring("System.".Length);

            // Se o tipo contiver o namespace do projeto, remove-o (ajuste conforme necessário)
            if (tipo.StartsWith("PerguntasFrequentesSuporte."))
                tipo = tipo.Substring("PerguntasFrequentesSuporte.".Length);

            return tipo switch
            {
                "String" => "Texto",
                "Int32" => "Inteiro (32 bits)",
                "Int64" => "Inteiro (64 bits)",
                "Boolean" => "Verdadeiro ou Falso",
                "Color" => "Cor",
                "FontStyle" => "Estilo de Fonte",
                _ => tipo
            };
        }

        // Duplo clique na TreeView: edita o valor do membro selecionado
        private void TreeViewConfig_DoubleClick(object sender, EventArgs e)
        {
            // Esconde o menu (supondo que exista um formulário Menu)
            foreach (Form form in Application.OpenForms)
            {
                if (form is Menu)
                    form.Hide();
            }

            if (TreeViewConfig.SelectedNode == null)
                return;
            if (!(TreeViewConfig.SelectedNode.Tag is ConfigItem item))
                return;

            object ParenteDoObjeto = ObterObjetoPai(item.Localizacao);

            if (ParenteDoObjeto == null)
                MessageBox.Show($"Erro: Objeto pai não encontrado para '{item.Localizacao}'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Se for um caminho de imagem, abre o InputBoxImagem
            if (item.MemberInfo?.Name.Contains("Caminho") == true)
            {
                string novoCaminho = InputBoxImagem.Show(
                    $"Escolha a imagem para {FormatMemberName(TreeViewConfig.SelectedNode.Text)}:",
                    item.Objeto != null ? item.Objeto.ToString() : ""
                );

                if (!string.IsNullOrWhiteSpace(novoCaminho))
                {
                    // Define o diretório de destino
                    string diretorioDestino = Path.Combine(Application.StartupPath, "imagens");
                    if (!Directory.Exists(diretorioDestino))
                        Directory.CreateDirectory(diretorioDestino);

                    // Obtém o nome do arquivo e cria o novo caminho
                    string nomeArquivo = Path.GetFileName(novoCaminho);
                    string caminhoDestino = Path.Combine(diretorioDestino, nomeArquivo);

                    // Verifica se a imagem já existe
                    if (File.Exists(caminhoDestino))
                    {
                        DialogResult resultado = MessageBox.Show($"A imagem '{nomeArquivo}' já existe. Pretende substituí-la?", "Imagem existente", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (resultado == DialogResult.Yes)
                            File.Copy(novoCaminho, caminhoDestino, true);
                        else if (resultado == DialogResult.No)
                            return; // Mantém o valor antigo
                        else
                            return; // Cancela a edição
                    }
                    else
                        File.Copy(novoCaminho, caminhoDestino);


                    // Atualiza o valor no objeto real
                    if (item.MemberInfo is PropertyInfo prop)
                        prop.SetValue(ParenteDoObjeto, caminhoDestino);
                    else if (item.MemberInfo is FieldInfo field)
                        field.SetValue(ParenteDoObjeto, caminhoDestino);

                    item.Objeto = caminhoDestino;
                    
                    //TreeViewConfig.SelectedNode.Text = $"{FormatMemberName(item.Localizacao)} ({FormatTipo(item.Objeto.GetType().Name)}): {caminhoDestino}";
                }
            }
            // Se o valor for primitivo ou string
            else if (item.Objeto != null && (item.Objeto.GetType().IsPrimitive || item.Objeto is string))
            {
                string input = InputBoxTemp.Show($"Editar {FormatMemberName(item.Localizacao)}:", "Editar", item.Objeto?.ToString() ?? "");

                if (string.IsNullOrWhiteSpace(input))
                    input = null; // Permite definir valores nulos

                if (input != null)
                {
                    try
                    {
                        // Se a variável tem escolha pré-definida, chama o subprograma correspondente
                        if (escolhasPreDefinidas.ContainsKey(item.MemberInfo?.Name ?? ""))
                            input = ObterEscolhaPreDefinida(item.MemberInfo.Name, input);

                        object novoValor = Convert.ChangeType(input, item.Objeto?.GetType() ?? typeof(string));

                        if (ParenteDoObjeto == null)
                        {
                            MessageBox.Show($"Erro: Não foi possível encontrar o objeto pai para {item.Localizacao}");
                            return;
                        }

                        if (item.MemberInfo is PropertyInfo prop)
                            prop.SetValue(ParenteDoObjeto, novoValor);
                        else if (item.MemberInfo is FieldInfo field)
                            field.SetValue(ParenteDoObjeto, novoValor);


                        item.Objeto = novoValor;
                        TreeNode temp = TreeViewConfig.SelectedNode;
                        TreeViewConfig.SelectedNode = null;  // Remove temporariamente a seleção
                        TreeViewConfig.SelectedNode = temp;  // Restaura a seleção
                    }
                    catch (Exception ex)
                    {
                        LogUtility.EscreverNaLog("Erro ao converter o valor: " + ex.Message);
                    }
                }
            }
            // Se o valor for uma cor, chama o InputBoxColor
            else if (item.Objeto is Color)
            {
                Color novoValor = InputBoxColor.Show($"Escolha a nova cor de {FormatMemberName(TreeViewConfig.SelectedNode.Text)}:", (Color)item.Objeto);

                if (item.MemberInfo is PropertyInfo prop)
                    prop.SetValue(ParenteDoObjeto, novoValor);
                else if (item.MemberInfo is FieldInfo field)
                    field.SetValue(ParenteDoObjeto, novoValor);

                item.Objeto = novoValor;
                TreeNode temp = TreeViewConfig.SelectedNode;
                TreeViewConfig.SelectedNode = null;  // Remove temporariamente a seleção
                TreeViewConfig.SelectedNode = temp;  // Restaura a seleção
                //TreeViewConfig.SelectedNode.Text = $"{FormatMemberName(item.Localizacao)} ({FormatTipo(item.Objeto.GetType().Name)}): {novoValor}";
            }
        }
        private void TreeViewConfig_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBoxDetalhes.Items.Clear();
            if (e.Node.Tag is ConfigItem item)
            {
                listBoxDetalhes.Items.Add("Localização: " + item.Localizacao);

                string tipoFormatado;
                if (item.Objeto != null)
                {
                    // Se não for null, pega o tipo do objeto
                    if (item.Objeto is Array)
                    {
                        tipoFormatado = (item.Objeto as Array).Length > 0
                            ? $"{FormatTipo(item.Objeto.GetType().GetElementType().Name)} - Array"
                            : "Array";
                    }
                    else
                        tipoFormatado = FormatTipo(item.Objeto.GetType().Name);
                }
                else
                {
                    // Se for null, tenta obter o tipo pelo MemberInfo (propriedade ou campo)
                    if (item.MemberInfo is PropertyInfo prop)
                        tipoFormatado = FormatTipo(prop.PropertyType.Name);
                    else if (item.MemberInfo is FieldInfo field)
                        tipoFormatado = FormatTipo(field.FieldType.Name);
                    else
                        tipoFormatado = "null";
                }
                listBoxDetalhes.Items.Add("Tipo: " + tipoFormatado);
                listBoxDetalhes.Items.Add("Valor: " + (item.Objeto != null ? item.Objeto.ToString() : "null"));

                if (item.MemberInfo != null)
                {
                    var descAttr = item.MemberInfo.GetCustomAttribute<DescriptionAttribute>();
                    if (descAttr != null)
                        listBoxDetalhes.Items.Add("Descrição: " + descAttr.Description);
                }
            }
        }
        private string ObterEscolhaPreDefinida(string nome, string valorAtual)
        {
            if (escolhasPreDefinidas.ContainsKey(nome))
                return InputEscolha.Show($"Escolha um valor para {nome}:", "Escolha", escolhasPreDefinidas[nome], valorAtual);
            return valorAtual;
        }
        // Subprograma simplificado para obter o objeto pai com base na localização
        private object ObterObjetoPai(string localizacao)
        {
            object objetoAtual = ConfiguracoesGlobais;

            // Divide a localização pelos pontos
            string[] partes = localizacao.Split('.');

            for (int i = 0; i < partes.Length - 1; i++) // Última parte é a propriedade final
            {
                string parteAtual = partes[i];

                // Verifica se há um índice de array (ex.: "Botoes[0]")
                Match matchArray = Regex.Match(parteAtual, @"(.+?)\[(\d+)\]$");
                if (matchArray.Success)
                {
                    string nomeLista = matchArray.Groups[1].Value;
                    int indice = int.Parse(matchArray.Groups[2].Value);

                    // Obtém a propriedade que é uma lista
                    PropertyInfo prop = objetoAtual.GetType().GetProperty(nomeLista);
                    if (prop != null)
                    {
                        var lista = prop.GetValue(objetoAtual) as IList<object>;
                        if (lista != null && indice < lista.Count)
                        {
                            objetoAtual = lista[indice]; // Vai para o elemento correto da lista
                        }
                        else
                        {
                            return null; // Índice fora dos limites ou lista nula
                        }
                    }
                }
                // Verifica se é um dicionário (ex.: "TextoBotaoMostrarEsconder[BotoesVisiveis]")
                else if (parteAtual.Contains("[") && parteAtual.Contains("]"))
                {
                    Match matchDicionario = Regex.Match(parteAtual, @"(.+?)\[(.+?)\]$");
                    if (matchDicionario.Success)
                    {
                        string nomeDicionario = matchDicionario.Groups[1].Value;
                        string chave = matchDicionario.Groups[2].Value.Replace("\"", ""); // Remove aspas se houver

                        PropertyInfo prop = objetoAtual.GetType().GetProperty(nomeDicionario);
                        if (prop != null)
                        {
                            var dicionario = prop.GetValue(objetoAtual) as IDictionary<string, object>;
                            if (dicionario != null && dicionario.ContainsKey(chave))
                                objetoAtual = dicionario[chave]; // Vai para o valor correto do dicionário
                            else
                                return null; // Chave não encontrada
                        }
                    }
                }
                else
                {
                    // Se não for array nem dicionário, é uma propriedade normal
                    PropertyInfo prop = objetoAtual.GetType().GetProperty(parteAtual);
                    if (prop != null)
                        objetoAtual = prop.GetValue(objetoAtual);
                    else
                        return null; // Propriedade não encontrada
                }
            }

            return objetoAtual;
        }

        // Ao clicar em "Salvar Alterações", aplica as cópias temporárias aos dados originais
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            Ficheiros.IdPerfilAtual++;
            // Environment.SetEnvironmentVariable("CaminhoSalvamento", Caminho, EnvironmentVariableTarget.User);
            Ficheiros.SalvarConfig(Ficheiros.IdPerfilAtual, ConfiguracoesGlobais);
            Environment.SetEnvironmentVariable("IdPerfilAtual", Ficheiros.IdPerfilAtual.ToString(), EnvironmentVariableTarget.User);
            Application.Restart();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        public static Dictionary<string, TreeNode> TreeNodeClasses = new Dictionary<string, TreeNode>();
        // Classe para armazenar informações de cada nó
        public class ConfigItem
        {
            public string Descricao { get; set; }
            public object Objeto { get; set; }
        }
        public static class InputEscolha
        {
            public static string Show(string prompt, string title, string[] opcoes, string valorAtual)
            {
                Form form = new Form();
                Label label = new Label();
                ComboBox comboBox = new ComboBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;
                comboBox.Items.AddRange(opcoes);
                // Se o valorAtual estiver entre as opções, seleciona-o; caso contrário, seleciona o primeiro
                comboBox.SelectedItem = Array.Exists(opcoes, o => o.Equals(valorAtual)) ? valorAtual : opcoes[0];
                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancelar";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                label.SetBounds(9, 20, 372, 13);
                comboBox.SetBounds(12, 36, 372, 20);
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                label.AutoSize = true;
                form.ClientSize = new Size(396, 107);
                form.Controls.AddRange(new Control[] { label, comboBox, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult result = form.ShowDialog();
                return result == DialogResult.OK ? comboBox.SelectedItem.ToString() : valorAtual;
            }
        }

        // Implementação simples de InputBoxTemp (para texto)
        public static class InputBoxTemp
        {
            public static string Show(string prompt, string title, string defaultValue = "")
            {
                // Esconde o menu
                foreach (Form form2 in Application.OpenForms)
                {
                    if (form2 is Menu)
                        form2.Hide();
                    if (form2.Name.StartsWith("Input"))
                    {
                        form2.Close();
                        form2.Dispose();
                    }
                }


                Form form = new Form();
                Label label = new Label();
                TextBox textBoxErro = new TextBox(); //System.ArgumentException: 'Parameter is not valid.'
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;
                textBoxErro.Text = defaultValue;
                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancelar";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                label.SetBounds(9, 20, 372, 13);
                textBoxErro.SetBounds(12, 36, 372, 20);
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                label.AutoSize = true;
                form.ClientSize = new Size(396, 107);
                form.Controls.AddRange(new Control[] { label, textBoxErro, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult dialogResult = form.ShowDialog();

                // Mostra novamente o menu
                foreach (Form form3 in Application.OpenForms)
                {
                    if (form3 is Menu)
                        form3.Show();
                }

                return dialogResult == DialogResult.OK ? textBoxErro.Text : "";
            }
        }

        // Implementação de InputBoxColor para selecionar uma cor

        private void comboBox1_Click(object sender, EventArgs    e)
        {
            BtnSalvar_Click(sender, e);
        }

        */
// Subprogramas de edição especial para certos tipos de configuração

/* Tentativa de Listar Sub-classes
private void Configuracoes_Load(object sender, EventArgs e)
{
    // Carrega a configuração a partir de um ficheiro ou cria um novo objeto padrão
    appConfigInstance = AcederConfig.ConfigAtual.AppConfig;

    // Popula a TreeView com a configuração
    PopularTreeView(appConfigInstance, TreeViewConfig);

    // Se tens um ComboBox para selecionar temas, por exemplo:
    comboBox1.Items.Clear();
    foreach (var tema in appConfigInstance.VisualAplicacao.Tema)
    {
        comboBox1.Items.Add(tema.NomeTema);
    }
    // Seleciona o tema atual no ComboBox (caso haja)
    if (appConfigInstance.IndiceTemaAtual < comboBox1.Items.Count)
        comboBox1.SelectedIndex = appConfigInstance.IndiceTemaAtual;
}
// Evento disparado quando um nó é selecionado na TreeView
private void TreeViewConfig_AfterSelect(object sender, TreeViewEventArgs e)
{
    listBoxDetalhes.Items.Clear();

    if (e.Node.Tag is NodeMetadata metadata)
    {
        listBoxDetalhes.Items.Add("Nome: " + metadata.NomeOriginal);

        if (metadata.Valor != null)
            listBoxDetalhes.Items.Add("Valor: " + metadata.Valor.ToString());
        else
            listBoxDetalhes.Items.Add("Valor: null");

        if (metadata.Propriedade != null)
        {
            listBoxDetalhes.Items.Add("Tipo: " + metadata.Propriedade.PropertyType.Name);

            var descAttr = metadata.Propriedade.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr != null)
                listBoxDetalhes.Items.Add("Descrição: " + descAttr.Description);
        }

        // Se for uma subclasse, mostrar as suas propriedades como detalhes
        if (metadata.Valor != null)
        {
            Type valorType = metadata.Valor.GetType();
            if (!valorType.IsPrimitive && !(metadata.Valor is string))
            {
                PropertyInfo[] propriedades = valorType.GetProperties();
                foreach (var prop in propriedades)
                {
                    object valorSub = prop.GetValue(metadata.Valor);
                    if (valorSub != null)
                        listBoxDetalhes.Items.Add(prop.Name + ": " + valorSub.ToString());
                    else
                        listBoxDetalhes.Items.Add(prop.Name + ": null");
                }
            }
        }
    }
}
public void PopularTreeView(AppConfig config, TreeView treeView)
{
    treeView.Nodes.Clear();
    TreeNode rootNode = new TreeNode("Configuração da Aplicação");

    // Começa o processo recursivo a partir do objeto raiz
    AdicionarPropriedadesPermitidas(config, rootNode);

    treeView.Nodes.Add(rootNode);
    treeView.CollapseAll();
    CopiarNomesDosNos(treeView);
}

private void AdicionarPropriedadesPermitidas(object objeto, TreeNode nodePai)
{
    if (objeto == null)
        return;

    foreach (PropertyInfo prop in objeto.GetType().GetProperties())
    {
        object valor = prop.GetValue(objeto);

        // Ignora propriedades não permitidas
        if (!PropriedadePermitida(valor))
            continue;

        string nomeFormatado = FormatarNome(prop.Name);
        TreeNode nodeFilho = new TreeNode(nomeFormatado)
        {
            Tag = new NodeMetadata
            {
                Valor = valor,
                Propriedade = prop,
                Descricao = ObterDescricao(prop),
                NomeOriginal = prop.Name,
                Pai = objeto
            }
        };

        // Se for uma coleção (lista ou dicionário)
        if (valor is IDictionary dict)
        {
            foreach (DictionaryEntry entry in dict)
            {
                TreeNode filhoDicionario = new TreeNode($"{entry.Key}");
                filhoDicionario.Tag = new NodeMetadata
                {
                    Valor = entry.Value,
                    NomeOriginal = entry.Key.ToString(),
                    Pai = valor
                };

                // Adiciona subpropriedades se forem permitidas
                if (PropriedadePermitida(entry.Value))
                {
                    AdicionarPropriedadesPermitidas(entry.Value, filhoDicionario);
                }

                nodeFilho.Nodes.Add(filhoDicionario);
            }
        }
        else if (valor is IList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TreeNode filhoLista = new TreeNode($"Item {i}");
                object item = list[i];

                filhoLista.Tag = new NodeMetadata
                {
                    Valor = item,
                    NomeOriginal = $"Item {i}",
                    Pai = valor
                };

                // Adiciona subpropriedades se forem permitidas
                if (PropriedadePermitida(item))
                {
                    AdicionarPropriedadesPermitidas(item, filhoLista);
                }

                nodeFilho.Nodes.Add(filhoLista);
            }
        }
        else if (PropriedadePermitida(valor)) // Verifica se o objeto é uma subclasse permitida
        {
            AdicionarPropriedadesPermitidas(valor, nodeFilho);
        }

        nodePai.Nodes.Add(nodeFilho);
    }
}


//CopiarNomesDosNos(treeView);
private void CopiarNomesDosNos(TreeView treeView)
{
    List<string> nomesDosNos = new List<string>();

    foreach (TreeNode node in treeView.Nodes)
    {
        ListarNomesRecursivamente(node, nomesDosNos);
    }

    // Concatena todos os nomes separados por nova linha
    string nomesConcatenados = string.Join(Environment.NewLine, nomesDosNos);

    // Copia para a área de transferência
    Clipboard.SetText(nomesConcatenados);

    // Exibe uma mensagem de confirmação
    MessageBox.Show("Os nomes dos nós foram copiados para a área de transferência.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
}



private bool PropriedadePermitida(object valor)
{
    if (valor == null)
        return false;

    Type tipo = valor.GetType();

    if (valor is IList || valor is IDictionary) // Permite listas e dicionários
        return true;

    List<Type> tiposPermitidos = new List<Type>
{
typeof(AppConfig),
typeof(ConfigGeral),
typeof(TamanhoInicialJanelas),
typeof(TemaVisual),
typeof(VisualBotoesPorJanela),
typeof(TemaMenu),
typeof(AparenciaGeral),
typeof(FicheiroImagem),
typeof(ConfigPassoAPasso),
typeof(ConfigSair_Mostrar),
typeof(FuncaoBotaoMenu)
};

    return tiposPermitidos.Contains(tipo);
}
private bool PropriedadeInternaOuDesnecessaria(PropertyInfo prop)
{
    // Ignorar propriedades internas como SyncRoot, Count, Capacity, etc.
    List<string> propriedadesIgnoradas = new List<string>
    {
        "SyncRoot", "Count", "Capacity", "IsFixedSize", "IsReadOnly", "IsSynchronized"
    };

    return propriedadesIgnoradas.Contains(prop.Name);
}


*/
/*
public static class EdicaoEspecial
{
    public static BtnMudarEsconder_MostrarMenu EdicaoEspecialParaConfig_BtnMudarEsconder_MostrarMenu(BtnMudarEsconder_MostrarMenu atual)
    {
        MessageBox.Show("Edição especial para 'Config_BtnMudarEsconder_MostrarMenu' não implementada.",
            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return atual;
    }

    public static JanelaMenu EdicaoEspecialParaConfig_Menu(JanelaMenu atual)
    {
        MessageBox.Show("Edição especial para 'Config_Menu' não implementada.",
            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return atual;
    }

    public static JanelaPassoAPasso EdicaoEspecialParaConfig_PassoAPasso(JanelaPassoAPasso atual)
    {
        MessageBox.Show("Edição especial para 'Config_PassoAPasso' não implementada.",
            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return atual;
    }
}
        */