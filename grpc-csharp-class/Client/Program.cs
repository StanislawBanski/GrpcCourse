using Greet;
using Grpc.Core;
using Sum;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        const string target = "127.0.0.1:50051";
        static void Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            channel.ConnectAsync().ContinueWith((task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successfully");
                }
            }));

            // var client = new DummyService.DummyServiceClient(channel);
            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting() { 
                FirstName = "Stanislaw",
                LastName = "Banski"
            };

            var request = new GreetingRequest() {
                Greeting = greeting
            };

            var response = client.Greet(request);

            Console.WriteLine(response.Result);


            var sumClient = new SumService.SumServiceClient(channel);

            var numbers = new NumbersPair()
            {
                A = 10002.45,
                B = 10005.55
            };

            var sumRequest = new SumRequest()
            {
                NumbersPair = numbers
            };

            var sumResponse = sumClient.Sum(sumRequest);

            Console.WriteLine(sumResponse.Result);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
