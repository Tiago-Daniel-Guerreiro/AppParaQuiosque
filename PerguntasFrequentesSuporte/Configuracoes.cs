using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
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
                case TypeCode.Int32:   return AbrirInputBoxInt32((int)metadata.Valor);
                case TypeCode.Int64:   return AbrirInputBoxInt64((long)metadata.Valor);
                case TypeCode.Double:  return AbrirInputBoxDouble((double)metadata.Valor);
                case TypeCode.String:  return AbrirInputBoxString(metadata.Valor.ToString());
                case TypeCode.Boolean: return AbrirInputBoxBoolean();
                case TypeCode.Object:
                    if (metadata.Valor is Color cor) return BibliotecaAuxiliarForms.Input.InputBox.Color(new BibliotecaAuxiliarForms.Input.InputBoxOpcoes.Color("Escolha um cor:",$"Introduza o novo valor para a cor {metadata.NomeOriginal}",cor));
                        if (metadata.Valor is Font fonte) return BibliotecaAuxiliarForms.Input.InputBox.Font(new BibliotecaAuxiliarForms.Input.InputBoxOpcoes.Font("Escolha um Fonte:", $"Introduza o novo valor para a fonte {metadata.NomeOriginal}",fonte));
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

        // Supondo que tens uma variável global para a configuração:
        private AppConfig appConfigInstance;
        private void Configuracoes_Load(object sender, EventArgs e)  // Evento de carregamento do formulário
        {
            // Carrega a configuração a partir de um ficheiro ou cria um novo objeto padrão
            appConfigInstance = AcederConfig.ConfigAtual;

            // Prenche a TreeView com a configuração
            TreeViewConfig.Nodes.Clear();
            TreeViewConfig.Nodes.Add(CriarTreeView(appConfigInstance));
            comboBox1.Items.Clear(); // Se tens um ComboBox para selecionar temas, por exemplo:
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
            }
        }
        // Método auxiliar para criar subnós de um objeto complexo

        // Método que cria a TreeView (nó raiz) a partir de um objeto AppConfig
        public TreeNode CriarTreeView(AppConfig config)
        {
            // Cria o nó raiz representando a configuração principal
            TreeNode raiz = new("Configuração da Aplicação");

            // Percorre as propriedades públicas de AppConfig e cria nós recursivamente
            foreach (PropertyInfo prop in config.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object valor = prop.GetValue(config);
                // Cria o nó para esta propriedade (usando o método recursivo)
                TreeNode noPropriedade = CriarTreeNodeRecursivo(valor, prop.Name, config, prop);
                raiz.Nodes.Add(noPropriedade);
            }

            return raiz;
        }

        // Método recursivo para criar um TreeNode para um objeto e suas propriedades
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
                Descricao = propPai != null ? ObterDescricao(propPai) : "",
                Pai = pai
            };

            TreeNode no = new TreeNode(texto);
            no.Tag = metadata;

            // Se o objeto for uma coleção (IList)
            if (objeto is IList lista)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    object elem = lista[i];
                    TreeNode noElem = CriarTreeNodeRecursivo(elem, $"{i}", objeto);
                    no.Nodes.Add(noElem);
                }
            }
            // Se for um dicionário (IDictionary)
            else if (objeto is IDictionary dict)
            {
                foreach (DictionaryEntry entry in dict)
                {
                    TreeNode noEntry = CriarTreeNodeRecursivo(entry.Value, entry.Key.ToString(), objeto);
                    no.Nodes.Add(noEntry);
                }
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

                    object valorProp = prop.GetValue(objeto);
                    TreeNode noSub = CriarTreeNodeRecursivo(valorProp, prop.Name, objeto, prop);
                    no.Nodes.Add(noSub);
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
            // Tenta obter o atributo Description, se existir
            var descAttr = prop.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descAttr != null ? descAttr.Description : "";
        }
        // Atualiza a instância de AppConfig (ou objetos aninhados) a partir do valor guardado na tag de cada TreeNode
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
            Hide();
           
            foreach (TreeNode no in TreeViewConfig.Nodes)  // Percorre todos os nós da TreeView e atualiza a configuração com os valores das tags
            {
                AtualizarConfiguracao(no);
            }

            int NovoId = Ficheiros.IdPerfilAtual + 1;
            Ficheiros.SalvarConfig(NovoId, appConfigInstance);
            Environment.SetEnvironmentVariable("IdPerfilAtual", NovoId.ToString(), EnvironmentVariableTarget.User) ;
            Application.Restart();
        }

        // Evento do botão Close para fechar a janela
        private void btnClose_Click(object sender, EventArgs e)
        {
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
    }
}
