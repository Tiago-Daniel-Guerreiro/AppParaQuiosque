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
            Email.TentarConverter("tiagodguerreiro@gmail.com", out Email email);

            // ================================
            //  CONFIGURA��O COMPLETA DOS LOGS
            // ================================
            List<ILogOutput> outputsLog = new()
            {
                new ArquivoLogOutput // Grava as logs em ficheiro
                (
                    ficheiroLogHelper: new GestorFicheiroLog
                    (
                        pastaDestino: Path.Combine(Ficheiros.Caminho, "Logs"),
                        nomeFicheiro: "logs",
                        formatoFicheiro: "txt",
                        quantidadeMaxFicheiros: 3,
                        substituirLinhas: false,
                        maxLinhas: 400,
                        diasParaManterLogs: 7
                    ),
                    nivelMinimo: NivelLog.TRACE_DETAILED
                ),
                new EmailLogOutput // Envia logs CRITICAL por email
                (
                    config: new ConfigEmail
                    (
                        servidorSmtp: "gmail.com",
                        portaSmtp: 587,
                        emailRemetente: email,
                        credencialAutenticacao: @"M:\\client_secret_128172112902 - oh7pim2o5nip24hhvc06slmbt9qfmsa0.apps.googleusercontent.com.json",
                        usarApiKey: true, // Define true para usar API do Gmail
                        caminho_credentials_json: @"M:\\client_secret_128172112902-oh7pim2o5nip24hhvc06slmbt9qfmsa0.apps.googleusercontent.com.json"
                    ),
                    NivelLog.TRACE_DETAILED
                ),
                new MensagemBoxLogOutput(nivelMinimo: NivelLog.TRACE_DETAILED), // Exibe logs em MessageBox
                new DebugLogOutput(nivelMinimo: NivelLog.TRACE_DETAILED), // Regista logs no Debug
                new TraceLogOutput(nivelMinimo: NivelLog.TRACE_DETAILED), // Regista logs no Trace
                new InterfaceGraficaLogOutput(nivelMinimo: NivelLog.TRACE_DETAILED) // Mostra logs numa janela gr�fica
            };

            LoggerGestor.Configurar( new 
            (
                configOutput: new // Configura��o avan�ada do servi�o de logs
                (
                    juntarTudoEmUm: true,
                    enviarACada: 10000,
                    emailDestino: "tiagodguerreiro@gmail.com"
                ),
                outputs: outputsLog
            ) );
            LoggerGestor.ObterLogger().TestarFicheiros();
            RegistoLog.Log("Inicializando aplica��o com todas as configura��es...", "Startup", NivelLog.INFO);

            // =====================================
            //  CONFIGURA��O COMPLETA DOS FORMS
            // =====================================
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

            // =====================================
            //  CONFIGURA��O COMPLETA DA APLICA��O
            // =====================================
            ConfiguracoesApp configApp = new
            (
                configForms_Entrada: configForms,
                acaoAntesDeFechar: () =>
                {
                    LoggerGestor.ObterLogger().Log("Aplica��o encerrada.", "Shutdown", NivelLog.INFO);
                    LoggerGestor.Parar();
                }
            );

            // =====================================
            //  INICIALIZA A APLICA��O COM TODAS AS CONFIGURA��ES
            // =====================================
            GestorApp.Iniciar
            (
                config: configApp,
                tipoForm: typeof(Menu)
            );
        }
    }
}

// Console
//Console Iniciada!
//Pressione qualquer tecla para sair...