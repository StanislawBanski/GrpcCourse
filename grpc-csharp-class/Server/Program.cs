using System;
using System.IO;
using Greet;
using Grpc.Core;
using Sum;

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
