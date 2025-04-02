using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Linq;
using BibliotecaAuxiliarForms.Ficheiros;
using System.Text.Json;

namespace PerguntasFrequentesSuporte
{
    public static class Ficheiros
    {
        private static string Caminho_Interno;
        public static string Caminho
        {
            get
            {
                if (string.IsNullOrEmpty(Caminho_Interno))
                {
                   if(AcederConfig.ConfigAtual != null)
                        if (string.IsNullOrEmpty(Caminho_Interno))
                            throw new Exception("Caminho não foi definido após inicializar configurações.");
                }

                return Caminho_Interno;
            }
            private set
            {
                Caminho_Interno = value;
            }
        }
        public static bool EscreverVariaveisVaziasNoJson { get; private set; } = true;
        public static int IdPerfilAtual { get; private set; } = 0;
        public static AppConfig IniciarConfiguracoes()
        {
            string IdPerfil_stringGuardade = Environment.GetEnvironmentVariable("IdPerfilAtual", EnvironmentVariableTarget.User);
            string caminhoGuardado = Environment.GetEnvironmentVariable("CaminhoSalvamento", EnvironmentVariableTarget.User);

            bool variaveisNaoExistem = string.IsNullOrEmpty(IdPerfil_stringGuardade) || string.IsNullOrEmpty(caminhoGuardado);  // Verifica se as variáveis de ambiente já existem

            if (variaveisNaoExistem)
            {
                Caminho = "Pasta_Executavel";
                Environment.SetEnvironmentVariable("CaminhoSalvamento", Caminho, EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable("IdPerfilAtual", IdPerfilAtual.ToString(), EnvironmentVariableTarget.User);

                return IniciarConfiguracoes();
            }

            if (int.TryParse(Environment.GetEnvironmentVariable("IdPerfilAtual", EnvironmentVariableTarget.User), out int idPerfil_Int))
                IdPerfilAtual = idPerfil_Int;

            CaminhoGuardado caminho = new(caminhoGuardado);
            Caminho = Path.Combine(caminho.ObterCaminhoCompleto("AppParaQuisoque"), "Perfil");
            CorrigirIdDeFicheiros();
            AppConfig configuracoes = null;
            
            if (caminho.Tipo == TipoPasta.Recurso)
            {
                if (!CarregarConfiguracoesEmbutidas(out configuracoes))
                    configuracoes = null;
            }
            if (configuracoes == null)
            {
                if (caminho.Tipo != TipoPasta.Executavel)
                {
                    if (!Directory.Exists(Caminho))
                    {
                        Environment.SetEnvironmentVariable("CaminhoSalvamento", "Pasta_Executavel", EnvironmentVariableTarget.User);
                        configuracoes = new AppConfig(); // Se também falhar, usa o Padrão
                        SalvarConfig(IdPerfilAtual, configuracoes); // Salva em um arquivo novo
                        Application.Restart(); // Reinicia assim carregando as configurações padrão
                    }
                }
                
                if (!CarregarConfig(IdPerfilAtual, out configuracoes)) // Primeiro tenta carregar o perfil atual
                {
                    if (!CarregarConfig(0, out configuracoes)) // Se falhar, tenta carregar o perfil 0
                    {
                        configuracoes = new AppConfig(); // Se também falhar, usa o padrão e salva em um arquivo novo
                        SalvarConfig(IdPerfilAtual, configuracoes);
                    }
                }
            }
            return configuracoes;
        }
        public static bool CarregarConfig(int idPerfil, out AppConfig config)
        {
            config = null;

            string caminhoFicheiro = Path.Combine(Caminho, $"Perfil_{idPerfil}"+".json");

            if (!File.Exists(caminhoFicheiro))
                return false;

            try
            {
                if (File.Exists(caminhoFicheiro))
                {
                    config = new(JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(caminhoFicheiro), new FicheirosJsonConversores().ObterConfigs()));
                    
                    if (config != null)
                        return true;
                }
                
                return false;
            }
            catch (Exception erroInterno)
            {
                try
                {
                    File.Copy(caminhoFicheiro + ".json", caminhoFicheiro + "_ERRO.json", true);
                    File.Delete(caminhoFicheiro + ".json");
                }
                catch
                {
                    MessageBox.Show(
                        $"Erro ao abrir e criar cópia do ficheiro Perfil_{ idPerfil}:\n{erroInterno.Message}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show(
                    $"Erro ao analisar o ficheiro de configuração do perfil {idPerfil}:\n{erroInterno.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
        }
        public static void SalvarConfig(int idPerfil, AppConfig config)
        {

            if (config == null)
            {
                MessageBox.Show("Configuração inválida. O salvamento foi cancelado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(Caminho))
                Directory.CreateDirectory(Caminho);
            try
            {
                GestorFicheirosJSON.SalvarConfig(Path.Combine(Caminho, $"Perfil_{idPerfil}.json"), config, new FicheirosJsonConversores().ObterConfigs(EscreverVariaveisVaziasNoJson));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a configuração do perfil {idPerfil}:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static bool CarregarConfiguracoesEmbutidas(out AppConfig config)
        {
            string resourceName = "PerguntasFrequentesSuporte.Resources.ConfigPadrao.json";
            config = new AppConfig();
            try
            {
                if (GestorFicheirosJSON.CarregarConfigRecurso(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName), new FicheirosJsonConversores().ObterConfigs(), out AppConfig configCarregada))
                {
                    config = new AppConfig(configCarregada);
                    return true;
                }
                else
                    return false; // Também retorna null se a configuração embutida falhar
            }
            catch
            {
                return false; // Agora, em caso de erro, retorna null
            }
        }
        public static List<AppConfig> ListarPerfisGuardados()
        {
            List<AppConfig> perfis = new();

            if (!Directory.Exists(Caminho))
                return perfis;

            string[] ficheiros = Directory.GetFiles(Caminho, "Perfil_*.json");

            foreach (string ficheiro in ficheiros)
            {
                string nome = Path.GetFileNameWithoutExtension(ficheiro);

                if (!nome.StartsWith("Perfil_"))
                    continue;

                if (int.TryParse(nome["Perfil_".Length..], out int idPerfil))
                    if (CarregarConfig(idPerfil, out AppConfig config) && config != null)
                        perfis.Add(config);
            }
           
            List<AppConfig> resultado = new();  // Organiza a lista, colocando "Padrao" primeiro

            foreach (AppConfig perfil in perfis)
            {
                if (perfil.Perfil == "Padrao")
                    resultado.Insert(0, perfil);  // Adiciona o "Padrao" no início
                else
                    resultado.Add(perfil);  // Adiciona os outros perfis no final
            }

            return resultado;
        }
        public static void CorrigirIdDeFicheiros()
        {
            if (!Directory.Exists(Caminho))
                return;

            string[] ficheiros = Directory.GetFiles(Caminho, "Perfil_*.json");

            List<(AppConfig Config, string CaminhoOriginal)> lista = new();

            foreach (string caminho in ficheiros)
            {
                if (ObterIdDoFicheiro(caminho) < 0)
                    continue;

                try
                {
                    AppConfig config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(caminho), new FicheirosJsonConversores().ObterConfigs());

                    if (config != null)
                        lista.Add((config, caminho));
                }
                catch { }
            }

            // Ordenar, garantindo que "Padrao" vem primeiro
            List<(AppConfig, string)> ordenados = new();

            foreach (var item in lista)
            {
                if (item.Config.Perfil == "Padrao")
                    ordenados.Insert(0, item);
                else
                    ordenados.Add(item);
            }

            // Atualizar o IdPerfilAtual com base no nome do perfil atual
            string nomePerfilAtual = "";

            if (IdPerfilAtual >= 0 && IdPerfilAtual < ordenados.Count)
                nomePerfilAtual = ordenados[IdPerfilAtual].Item1.Perfil;

            int novoId = -1;

            for (int i = 0; i < ordenados.Count; i++)
            {
                if (ordenados[i].Item1.Perfil == nomePerfilAtual)
                {
                    novoId = i;
                    break;
                }
            }

            if (novoId == -1 && ordenados.Count > 0)
                novoId = ordenados.Count - 1;

            if (novoId == -1)
            {
                Application.Restart();
                return;
            }

            // Atualizar a variável IdPerfilAtual e guardar no ambiente
            IdPerfilAtual = novoId;
            Environment.SetEnvironmentVariable("IdPerfilAtual", IdPerfilAtual.ToString(), EnvironmentVariableTarget.User);

            // Regravar os ficheiros com os novos nomes
            for (int i = 0; i < ordenados.Count; i++)
            {
                AppConfig config = ordenados[i].Item1;
                string caminhoAntigo = ordenados[i].Item2;
                string caminhoNovo = Path.Combine(Caminho, "Perfil_" + i + ".json");

                try
                {
                    if (File.Exists(caminhoNovo))
                        File.Delete(caminhoNovo);

                    config.Validate();

                    GestorFicheirosJSON.SalvarConfig(caminhoNovo, config, new FicheirosJsonConversores().ObterConfigs(EscreverVariaveisVaziasNoJson));

                    if (!string.Equals(caminhoAntigo, caminhoNovo, StringComparison.OrdinalIgnoreCase))
                        File.Delete(caminhoAntigo);
                }
                catch { }
            }
        }
        private static int ObterIdDoFicheiro(string caminho)
        {
            string nome = Path.GetFileNameWithoutExtension(caminho);

            if (nome.StartsWith("Perfil_"))
                if (int.TryParse(nome["Perfil_".Length..], out int valor))
                    return valor;

            return -1;
        }
    }
    public static class ConfiguracoesHelper
    {
        public static ConfigGeral CriarConfigGeralBase()
        {
            return new ConfigGeral
            {
                PercentagemEcraParaBotoesAltura = 80,
                PercentagemEcraParaBotoesLargura = 100,
                BotoesPorLinha = 4,
                QuantidadeBotoes = 5,
                ConfiguracoesBotoesMenu = new List<FuncaoBotaoMenu>
                {
                    new FuncaoBotaoMenu 
                    { 
                        Nome = "Explicação \nWifi", 
                        Tipo = "FORMS", 
                        Diretorio_Link = "PassoAPasso[0]" 
                    },
                    new FuncaoBotaoMenu 
                    { 
                        Nome = "Esqueci-me \nda Senha", 
                        Tipo = "LINK", 
                        Diretorio_Link = "https://web.novalaw.unl.pt/Help.asp" 
                    },
                    new FuncaoBotaoMenu 
                    { 
                        Nome = "Office \n365", 
                        Tipo = "PDF", 
                        Diretorio_Link = "AjudaOffice.pdf" 
                    },
                    new FuncaoBotaoMenu 
                    { 
                        Nome = "Retirar \nSenha", 
                        Tipo = "LINK", 
                        Diretorio_Link = "https://hub.novalaw.pt/senhas?lang=pt" 
                    }
                    /*
                    new FuncaoBotaoMenu 
                    { 
                        Nome = "Abrir\nConfigurações", 
                        Tipo = "FORMS", 
                        Diretorio_Link = "Config" 
                    }
                    */
                },
                ConfiguracoesSair_Mostrar = new ConfigSair_Mostrar
                {
                    PosicaoEsquerda = false,
                    TextoBotaoMostrarEsconder = new Dictionary<string, string>
                    {
                        { "BotoesVisiveis", "Esconder" },
                        { "BotoesEscondidos", "Mostrar" },
                        { "FormsAberto", "Feche a janela." }
                    }
                }
            };
        }
        public static AparenciaGeral CriarAparenciaGeralBase()
        {
            return new AparenciaGeral
            {
                Titulo = "Menu de Botões",
                JanelasRedimensionaveis = true,
                JanelasMoveis = true,
                AparecerNaBarraDeTarefas = false,
                UsaIcone = false,
                CaminhoIcone = "",
                OpacidadeDoPrograma = 0.99,
                ProgramaNoTrayMenu = true,
                TamanhoJanelas = new TamanhoInicialJanelas(),
                Tema = new List<TemaVisual>
                {
                    new TemaVisual
                    {
                        NomeTema = "Claro",
                        CorFundo = Color.White,
                        CorDoContraste = Color.Black,
                        TamanhoDoContraste = 3,
                        VisualBotoes = JuntarVisualBotoes(
                            CriarConfigVisualFormsConfiguracoesPadrao(false), 
                            CriarConfigVisualFormsMenuPadrao(false),
                            CriarConfigVisualFormsPassoAPassoPadrao(false) )
                    },
                    new TemaVisual
                    {
                        NomeTema = "Escuro",
                        CorFundo = Color.Black,
                        CorDoContraste = Color.White,
                        TamanhoDoContraste = 3,
                        VisualBotoes = JuntarVisualBotoes(
                            CriarConfigVisualFormsConfiguracoesPadrao(true),
                            CriarConfigVisualFormsMenuPadrao(true),
                            CriarConfigVisualFormsPassoAPassoPadrao(true))
                    }
                }
            };
        }
        // Junta vários dicionários num só
        private static Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> JuntarVisualBotoes(params Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>[] dicts)
        {
            var result = new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>();
            foreach (var dic in dicts)
            {
                foreach (var kvp in dic)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }
        // Configuração visual padrão para o form "Configuracoes"
        public static Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> CriarConfigVisualFormsConfiguracoesPadrao(bool TemaEscuro)
        {
            Color CorPrincipal = Color.White, CorSecundaria = Color.Black;

            if (TemaEscuro)
                (CorPrincipal, CorSecundaria) = (CorSecundaria, CorPrincipal);

            return new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>
            {
                { "Configuracoes", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Default", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 10f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 4,
                                Margem = 10,
                                TamanhoContrasteBorda = 8
                            }
                        },
                        { "Escolha perfil", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 28f, FontStyle.Bold),
                                IntensidadeArredondarBorda = 4,
                                Margem = 20,
                                TamanhoContrasteBorda = 8
                            }
                        },
                        { "lista detalhes", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 20f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 0,
                                Margem = 0,
                                TamanhoContrasteBorda = 0
                            }
                        },
                        { "Arvore Configs", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 20f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 0,
                                Margem = 0,
                                TamanhoContrasteBorda = 0
                            }
                        },
                        { "Botao de Salvar/Sair", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial",13f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 3,
                                Margem = 20,
                                TamanhoContrasteBorda = 15
                            }
                        }
                    }
                }
            };
        }
        // Configuração visual padrão para o form "Menu"
        public static Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> CriarConfigVisualFormsMenuPadrao(bool TemaEscuro)
        {
            Color CorPrincipal = Color.White, CorSecundaria = Color.Black;

            if (TemaEscuro)
                (CorPrincipal, CorSecundaria) = (CorSecundaria, CorPrincipal);

            return new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>
            {
                { "Menu", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Default", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 10f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 4,
                                Margem = 0,
                                TamanhoContrasteBorda = 10
                            }
                        },
                        { "Botao de Acao", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 35f, FontStyle.Bold),
                                IntensidadeArredondarBorda = 4,
                                Margem = 10,
                                TamanhoContrasteBorda = 15
                            }
                        },
                        { "Botao de Sair ou Esconder/Mostrar", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 20f, FontStyle.Bold),
                                IntensidadeArredondarBorda = 3,
                                Margem = 15,
                                TamanhoContrasteBorda = 10
                            }
                        }
                    }
                }
            };
        }
        // Configuração visual padrão para o form "PassoAPasso"
        public static Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> CriarConfigVisualFormsPassoAPassoPadrao(bool TemaEscuro)
        {
            Color CorPrincipal = Color.White, CorSecundaria = Color.Black;

            if (TemaEscuro)
                (CorPrincipal, CorSecundaria) = (CorSecundaria, CorPrincipal);

            return new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>
            {
                { "PassoAPasso", new Dictionary<string, VisualBotoesPorJanela>
                    {
                        { "Default", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 10f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 4,
                                Margem = 10,
                                TamanhoContrasteBorda = 8
                            }
                        },
                        { "Botao de Categoria", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 14f, FontStyle.Bold),
                                IntensidadeArredondarBorda = 4,
                                Margem = 30,
                                TamanhoContrasteBorda = 8
                            }
                        },
                        { "Botao Sair", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 20f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 3,
                                Margem = 20,
                                TamanhoContrasteBorda = 8
                            }
                        },
                        { "Botao passar", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 20f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 3,
                                Margem = 20,
                                TamanhoContrasteBorda = 8
                            }
                        },
                        { "Texto passo atual", new VisualBotoesPorJanela()
                            {
                                CorContrasteBorda = CorSecundaria,
                                CorFundo = CorPrincipal,
                                CorTexto = CorSecundaria,
                                Fonte = new Font("Arial", 10f, FontStyle.Regular),
                                IntensidadeArredondarBorda = 3,
                                Margem = 20,
                                TamanhoContrasteBorda = 15
                            }
                        }
                    }
                }
            };
        }
        /*
        // Valida e corrige cada tema: verifica se o dicionário VisualBotoes tem exatamente os 3 forms e,
        // para cada form, se o array de VisualBotoesPorJanela corresponde ao padrão esperado.
        // Caso contrário, substitui essa parte pelo padrão e devolve o AppConfig atualizado.
        public static AppConfig VerificarConfig(AppConfig appConfig)
        {
            Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> padraoConfig = CriarConfigVisualConfigPadrao(false);
            Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> padraoMenu = CriarConfigVisualMenuPadrao(false);
            Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> padraoPasso = CriarConfigVisualPassoAPassoPadrao(false);

            Dictionary<string, Dictionary<string, VisualBotoesPorJanela>> padroes = JuntarVisualBotoes(
                padraoConfig, padraoMenu, padraoPasso
            );


            foreach (TemaVisual tema in appConfig.VisualAplicacao.Tema)
            {
                if (tema.VisualBotoes == null)
                    tema.VisualBotoes = new Dictionary<string, Dictionary<string, VisualBotoesPorJanela>>();

                List<string> chavesParaRemover = new List<string>();
                foreach (KeyValuePair<string, Dictionary<string, VisualBotoesPorJanela>> formExistente in tema.VisualBotoes)
                {
                    if (!padroes.ContainsKey(formExistente.Key))
                        chavesParaRemover.Add(formExistente.Key);
                }

                foreach (string chaveRemover in chavesParaRemover)
                {
                    tema.VisualBotoes.Remove(chaveRemover);
                }
            }

            return appConfig;
        }
        */
        // Dicionário com os forms e respetivos botões permitidos
        private static readonly Dictionary<string, string[]> FormsBotoes = new Dictionary<string, string[]>
        {
            { "Configuracoes", new string[] { "Escolha perfil", "lista detalhes", "Arvore Configs", "Botao de Salvar/Sair" } },
            { "Menu", new string[] { "Botao de Acao", "Botao de Sair ou Esconder/Mostrar" } },
            { "PassoAPasso", new string[] { "Botao de Categoria", "Botao Sair", "Botao passar", "Texto passo atual" } }
        };

        // Subprograma para obter o array de forms
        public static string[] ObterForms()
        {
            return FormsBotoes.Keys.ToArray();
        }

        // Subprograma que, dado o nome de um form, devolve um array com os nomes dos botões permitidos
        public static string[] ObterBotoes(string nomeForm)
        {
            if (FormsBotoes.ContainsKey(nomeForm))
            {
                return FormsBotoes[nomeForm];
            }
            // Se o form não existir, retorna um array vazio
            return new string[0];
        }
    }
}
