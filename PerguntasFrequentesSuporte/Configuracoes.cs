using BibliotecaAuxiliarForms.Ficheiros;
using BibliotecaAuxiliarForms.Utilidades;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using static BibliotecaAuxiliarForms.Input.InputBoxOpcoes;

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

                if (novoValor != null)
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
                        int index;
                        if (int.TryParse(metadata.NomeOriginal.Replace("Item ", ""), out index) || int.TryParse(metadata.NomeOriginal, out index))
                        {
                            if (index >= 0 && index < listPai.Count)
                                listPai[index] = novoValor;
                        }
                    }

                    metadata.Valor = novoValor;
                    e.Node.Tag = metadata;

                    // Atualiza os subnós com base no novo conteúdo
                    AtualizarSubNiveisDoNo(e.Node);

                    // Atualiza a interface lateral
                    listBoxDetalhes.Items.Clear();
                    TreeViewConfig.SelectedNode = e.Node;
                    TreeViewConfig_AfterSelect(sender, new TreeViewEventArgs(e.Node));
                }
            }
        }
        private void AtualizarSubNiveisDoNo(TreeNode nodePai)
        {
            if (nodePai == null || nodePai.Tag == null)
                return;

            if (nodePai.Tag is NodeMetadata meta)
            {
                nodePai.Nodes.Clear();

                TreeNode atualizado = CriarTreeNodeRecursivo(meta.Valor, meta.NomeOriginal, meta.Pai, meta.Propriedade);

                foreach (TreeNode filho in atualizado.Nodes)
                    nodePai.Nodes.Add(filho);
            }
        }
        private object AbrirInputBoxPorTipo(NodeMetadata metadata)
        {
            // Guarda os forms visíveis
            List<Form> formsVisiveis = new();
            foreach (Form f in Application.OpenForms)
                if (f.Visible && f != this)
                    formsVisiveis.Add(f);

            foreach (var f in formsVisiveis)
                f.Visible = false;

            object resultado = null;
            object valorAtual = metadata.Valor;
            Type tipo = null;

            if (valorAtual != null)
                tipo = valorAtual.GetType();

            if (valorAtual == null && metadata.Propriedade != null)
                tipo = metadata.Propriedade.PropertyType;

            if (valorAtual == null && metadata.Propriedade == null)
            {
                MessageBox.Show($"Não foi possível determinar o tipo da propriedade '{metadata.NomeOriginal}'.");

                foreach (var f in formsVisiveis)
                    f.Visible = true;

                return null;
            }

            switch (tipo)
            {
                case Type _ when tipo == typeof(int):
                {
                    int valor = 0;
                    if (valorAtual != null)
                        valor = (int)valorAtual;

                    resultado = AbrirInputBoxInt32(valor);
                    break;
                }

                case Type _ when tipo == typeof(long):
                {
                    long valor = 0L;
                    if (valorAtual != null)
                        valor = (long)valorAtual;

                    resultado = AbrirInputBoxInt64(valor);
                    break;
                }

                case Type _ when tipo == typeof(double):
                {
                    double valor = 0.0;
                    if (valorAtual != null)
                        valor = (double)valorAtual;

                    resultado = AbrirInputBoxDouble(valor);
                    break;
                }

                case Type _ when tipo == typeof(string):
                {
                    string texto = "";
                    if (valorAtual != null)
                        texto = valorAtual.ToString();

                    resultado = AbrirInputBoxString(texto);
                    break;
                }

                case Type _ when tipo == typeof(bool):
                {
                    resultado = AbrirInputBoxBoolean();
                    break;
                }

                case Type _ when tipo == typeof(System.Drawing.Color):
                {
                    if (valorAtual != null)
                    {
                        System.Drawing.Color cor = (System.Drawing.Color)valorAtual;
                        resultado = BibliotecaAuxiliarForms.Input.InputBox.Color(
                            new BibliotecaAuxiliarForms.Input.InputBoxOpcoes.Color("Escolha uma cor:", $"Introduza o novo valor para a cor {metadata.NomeOriginal}", cor));
                    }
                    break;
                }

                case Type _ when tipo == typeof(System.Drawing.Font):
                {
                    if (valorAtual != null)
                    {
                        System.Drawing.Font fonte = (System.Drawing.Font)valorAtual;
                        resultado = BibliotecaAuxiliarForms.Input.InputBox.Font(
                            new BibliotecaAuxiliarForms.Input.InputBoxOpcoes.Font("Escolha uma fonte:", $"Introduza o novo valor para a fonte {metadata.NomeOriginal}", fonte));
                    }
                    break;
                }

                case Type _ when tipo == typeof(ConfigPassoAPasso):
                {
                    ConfigPassoAPasso valor = new ConfigPassoAPasso();
                    if (valorAtual is ConfigPassoAPasso p)
                        valor = p;

                    valor.Validate();
                    resultado = new InputBoxConfigPassoAPasso(valor).ShowDialog() == DialogResult.OK ? valor : null;
                    break;
                }

                case Type _ when tipo == typeof(List<ConfigPassoAPasso>):
                    {
                        List<ConfigPassoAPasso> listaOriginal = valorAtual as List<ConfigPassoAPasso> ?? new();
                        var form = new InputBoxPassosAPasso(listaOriginal);

                        if (form.ShowDialog() == DialogResult.OK && form.Resultado != null)
                        {
                            listaOriginal.Clear();
                            listaOriginal.AddRange(form.Resultado);
                            resultado = listaOriginal;
                        }

                        break;
                    }


                case Type _ when tipo == typeof(FuncaoBotaoMenu):
                {
                    FuncaoBotaoMenu valor = new FuncaoBotaoMenu();
                    if (valorAtual is FuncaoBotaoMenu f)
                        valor = f;

                    resultado = new InputBoxFuncaoBotaoMenu(valor).ShowDialog() == DialogResult.OK ? valor : null;
                    break;
                }

                case Type _ when tipo == typeof(List<FuncaoBotaoMenu>):
                {
                    List<FuncaoBotaoMenu> lista = new();
                    if (valorAtual is List<FuncaoBotaoMenu> l)
                        lista = l;

                    resultado = new InputBoxFuncoesDoMenu(lista).ShowDialog() == DialogResult.OK ? lista : null;
                    break;
                }

                case Type _ when tipo == typeof(FicheiroGenerico):
                {
                    FicheiroGenerico valor = new FicheiroGenerico();
                    if (valorAtual is FicheiroGenerico f)
                        valor = f;

                    resultado = new InputBoxFicheiroGenerico(valor).ShowDialog() == DialogResult.OK ? valor : null;
                    break;
                }

                default:
                {
                    MessageBox.Show($"O tipo {tipo.Name} não é suportado para edição.");
                    break;
                }
            }

            // Volta a mostrar os forms
            foreach (var f in formsVisiveis)
                f.Visible = true;

            return resultado;
        }
        public class InputBoxArrayImagens : Form
        {
            private ListBox lstImagens = new();
            private PictureBox picPreview = new();
            private Button btnAdicionar = new();
            private Button btnRemover = new();
            private Button btnSubir = new();
            private Button btnDescer = new();
            private Button btnOK = new();
            private Button btnCancelar = new();

            private List<string> imagensTemp = new(); // Caminhos absolutos temporários
            private readonly string pastaDestino;
            private readonly string nomeBaseImagem;

            public List<string> ResultadoAbsoluto { get; private set; } = new();

            public InputBoxArrayImagens(string pastaDestino, string nomeBaseImagem = "Imagem_")
            {
                this.pastaDestino = pastaDestino;
                this.nomeBaseImagem = nomeBaseImagem;

                Text = "Editar imagens";
                Size = new Size(600, 400);
                StartPosition = FormStartPosition.CenterParent;

                Directory.CreateDirectory(pastaDestino);

                InicializarComponentes();
                CarregarImagensExistentes();
            }

            private void InicializarComponentes()
            {
                lstImagens.Location = new Point(10, 10);
                lstImagens.Size = new Size(250, 300);
                lstImagens.SelectedIndexChanged += (s, e) => AtualizarPreview();

                picPreview.Location = new Point(270, 10);
                picPreview.Size = new Size(300, 200);
                picPreview.SizeMode = PictureBoxSizeMode.Zoom;

                btnAdicionar.Text = "Adicionar";
                btnAdicionar.Location = new Point(10, 320);
                btnAdicionar.Click += BtnAdicionar_Click;

                btnRemover.Text = "Remover";
                btnRemover.Location = new Point(100, 320);
                btnRemover.Click += (s, e) => RemoverSelecionado();

                btnSubir.Text = "↑";
                btnSubir.Location = new Point(200, 320);
                btnSubir.Click += (s, e) => MoverSelecionado(-1);

                btnDescer.Text = "↓";
                btnDescer.Location = new Point(240, 320);
                btnDescer.Click += (s, e) => MoverSelecionado(1);

                btnOK.Text = "OK";
                btnOK.Location = new Point(400, 320);
                btnOK.Click += BtnOK_Click;

                btnCancelar.Text = "Cancelar";
                btnCancelar.Location = new Point(480, 320);
                btnCancelar.Click += (s, e) => DialogResult = DialogResult.Cancel;

                Controls.AddRange([
                    lstImagens, picPreview, btnAdicionar, btnRemover,
            btnSubir, btnDescer, btnOK, btnCancelar
                ]);
            }
            private void CarregarImagensExistentes()
            {
                if (!Directory.Exists(pastaDestino))
                    return;

                var ficheiros = Directory.GetFiles(pastaDestino)
                    .OrderBy(f => f)
                    .ToList();

                foreach (var caminho in ficheiros)
                {
                    imagensTemp.Add(caminho);
                    lstImagens.Items.Add(Path.GetFileName(caminho));
                }

            }
            private void AtualizarPreview()
            {
                picPreview.Image?.Dispose();
                picPreview.Image = null;

                if (lstImagens.SelectedIndex >= 0 && lstImagens.SelectedIndex < imagensTemp.Count)
                {
                    try
                    {
                        using var fs = new FileStream(imagensTemp[lstImagens.SelectedIndex], FileMode.Open, FileAccess.Read);
                        using var temp = System.Drawing.Image.FromStream(fs);
                        picPreview.Image = new Bitmap(temp); // cópia interna
                    }
                    catch
                    {
                        picPreview.Image = null;
                    }
                }
            }
            private void BtnAdicionar_Click(object sender, EventArgs e)
            {
                using OpenFileDialog dlg = new()
                {
                    Title = "Selecionar imagens",
                    Filter = "Imagens|*.png;*.jpg;*.jpeg;*.gif;*.ico",
                    Multiselect = true
                };

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string caminho in dlg.FileNames)
                    {
                        imagensTemp.Add(caminho);
                        lstImagens.Items.Add(Path.GetFileName(caminho));
                    }
                }
            }
            private void MoverSelecionado(int direcao)
            {
                int idx = lstImagens.SelectedIndex;
                if (idx < 0 || idx + direcao < 0 || idx + direcao >= imagensTemp.Count)
                    return;

                (imagensTemp[idx], imagensTemp[idx + direcao]) = (imagensTemp[idx + direcao], imagensTemp[idx]);
                object item = lstImagens.Items[idx];
                lstImagens.Items[idx] = lstImagens.Items[idx + direcao];
                lstImagens.Items[idx + direcao] = item;
                lstImagens.SelectedIndex = idx + direcao;
            }
            private void RemoverSelecionado()
            {
                int idx = lstImagens.SelectedIndex;
                if (idx < 0 || idx >= imagensTemp.Count)
                    return;

                imagensTemp.RemoveAt(idx);
                lstImagens.Items.RemoveAt(idx);
                picPreview.Image = null;
            }
            private void BtnOK_Click(object sender, EventArgs e)
            {
                picPreview.Image?.Dispose();
                picPreview.Image = null;

                ResultadoAbsoluto.Clear();

                // Apagar todos os ficheiros atuais
                try
                {
                    foreach (var f in Directory.GetFiles(pastaDestino))
                        try { File.Delete(f); } catch { }
                }
                catch { }

                // Copiar todos os ficheiros em ordem
                int contador = 1;
                foreach (string origem in imagensTemp)
                {
                    try
                    {
                        string extensao = Path.GetExtension(origem);
                        string nomeFinal = $"{nomeBaseImagem}{contador++}{extensao}";
                        string destino = Path.Combine(pastaDestino, nomeFinal);

                        if (!string.Equals(Path.GetFullPath(origem), Path.GetFullPath(destino), StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(origem, destino, true);
                        }

                        ResultadoAbsoluto.Add(destino);
                    }
                    catch { }
                }

                DialogResult = DialogResult.OK;
            }
        }
        public class InputBoxConfigPassoAPasso : Form
        {
            private ConfigPassoAPasso Config;
            private TextBox txtTitulo;
            private TextBox[] txtBotoes = new TextBox[3];
            private Button[] btnEditarImgs = new Button[3];
            private Button btnOK, btnCancelar;

            public ConfigPassoAPasso Resultado { get; private set; }
            public InputBoxConfigPassoAPasso(ConfigPassoAPasso config)
            {
                Config = config ?? new ConfigPassoAPasso();
                Config.Validate();

                InicializarComponentes();
                PreencherCampos();
            }
            private void InicializarComponentes()
            {
                Text = "Editar Passo a Passo";
                Width = 450;
                Height = 300;
                StartPosition = FormStartPosition.CenterParent;

                Label lblTitulo = new() { Text = "Título:", Left = 10, Top = 20 };
                txtTitulo = new() { Left = 80, Top = 18, Width = 340 };

                Controls.Add(lblTitulo);
                Controls.Add(txtTitulo);

                for (int i = 0; i < 3; i++)
                {
                    var lbl = new Label
                    {
                        Text = $"Botão {i + 1}:",
                        Left = 10,
                        Top = 60 + i * 40,
                        Width = 70
                    };

                    txtBotoes[i] = new TextBox
                    {
                        Left = 80,
                        Top = 58 + i * 40,
                        Width = 180
                    };

                    btnEditarImgs[i] = new Button
                    {
                        Text = "Editar imagens...",
                        Left = 270,
                        Top = 56 + i * 40,
                        Width = 150
                    };

                    int idx = i;
                    btnEditarImgs[i].Click += (s, e) =>
                    {
                        string nomeBotao = txtBotoes[idx].Text.Trim();
                        if (string.IsNullOrWhiteSpace(nomeBotao))
                        {
                            MessageBox.Show("O nome do botão não pode estar vazio.");
                            return;
                        }

                        Config.NomeBotoes[idx] = nomeBotao; // <-- IMPORTANTE

                        string titulo = txtTitulo.Text.Trim();
                        if (string.IsNullOrWhiteSpace(titulo))
                        {
                            MessageBox.Show("O título do passo a passo não pode estar vazio.");
                            return;
                        }

                        Config.Titulo = titulo; // <-- Atualiza também o título

                        string pasta = Path.Combine(Ficheiros.Caminho, "Imagens", titulo, nomeBotao);
                        using var frm = new InputBoxArrayImagens(pasta);
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            Config.AdicionarImagensAoBotao(idx, frm.ResultadoAbsoluto.ToArray());
                        }
                    };

                    Controls.Add(lbl);
                    Controls.Add(txtBotoes[i]);
                    Controls.Add(btnEditarImgs[i]);
                }

                btnOK = new() { Text = "OK", DialogResult = DialogResult.OK, Left = 240, Top = 200 };
                btnCancelar = new() { Text = "Cancelar", DialogResult = DialogResult.Cancel, Left = 330, Top = 200 };

                btnOK.Click += (s, e) =>
                {
                    Config.Titulo = txtTitulo.Text.Trim();
                    for (int i = 0; i < 3; i++)
                        Config.NomeBotoes[i] = txtBotoes[i].Text.Trim();

                    Config.Validate();
                    Resultado = Config;
                };

                Controls.Add(btnOK);
                Controls.Add(btnCancelar);
            }
            private void PreencherCampos()
            {
                txtTitulo.Text = Config.Titulo;
                for (int i = 0; i < 3; i++)
                {
                    txtBotoes[i].Text = Config.NomeBotoes[i];
                }
            }
        }
        public class InputBoxPassosAPasso : Form
        {
            private ListBox listBox;
            private Button btnAdicionar, btnEditar, btnRemover, btnOK, btnCancelar;

            public List<ConfigPassoAPasso> Resultado { get; private set; }
            public InputBoxPassosAPasso(List<ConfigPassoAPasso> passos)
            {
                Resultado = passos.Select(p => new ConfigPassoAPasso
                {
                    Titulo = p.Titulo,
                    NomeBotoes = p.NomeBotoes.ToArray()
                }).ToList();

                InicializarComponentes();
                AtualizarLista();
            }
            private void InicializarComponentes()
            {
                Text = "Lista de Passo a Passo";
                Width = 500;
                Height = 400;
                StartPosition = FormStartPosition.CenterParent;

                listBox = new()
                {
                    Left = 10,
                    Top = 10,
                    Width = 460,
                    Height = 250
                };

                btnAdicionar = new() { Text = "Adicionar", Left = 10, Top = 270, Width = 100 };
                btnEditar = new() { Text = "Editar", Left = 120, Top = 270, Width = 100 };
                btnRemover = new() { Text = "Remover", Left = 230, Top = 270, Width = 100 };

                btnOK = new() { Text = "OK", DialogResult = DialogResult.OK, Left = 280, Top = 320 };
                btnCancelar = new() { Text = "Cancelar", DialogResult = DialogResult.Cancel, Left = 370, Top = 320 };

                btnAdicionar.Click += (s, e) =>
                {
                    var novo = new ConfigPassoAPasso();
                    novo.Validate();
                    var form = new InputBoxConfigPassoAPasso(novo);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        Resultado.Add(form.Resultado);
                        AtualizarLista();
                    }
                };

                btnEditar.Click += (s, e) =>
                {
                    if (listBox.SelectedIndex >= 0)
                    {
                        var atual = Resultado[listBox.SelectedIndex];
                        var form = new InputBoxConfigPassoAPasso(atual);
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            Resultado[listBox.SelectedIndex] = form.Resultado;
                            AtualizarLista();
                        }
                    }
                };

                btnRemover.Click += (s, e) =>
                {
                    if (listBox.SelectedIndex >= 0)
                    {
                        Resultado.RemoveAt(listBox.SelectedIndex);
                        AtualizarLista();
                    }
                };

                Controls.AddRange([
                    listBox,
            btnAdicionar, btnEditar, btnRemover,
            btnOK, btnCancelar
                ]);
            }
            private void AtualizarLista()
            {
                listBox.Items.Clear();
                for (int i = 0; i < Resultado.Count; i++)
                {
                    listBox.Items.Add($"Passo {i + 1}: {Resultado[i].Titulo}");
                }
            }
        }
        public class InputBoxFuncoesDoMenu : Form
        {
            private List<FuncaoBotaoMenu> Lista_Interna;
            private ListBox listBoxFuncoes;
            private Button btnAdicionar, btnEditar, btnRemover, btnOK, btnCancelar;

            public List<FuncaoBotaoMenu> Resultado { get; private set; }

            public InputBoxFuncoesDoMenu(List<FuncaoBotaoMenu> listaOriginal)
            {
                Lista_Interna = new();

                foreach (var f in listaOriginal)
                {
                    Lista_Interna.Add(new FuncaoBotaoMenu
                    {
                        Nome = f.Nome,
                        Tipo = f.Tipo,
                        Diretorio_Link = f.Diretorio_Link,
                        DocumentoPDF = f.DocumentoPDF
                    });
                }

                InicializarComponentes();
                AtualizarListBox();
            }

            private void InicializarComponentes()
            {
                listBoxFuncoes = new ListBox { Dock = DockStyle.Top, Height = 200 };
                btnAdicionar = new Button { Text = "Adicionar" };
                btnEditar = new Button { Text = "Editar" };
                btnRemover = new Button { Text = "Remover" };
                btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
                btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

                FlowLayoutPanel painelBotoes = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
                painelBotoes.Controls.AddRange(new Control[] { btnAdicionar, btnEditar, btnRemover, btnOK, btnCancelar });

                Controls.Add(listBoxFuncoes);
                Controls.Add(painelBotoes);

                btnAdicionar.Click += BtnAdicionar_Click;
                btnEditar.Click += BtnEditar_Click;
                btnRemover.Click += BtnRemover_Click;

                AcceptButton = btnOK;
                CancelButton = btnCancelar;
            }

            private void AtualizarListBox()
            {
                listBoxFuncoes.Items.Clear();
                foreach (var funcao in Lista_Interna)
                    listBoxFuncoes.Items.Add(funcao.ToString());
            }

            private void BtnAdicionar_Click(object sender, EventArgs e)
            {
                var nova = new FuncaoBotaoMenu();
                if (EditarFuncao(nova))
                {
                    Lista_Interna.Add(nova);
                    AtualizarListBox();
                }
            }

            private void BtnEditar_Click(object sender, EventArgs e)
            {
                int idx = listBoxFuncoes.SelectedIndex;
                if (idx >= 0)
                {
                    var funcao = Lista_Interna[idx];
                    if (EditarFuncao(funcao))
                        AtualizarListBox();
                }
            }

            private void BtnRemover_Click(object sender, EventArgs e)
            {
                int idx = listBoxFuncoes.SelectedIndex;
                if (idx >= 0)
                {
                    Lista_Interna.RemoveAt(idx);
                    AtualizarListBox();
                }
            }

            private bool EditarFuncao(FuncaoBotaoMenu funcao)
            {
                var form = new InputBoxFuncaoBotaoMenu(funcao);
                return form.ShowDialog() == DialogResult.OK;
            }

            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                if (DialogResult == DialogResult.OK)
                    Resultado = Lista_Interna;
                base.OnFormClosing(e);
            }
        }
        public class InputBoxFuncaoBotaoMenu : Form
        {
            private readonly FuncaoBotaoMenu Funcao;
            private TextBox txtNome;
            private ComboBox cmbTipo;
            private TextBox txtDiretorioLink;
            private Button btnEditarPDF, btnOK, btnCancelar;

            public InputBoxFuncaoBotaoMenu(FuncaoBotaoMenu funcao)
            {
                Funcao = funcao;
                InicializarComponentes();
                PreencherCampos();
            }

            private void InicializarComponentes()
            {
                Text = "Editar Função do Botão do Menu";
                Width = 420;
                Height = 260;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterParent;

                Label lblNome = new() { Text = "Nome:", AutoSize = true, Top = 20, Left = 10 };
                txtNome = new() { Left = 120, Top = 18, Width = 260 };

                Label lblTipo = new() { Text = "Tipo:", AutoSize = true, Top = 60, Left = 10 };
                cmbTipo = new() { Left = 120, Top = 58, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbTipo.Items.AddRange(new string[] { "PDF", "LINK", "FORMS" });
                cmbTipo.SelectedIndexChanged += CmbTipo_SelectedIndexChanged;

                Label lblDiretorio = new() { Text = "Diretório/Link:", AutoSize = true, Top = 100, Left = 10 };
                txtDiretorioLink = new() { Left = 120, Top = 98, Width = 260 };

                btnEditarPDF = new() { Text = "Editar PDF...", Left = 120, Top = 130, Width = 180, Visible = false };
                btnEditarPDF.Click += BtnEditarPDF_Click;

                btnOK = new() { Text = "OK", DialogResult = DialogResult.OK, Left = 220, Width = 80, Top = 170 };
                btnCancelar = new() { Text = "Cancelar", DialogResult = DialogResult.Cancel, Left = 310, Width = 80, Top = 170 };

                btnOK.Click += BtnOK_Click;

                Controls.AddRange([
                    lblNome, txtNome,
            lblTipo, cmbTipo,
            lblDiretorio, txtDiretorioLink,
            btnEditarPDF,
            btnOK, btnCancelar
                ]);

                AcceptButton = btnOK;
                CancelButton = btnCancelar;
            }

            private void PreencherCampos()
            {
                Funcao.Validate();
                txtNome.Text = Funcao.Nome;
                cmbTipo.SelectedItem = Funcao.Tipo.ToUpper();
                txtDiretorioLink.Text = Funcao.Diretorio_Link;
                AtualizarVisibilidadePDF();
            }

            private void BtnOK_Click(object sender, EventArgs e)
            {
                Funcao.Nome = txtNome.Text.Trim();
                Funcao.Tipo = cmbTipo.SelectedItem?.ToString() ?? "LINK";
                Funcao.Diretorio_Link = txtDiretorioLink.Text.Trim();
                Funcao.Validate();
            }

            private void CmbTipo_SelectedIndexChanged(object sender, EventArgs e)
            {
                AtualizarVisibilidadePDF();
            }

            private void AtualizarVisibilidadePDF()
            {
                string tipo = cmbTipo.SelectedItem?.ToString()?.ToUpper();
                btnEditarPDF.Visible = tipo == "PDF";
                txtDiretorioLink.Enabled = tipo != "PDF";

                if (tipo == "PDF")
                    txtDiretorioLink.Text = Path.GetFileNameWithoutExtension(Funcao.DocumentoPDF?.NomeFicheiro ?? "DocumentoDefault");
            }

            private void BtnEditarPDF_Click(object sender, EventArgs e)
            {
                if (Funcao.DocumentoPDF == null)
                    Funcao.DocumentoPDF = new FicheiroGenerico();

                var form = new InputBoxFicheiroGenerico(Funcao.DocumentoPDF);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Funcao.DocumentoPDF = form.Resultado;
                    txtDiretorioLink.Text = Path.GetFileNameWithoutExtension(Funcao.DocumentoPDF.NomeFicheiro);
                }
            }
        }
        public class InputBoxFicheiroGenerico : Form
        {
            private FicheiroGenerico Ficheiro;
            private TextBox txtNome;
            private Button btnSelecionar, btnOK, btnCancelar;

            public FicheiroGenerico Resultado { get; private set; }

            public InputBoxFicheiroGenerico(FicheiroGenerico ficheiro)
            {
                Ficheiro = new FicheiroGenerico
                {
                    NomeFicheiro = ficheiro.NomeFicheiro
                };

                InicializarComponentes();
                PreencherCampos();
            }

            private void InicializarComponentes()
            {
                Text = "Editar Ficheiro";
                Width = 460;
                Height = 160;
                StartPosition = FormStartPosition.CenterParent;

                Label lblNome = new() { Text = "Nome do Ficheiro:", Left = 10, Top = 20 };
                txtNome = new() { Left = 150, Top = 18, Width = 280, ReadOnly = true };

                btnSelecionar = new() { Text = "Selecionar Ficheiro...", Left = 150, Top = 50, Width = 200 };
                btnSelecionar.Click += BtnSelecionar_Click;

                btnOK = new() { Text = "OK", DialogResult = DialogResult.OK, Left = 250, Top = 90 };
                btnCancelar = new() { Text = "Cancelar", DialogResult = DialogResult.Cancel, Left = 340, Top = 90 };

                btnOK.Click += BtnOK_Click;

                Controls.AddRange([
                    lblNome, txtNome,
            btnSelecionar, btnOK, btnCancelar
                ]);
            }

            private void PreencherCampos()
            {
                txtNome.Text = Ficheiro.NomeFicheiro;
            }

            private void BtnSelecionar_Click(object sender, EventArgs e)
            {
                using OpenFileDialog dlg = new()
                {
                    Title = "Selecionar ficheiro",
                    Filter = "Todos os ficheiros|*.*"
                };

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Ficheiro.AdicionarFicheiro(dlg.FileName);
                    txtNome.Text = Ficheiro.NomeFicheiro;
                }
            }

            private void BtnOK_Click(object sender, EventArgs e)
            {
                Resultado = Ficheiro;
            }
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

                    DescriptionAttribute descAttr = metadata.Propriedade.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (descAttr != null)
                    {
                        string[] linhas = descAttr.Description.Split(new[] { "\n" }, StringSplitOptions.None);
                        listBoxDetalhes.Items.Add("Descrição:");
                        foreach (var linha in linhas)
                            listBoxDetalhes.Items.Add("  " + linha);
                    }
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
                //"IndiceTemaAtual",
                "Perfil",
                "Capacity"
            };

            foreach (var x in propriedadesIgnoradas)
                if (x.Trim().Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase))
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
                if (metadata.Propriedade != null && metadata.Propriedade.CanWrite)
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
            foreach (Form f in Application.OpenForms)
                    f.Visible = false;

            // Atualiza os valores da TreeView na configuração
            foreach (TreeNode no in TreeViewConfig.Nodes)
                AtualizarConfiguracao(no);

            // Atualiza propriedades que não estão na árvore
            if (Ficheiros.CarregarConfig(comboBoxPerfis.SelectedIndex, out AppConfig original))
                SincronizarPropriedadesForaDaTreeView(original, PerfilAtual);

            Ficheiros.SalvarConfig(comboBoxPerfis.SelectedIndex, PerfilAtual);
            Ficheiros.SalvarConfig(-1, PerfilAtual);
            Environment.SetEnvironmentVariable("IdPerfilAtual", comboBoxPerfis.SelectedIndex.ToString(), EnvironmentVariableTarget.User);
            GestorEncerramento.ExecutarReinicio();
        }
        private void SincronizarPropriedadesForaDaTreeView(AppConfig destino, AppConfig origem)
        {
            if (destino == null || origem == null)
                return;

            destino.Perfil = origem.Perfil;
            destino.IndiceTemaAtual = origem.IndiceTemaAtual;
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
            string novoNome = AbrirInputBoxString($"'{nomeAntigo}");

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

            AppConfig novo = new(PerfilAtual.VisualAplicacao, PerfilAtual.ConfiguracaoAplicacao, nome, PerfilAtual.IndiceTemaAtual);

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
