using BibliotecaAuxiliarForms.Ficheiros;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace PerguntasFrequentesSuporte
{
    public partial class Configuracoes : Form
    // Erro que não dá para editar listas ou arrays porque não encontra o pai,
    // quando se acede ao valor de alguma sub arvore o caminho fica apenas como subArvore.Variável e não como Arvore.SubArvore.Variável
    // falta fazer apresentar dicionários, onde cada chave aparece como um subvalor e o valor que se edita é o da valor associado á chave, e nunca se edita a chave em si
    {
        private List<AppConfig> PerfisDisponiveis = new();

        private AppConfig PerfilAtual
        {
            get
            {
                int indice = comboBoxPerfis.SelectedIndex;
                if (indice >= 0 && indice < PerfisDisponiveis.Count)
                    return PerfisDisponiveis[indice];

                return null;
            }
        }
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
        private void TreeViewConfig_DoubleClick(object sender, EventArgs Event)
        {
            TreeNodeMouseClickEventArgs e = new(TreeViewConfig.SelectedNode, MouseButtons.Left, 2, 0, 0);
            if (e.Node != null && e.Node.Tag is NodeMetadata metadata)
            {
                object novoValor = AbrirInputBoxPorTipo(metadata); // Usa o InputBox adequado com base no tipo

                if (novoValor != null && !novoValor.Equals(metadata.Valor))
                {
                    // Atualizar o objeto na configuração
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

                    metadata.Valor = novoValor;  // Atualiza apenas a tag do nó com o novo valor (já convertido)
                    e.Node.Tag = metadata;

                    // NÃO altera o texto do nó: mantém apenas o nome de exibição
                    // Caso pretendas atualizar os detalhes (por exemplo, numa ListBox), podes usar:
                    listBoxDetalhes.Items.Clear();
                    TreeViewConfig.SelectedNode = e.Node;
                    TreeViewConfig_AfterSelect(sender, new TreeViewEventArgs(e.Node));
                }
            }
        }
        private object AbrirInputBoxPorTipo(NodeMetadata metadata)
        {
            Type tipo = metadata.Valor.GetType();

            switch (Type.GetTypeCode(tipo))
            {
                case TypeCode.Int32: return AbrirInputBoxInt32((int)metadata.Valor);
                case TypeCode.Int64: return AbrirInputBoxInt64((long)metadata.Valor);
                case TypeCode.Double: return AbrirInputBoxDouble((double)metadata.Valor);
                case TypeCode.String: return AbrirInputBoxString(metadata.Valor.ToString());
                case TypeCode.Boolean: return AbrirInputBoxBoolean();
                case TypeCode.Object:
                    if (metadata.Valor is Color cor) return BibliotecaAuxiliarForms.Input.InputBox.Color(new BibliotecaAuxiliarForms.Input.InputBoxOpcoes.Color("Escolha um cor:", $"Introduza o novo valor para a cor {metadata.NomeOriginal}", cor));
                    if (metadata.Valor is Font fonte) return BibliotecaAuxiliarForms.Input.InputBox.Font(new BibliotecaAuxiliarForms.Input.InputBoxOpcoes.Font("Escolha um Fonte:", $"Introduza o novo valor para a fonte {metadata.NomeOriginal}", fonte));
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
        private bool AbrirInputBoxBoolean()
        {
            DialogResult resultado = MessageBox.Show("Ativar valor?", "Editar Booleano", MessageBoxButtons.YesNo);
            return resultado == DialogResult.Yes;
        }
        private void Configuracoes_Load(object sender, EventArgs e)
        {
            CarregarPerfisNaComboBox();
        }
        private void CarregarPerfisNaComboBox()
        {
            PerfisDisponiveis = Ficheiros.ListarPerfisGuardados();

            comboBoxPerfis.Items.Clear();

            for (int i = 0; i < PerfisDisponiveis.Count; i++)
            {
                AppConfig perfil = PerfisDisponiveis[i];
                if (perfil != null)
                    comboBoxPerfis.Items.Add(perfil.Perfil);
            }

            // Seleciona o último usado com base na variável de ambiente
            if (Ficheiros.IdPerfilAtual >= 0 && Ficheiros.IdPerfilAtual < comboBoxPerfis.Items.Count)
                comboBoxPerfis.SelectedIndex = Ficheiros.IdPerfilAtual;
            else if (comboBoxPerfis.Items.Count > 0)
                comboBoxPerfis.SelectedIndex = 0;
        }
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
            }
        }
        public TreeNode CriarTreeView(AppConfig config)
        {
            // Cria o nó raiz representando a configuração principal
            TreeNode raiz = new("Configuração da Aplicação");

            // Percorre as propriedades públicas de AppConfig e cria nós recursivamente
            foreach (PropertyInfo prop in config.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (DeveIgnorarPropriedade(prop.Name))
                    continue;

                object valor = prop.GetValue(config);
                TreeNode noPropriedade = CriarTreeNodeRecursivo(valor, prop.Name, config, prop);
                raiz.Nodes.Add(noPropriedade);
            }

            return raiz;
        }
        private TreeNode CriarTreeNodeRecursivo(object objeto, string nome, object pai, PropertyInfo propPai = null)
        {
            // Formata o nome (ex.: "NomeVariavel" → "Nome variavel")
            string texto = FormatarNome(nome);

            // NÃO inclui o valor no texto do nó; o nome é apenas visual
            // if (objeto == null || objeto.GetType().IsPrimitive || objeto is string)
            //     texto += ": " + (objeto != null ? objeto.ToString() : "null");

            // Cria o NodeMetadata para este nó
            NodeMetadata metadata = new()
            {
                NomeOriginal = nome,
                Valor = objeto,
                Propriedade = propPai,
                Descricao = ObterDescricao(propPai),
                Pai = pai
            };

            TreeNode no = new(texto);
            no.Tag = metadata;

            // Se o objeto for uma coleção (IList)
            if (objeto is IList lista)
            {
                for (int i = 0; i < lista.Count; i++)
                    no.Nodes.Add(CriarTreeNodeRecursivo(lista[i], $"{i}", objeto));
            }
            // Se for um dicionário (IDictionary)
            else if (objeto is IDictionary dict)
            {
                foreach (DictionaryEntry entry in dict)
                    no.Nodes.Add(CriarTreeNodeRecursivo(entry.Value, entry.Key.ToString(), objeto));
            }
            // Se for um objeto complexo (não primitivo nem string) e for permitido aprofundar
            else if (objeto != null && !objeto.GetType().IsPrimitive && !(objeto is string) && PropriedadePermitida(objeto))
            {
                foreach (PropertyInfo prop in objeto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    // Ignora propriedades desnecessárias
                    if (DeveIgnorarPropriedade(prop.Name))
                        continue;
                    // Ignora se tiver [Browsable(false)]
                    if (prop.GetCustomAttribute(typeof(BrowsableAttribute)) is BrowsableAttribute browsable && !browsable.Browsable)
                        continue;

                    no.Nodes.Add(CriarTreeNodeRecursivo(prop.GetValue(objeto), prop.Name, objeto, prop));
                }
            }

            return no;
        }
        private bool PropriedadePermitida(object valor) //não funciona 100% bem
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
        private bool DeveIgnorarPropriedade(string nome)
        {
            List<string> propriedadesIgnoradas = new() // Lista de nomes (em minúsculas) das propriedades a serem ignoradas
            {
                "syncroot",
                "longlength",
                "length",
                "rank",
                "isreadonly",
                "isfixedsize",
                "issynchronized",
                "value__",
                "Count",
                "Item",
                "IndiceTemaAtual",
                "Perfil",
                "Capacity"
            };

            return propriedadesIgnoradas.Contains(nome.ToLowerInvariant());  // Verifica se o nome na lista de propriedades ignoradasl, returna true se estiver na lista
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
            if (prop == null)
                return "";

            object atributo = prop.GetCustomAttribute(typeof(DescriptionAttribute));

            if (atributo == null)
                return "";

            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)atributo;

            if (descriptionAttribute.Description == null)
                return "";

            return descriptionAttribute.Description;
        }
        private void AtualizarConfiguracao(TreeNode no)
        {
            if (no.Tag is NodeMetadata metadata)
            {
                // Se houver uma referência a uma propriedade, atualiza o objeto pai com o novo valor
                if (metadata.Propriedade != null)
                {
                    metadata.Propriedade.SetValue(metadata.Pai, metadata.Valor);
                }
                // Se o pai for um dicionário, atualiza o valor da chave correspondente
                else if (metadata.Pai is IDictionary dictPai)
                {
                    dictPai[metadata.NomeOriginal] = metadata.Valor;
                }
                // Se o pai for uma lista, atualiza o elemento pelo índice extraído do nome
                else if (metadata.Pai is IList listPai)
                {
                    int index = int.Parse(metadata.NomeOriginal.Replace("Item ", ""));
                    listPai[index] = metadata.Valor;
                }
            }
            // Percorre recursivamente os subnós
            foreach (TreeNode filho in no.Nodes)
            {
                AtualizarConfiguracao(filho);
            }
        }
        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            Visible = false;

            foreach (TreeNode no in TreeViewConfig.Nodes)
                AtualizarConfiguracao(no);

            foreach (TreeNode no in TreeViewConfig.Nodes)  // Percorre todos os nós da TreeView e atualiza a configuração com os valores das tags
            {
                AtualizarConfiguracao(no);
            }

            Ficheiros.SalvarConfig(comboBoxPerfis.SelectedIndex, PerfilAtual);
            Environment.SetEnvironmentVariable("IdPerfilAtual", comboBoxPerfis.SelectedIndex.ToString(), EnvironmentVariableTarget.User);
            Application.Restart();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (VerificarAlteracoesPendentes())
            {
                var resultado = MessageBox.Show(
                    "Existem alterações por guardar.\n\nDeseja guardar antes de sair?",
                    "Alterações pendentes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Cancel)
                    return;

                if (resultado == DialogResult.Yes)
                {
                    foreach (TreeNode no in TreeViewConfig.Nodes)
                        AtualizarConfiguracao(no);

                    int indice = comboBoxPerfis.SelectedIndex;

                    if (indice >= 0 && indice < PerfisDisponiveis.Count)
                        Ficheiros.SalvarConfig(indice, PerfilAtual);
                }

                if (resultado == DialogResult.No)
                {
                    int indice = comboBoxPerfis.SelectedIndex;

                    if (indice >= 0)
                    {
                        AppConfig original;

                        if (Ficheiros.CarregarConfig(indice, out original))
                            PerfisDisponiveis[indice] = original;
                    }
                }
            }

            Hide();
        }
        private void Configuracoes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;  // Impede o fecho
                Hide();                 // Apenas esconde
            }
        }
        private void comboBoxPerfis_SelectedIndexChanged(object sender, EventArgs e)
        {
            int novoIndice = comboBoxPerfis.SelectedIndex;

            if (novoIndice < 0 || novoIndice >= PerfisDisponiveis.Count)
                return;

            if (VerificarAlteracoesPendentes())
            {
                var resultado = MessageBox.Show(
                    "Existem alterações por guardar neste perfil.\n\nDeseja guardar antes de mudar?",
                    "Alterações pendentes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (resultado == DialogResult.Cancel)
                {
                    comboBoxPerfis.SelectedIndexChanged -= comboBoxPerfis_SelectedIndexChanged;
                    comboBoxPerfis.SelectedIndex = PerfisDisponiveis.FindIndex(p => p.Perfil == PerfilAtual.Perfil);
                    comboBoxPerfis.SelectedIndexChanged += comboBoxPerfis_SelectedIndexChanged;
                    return;
                }

                if (resultado == DialogResult.Yes)
                {
                    foreach (TreeNode no in TreeViewConfig.Nodes)
                        AtualizarConfiguracao(no);

                    int atual = PerfisDisponiveis.FindIndex(p => p.Perfil == PerfilAtual.Perfil);

                    if (atual >= 0)
                        Ficheiros.SalvarConfig(atual, PerfilAtual);
                }

                if (resultado == DialogResult.No)
                {
                    int anterior = PerfisDisponiveis.FindIndex(p => p.Perfil == PerfilAtual.Perfil);
                    if (anterior >= 0)
                    {
                        AppConfig recarregado;
                        if (Ficheiros.CarregarConfig(anterior, out recarregado))
                            PerfisDisponiveis[anterior] = recarregado;
                    }
                }
            }

            AtualizarInterfaceDoPerfilAtual();
        }
        private void AtualizarInterfaceDoPerfilAtual()
        {
            if (PerfilAtual == null)
                return;

            // Atualiza a TreeView com o novo perfil
            TreeViewConfig.Nodes.Clear();
            TreeViewConfig.Nodes.Add(CriarTreeView(PerfilAtual));

            // Atualiza a lista de temas
            comboBoxTema.SelectedIndexChanged -= comboBoxTema_SelectedIndexChanged; // desliga o evento
            comboBoxTema.Items.Clear();

            if (PerfilAtual.VisualAplicacao != null && PerfilAtual.VisualAplicacao.Tema != null)
            {
                foreach (TemaVisual tema in PerfilAtual.VisualAplicacao.Tema)
                {
                    if (tema != null)
                        comboBoxTema.Items.Add(tema.NomeTema);
                }

                int indice = PerfilAtual.IndiceTemaAtual;

                if (indice >= 0 && indice < comboBoxTema.Items.Count)
                    comboBoxTema.SelectedIndex = indice;
                else
                    comboBoxTema.SelectedIndex = 0;
            }

            comboBoxTema.SelectedIndexChanged += comboBoxTema_SelectedIndexChanged; // volta a ligar o evento
        }
        private void comboBoxTema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PerfilAtual == null)
                return;

            PerfilAtual.IndiceTemaAtual = comboBoxTema.SelectedIndex; ;
            AtualizarInterfaceDoPerfilAtual();
        }
        private void btnRenomearPerfil_Click(object sender, EventArgs e)
        {
            if (PerfilAtual == null)
                return;

            string nomeAntigo = PerfilAtual.Perfil;
            string novoNome = AbrirInputBoxString($"Novo nome para o perfil '{nomeAntigo}':");

            if (string.IsNullOrWhiteSpace(novoNome) || novoNome == nomeAntigo)
                return;

            foreach (var perfil in PerfisDisponiveis)
            {
                if (perfil != PerfilAtual && perfil.Perfil.Equals(novoNome, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Já existe um perfil com esse nome.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            PerfilAtual.Perfil = novoNome;
            comboBoxPerfis.Items[comboBoxPerfis.SelectedIndex] = novoNome;
            Ficheiros.SalvarConfig(comboBoxPerfis.SelectedIndex, PerfilAtual);
        }
        private void btnNovoPerfil_Click(object sender, EventArgs e)
        {
            string nome = AbrirInputBoxString("Nome para o novo perfil: - Novo Perfil");

            if (string.IsNullOrWhiteSpace(nome))
                return;

            foreach (var p in PerfisDisponiveis)
            {
                if (p.Perfil.ToLower() == nome.ToLower())
                {
                    MessageBox.Show("Já existe um perfil com esse nome.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            AppConfig novo = new AppConfig(PerfilAtual.VisualAplicacao, PerfilAtual.ConfiguracaoAplicacao, nome, PerfilAtual.IndiceTemaAtual);

            int novoId = PerfisDisponiveis.Count;

            Ficheiros.SalvarConfig(novoId, novo);
            Ficheiros.CorrigirIdDeFicheiros();

            CarregarPerfisNaComboBox();
            comboBoxPerfis.SelectedIndex = novoId;
        }
        private void btnApagarPerfil_Click(object sender, EventArgs e)
        {
            int indice = comboBoxPerfis.SelectedIndex;

            if (indice < 0 || PerfilAtual == null)
                return;

            if (PerfilAtual.Perfil == "Padrao")
            {
                MessageBox.Show("Não é possível apagar o perfil padrão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Tem a certeza que pretende apagar este perfil?", "Apagar Perfil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            string caminho = Path.Combine(Ficheiros.Caminho, "Perfil_" + indice + ".json");

            if (File.Exists(caminho))
            {
                try
                {
                    File.Delete(caminho);
                }
                catch
                {
                    MessageBox.Show("Erro ao apagar o ficheiro.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Ficheiros.CorrigirIdDeFicheiros();
            CarregarPerfisNaComboBox();
        }
        private bool VerificarAlteracoesPendentes()
        {
            int indice = comboBoxPerfis.SelectedIndex;

            if (indice < 0)
                return false;

            if (!Ficheiros.CarregarConfig(indice, out AppConfig original))
                return false;

            string originalJson = JsonSerializer.Serialize(original, new FicheirosJsonConversores().ObterConfigs());
            string atualJson = JsonSerializer.Serialize(PerfilAtual, new FicheirosJsonConversores().ObterConfigs());

            return originalJson != atualJson;
        }
        private void btnResetarAlteracoes_Click(object sender, EventArgs e)
        {
            int indice = comboBoxPerfis.SelectedIndex;

            if (indice < 0)
                return;

            AppConfig recarregado;

            if (Ficheiros.CarregarConfig(indice, out recarregado))
            {
                PerfisDisponiveis[indice] = recarregado;

                TreeViewConfig.Nodes.Clear();
                TreeViewConfig.Nodes.Add(CriarTreeView(recarregado));

                comboBoxTema.Items.Clear();

                foreach (var tema in recarregado.VisualAplicacao.Tema)
                    comboBoxTema.Items.Add(tema.NomeTema);

                if (recarregado.IndiceTemaAtual >= 0 && recarregado.IndiceTemaAtual < comboBoxTema.Items.Count)
                    comboBoxTema.SelectedIndex = recarregado.IndiceTemaAtual;
            }
        }
        private void btnCarregarPadrao_Click(object sender, EventArgs e)
        {
            if (PerfilAtual == null)
                return;

            if (!Ficheiros.CarregarConfig(0, out AppConfig padrao))
                padrao = Ficheiros.IniciarConfiguracoes();

            // Mantém o nome e índice atuais
            padrao.Perfil = PerfilAtual.Perfil;

            // Substitui na lista principal
            PerfisDisponiveis[comboBoxPerfis.SelectedIndex] = padrao;

            // Atualiza UI
            TreeViewConfig.Nodes.Clear();
            TreeViewConfig.Nodes.Add(CriarTreeView(padrao));
            AtualizarComboBoxTemas(padrao);
        }
        private void AtualizarComboBoxTemas(AppConfig config)
        {
            comboBoxTema.Items.Clear();

            if (config.VisualAplicacao?.Tema == null)
                return;

            foreach (var tema in config.VisualAplicacao.Tema)
            {
                if (tema != null)
                    comboBoxTema.Items.Add(tema.NomeTema);
            }
            comboBoxTema.SelectedIndex = 0;

            if (config.IndiceTemaAtual >= 0 && config.IndiceTemaAtual < comboBoxTema.Items.Count)
                comboBoxTema.SelectedIndex = config.IndiceTemaAtual;
        }
    }
}
