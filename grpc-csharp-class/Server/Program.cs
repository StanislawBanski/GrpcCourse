using System;
using System.IO;
using Greet;
using Grpc.Core;
using Sum;
using PrimeNumbers;
using Average;
using Max;
using Sqrt;
using System.Collections.Generic;

namespace ServerApp
{
    class Program
    {
        const int port = 50051;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                var serverCert = File.ReadAllText("ssl/server.crt");
                var serverKey = File.ReadAllText("ssl/server.key");
                var keypair = new KeyCertificatePair(serverCert, serverKey);
                var caCrt = File.ReadAllText("ssl/ca.crt");

                var credentials = new SslServerCredentials(new List<KeyCertificatePair>() { keypair }, caCrt, true);

                server = new Server()
                {
                    Services =
                    {
                        GreetingService.BindService(new GreetingServiceImpl()),
                        SumService.BindService(new SumServiceImpl()),
                        PrimeNumbersService.BindService(new PrimeNumbersServiceImpl()),
                        AverageService.BindService(new AverageServiceImpl()),
                        MaxService.BindService(new MaxServiceImpl()),
                        SqrtService.BindService(new SquareRootServiceImpl())
                    },

                    Ports = {new ServerPort("localhost", port, credentials)}
                };

                server.Start();
                Console.WriteLine($"Server is listening on port: {port}");
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine($"Server failed to start - {e.Message}");
                throw;
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}
