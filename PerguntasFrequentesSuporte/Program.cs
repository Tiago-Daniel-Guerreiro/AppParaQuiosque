using System;
using BibliotecaAuxiliarForms.App;
using BibliotecaAuxiliarForms.Log;
using System.Collections.Generic;
using System.Drawing;
using BibliotecaAuxiliarForms.Config;
using BibliotecaAuxiliarForms.Ficheiros;
using BibliotecaAuxiliarForms.Emails;
using BibliotecaAuxiliarForms.Log.Output;
using System.IO;

namespace PerguntasFrequentesSuporte
{
    static class Program
    {
        static void Main()
        {
            List<ILogOutput> outputsLog = new()
            {
                new ArquivoLogOutput // Grava as logs em ficheiro
                (
                    ficheiroLogHelper: new GestorFicheiroLog
                    (
                        pastaDestino: Path.Combine(Ficheiros.Caminho, "Logs"), // Ainda não funciona corretamente
                        nomeFicheiro: "logs",
                        formatoFicheiro: "txt",
                        quantidadeMaxFicheiros: 3,
                        substituirLinhas: false,
                        maxLinhas: 400,
                        diasParaManterLogs: 7
                    ),
                    nivelMinimo: NivelLog.TRACE_DETAILED
                )
            };

            LoggerGestor.Configurar( new 
            (
                configOutput: new // Configuração avançada do serviço de logs
                (
                    juntarTudoEmUm: true,
                    enviarACada: 6000
                ),
                outputs: outputsLog
            ) );
            LoggerGestor.ObterLogger().TestarFicheiros();
            Dictionary<Type, Size> tamanhos = new()
            {
                { typeof(PassoAPasso), new Size(800, 600) },
                { typeof(Configuracoes), new Size(1024, 768) }
            };
            ConfiguracoesForms configForms = new
            (
                tipoForm_Entrada: typeof(Menu),
                tamanhosJanelas_Entrada: new BibliotecaAuxiliarForms.UI.TamanhoInicialJanelas(tamanhos)
            );
            ConfiguracoesApp configApp = new
            (
                configForms_Entrada: configForms,
                acaoAntesDeFechar: () =>
                {
                    LoggerGestor.ObterLogger().Log("Aplicação encerrada.", "Shutdown", NivelLog.INFO);
                    LoggerGestor.Parar();
                }
            );
            GestorApp.Iniciar
            (
                config: configApp,
                tipoForm: typeof(Menu)
            );
        }
    }
}
