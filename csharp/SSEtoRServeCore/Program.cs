using Grpc.Core;
using Qlik.Sse;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using NLog;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace SSEtoRserve
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder() 
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("config.xml");

                Configuration = builder.Build();

                //Convert.ToString(Configuration["grpcHost"] ?? "localhost");
                var grpcHost = ParameterValue("grpcHost", "localhost");
                int grpcPort = Convert.ToInt32(ParameterValue("grpcPort", "50051"));
                var rserveUser = Convert.ToString(ParameterValue("rserveUser", ""));
                var rservePassword = Convert.ToString(ParameterValue("rservePassword",""));
                var rservePoolNodes = Convert.ToString(ParameterValue("rservePoolNodes", ""));

                string rProcessPath;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    rProcessPath = Convert.ToString(Configuration["rProcessPathToStart-Win"] ?? "");
                }
                else
                {
                    rProcessPath = Convert.ToString(Configuration["rProcessPathToStart-Linux"] ?? "");
                }
                
                var rProcessCommandLineArgs = Convert.ToString(Configuration["rProcessCommandLineArgs"] ?? "");
                var rserveInitScript = Convert.ToString(Configuration["rserveInitScript"] ?? "");
                bool allowScript = Convert.ToBoolean(ParameterValue("allowScript","True"));
                var functionDefinitionsFile = Convert.ToString(Configuration["functionDefinitionsFile"] ?? "");

                

                var sslCredentials = ServerCredentials.Insecure;
                var certificateFolderFullPath = Convert.ToString(ParameterValue("certificateFolderFullPath",""));

                if (certificateFolderFullPath.Length > 3)
                {
                    var rootCertPath = Path.Combine(certificateFolderFullPath, @"root_cert.pem");
                    var serverCertPath = Path.Combine(certificateFolderFullPath, @"sse_server_cert.pem");
                    var serverKeyPath = Path.Combine(certificateFolderFullPath, @"sse_server_key.pem");
                    if (File.Exists(rootCertPath) &&
                        File.Exists(serverCertPath) &&
                        File.Exists(serverKeyPath))
                    {
                        var rootCert = File.ReadAllText(rootCertPath);
                        var serverCert = File.ReadAllText(serverCertPath);
                        var serverKey = File.ReadAllText(serverKeyPath);
                        var serverKeyPair = new KeyCertificatePair(serverCert, serverKey);
                        sslCredentials = new SslServerCredentials(new List<KeyCertificatePair>() { serverKeyPair }, rootCert, true);
                        logger.Info($"Path to certificates ({certificateFolderFullPath}) and certificate files found. Opening secure channel with mutual authentication.");
                    }
                    else
                    {
                        logger.Warn($"Path to certificates ({certificateFolderFullPath}) not found or files missing. Opening insecure channel instead.");
                    }
                }
                else
                {
                    logger.Info("No certificates defined. Opening insecure channel.");
                }

                
                var parameter = new RserveParameter(rserveInitScript, rProcessCommandLineArgs, rserveUser, rservePassword, rservePoolNodes);

                GrpcEnvironment.SetThreadPoolSize(100);
                GrpcEnvironment.SetCompletionQueueCount(100);

                using (var rServeEvaluator = new RServeEvaluator(parameter, allowScript, functionDefinitionsFile))
                {
                    var server = new Server
                    {
                        Services = { Connector.BindService(rServeEvaluator) },
                        Ports = { new ServerPort(grpcHost, grpcPort, sslCredentials) }
                    };
                    server.RequestCallTokensPerCompletionQueue = 32768;

                    server.Start();
                    //Console.WriteLine("Press any key to stop SSEtoRserve...");
                    logger.Info($"gRPC listening to host {grpcHost}");
                    logger.Info($"gRPC listening on port {grpcPort}");
                    //Console.ReadKey();
                    try {
                      while(true) {
                        Thread.Sleep(10000);
                      }
                    } finally {
                      logger.Info("Shutting down SSEtoRserve... Bye!");
                      server?.ShutdownAsync().Wait();
                      rServeEvaluator?.Dispose();
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error in main entry point of SSEtoRserve: {ex}");
                Console.WriteLine(ex);
                Console.ReadKey();

            }
        }

        public static string ParameterValue(string parameterName, string defaultValue)
        {

            var val = defaultValue;

            if (Configuration[parameterName] != "")
            {
                val = Configuration[parameterName];
            }

            try
            {
                
                if (Environment.GetEnvironmentVariable("sse2rserve_" + parameterName) != null)
                {
                    val = Convert.ToString(Environment.GetEnvironmentVariable("sse2rserve_" + parameterName));
                }
            }catch(Exception e)
            {
                logger.Error($"Error With Environment Variable: {e}");
                Console.WriteLine(e);
            }

            return val;
        }
    }
}
