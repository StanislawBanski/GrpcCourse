using System;
using System.IO;
using Greet;
using Grpc.Core;
using Sum;
using PrimeNumbers;
using Average;

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
                server = new Server()
                {
                    Services =
                    {
                        GreetingService.BindService(new GreetingServiceImpl()),
                        SumService.BindService(new SumServiceImpl()),
                        PrimeNumbersService.BindService(new PrimeNumbersServiceImpl()),
                        AverageService.BindService(new AverageServiceImpl())
                    },

                    Ports = {new ServerPort("localhost", port, ServerCredentials.Insecure)}
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
