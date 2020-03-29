using Greet;
using Grpc.Core;
using Sum;
using PrimeNumbers;
using System;
using System.Threading.Tasks;
using System.Linq;
using Average;

namespace Client
{
    class Program
    {
        const string target = "127.0.0.1:50051";
        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successfully");
                }
            }));

            Console.WriteLine("Greet unary");
            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting()
            {
                FirstName = "Stanislaw",
                LastName = "Banski"
            };

            var request = new GreetingRequest()
            {
                Greeting = greeting
            };

            var response = client.Greet(request);

            Console.WriteLine(response.Result);
            Console.WriteLine("");



            Console.WriteLine("Sum service");
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
            Console.WriteLine("");



            Console.WriteLine("Greet stream");

            var requestForStream = new GreetManyTimesRequest() { Greeting = greeting };

            var responseStream = client.GreetManyTimes(requestForStream);


            while (await responseStream.ResponseStream.MoveNext())
            {
                Console.WriteLine(responseStream.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            Console.WriteLine("");



            Console.WriteLine("Prime numbers stream");

            var primeClient = new PrimeNumbersService.PrimeNumbersServiceClient(channel);

            var primeRequest = new PrimeRequest() { Number = new Number { A = 120 } };

            var responsePrimeStream = primeClient.ListPrimeNumbers(primeRequest);

            while (await responsePrimeStream.ResponseStream.MoveNext())
            {
                Console.WriteLine(responsePrimeStream.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            Console.WriteLine("");



            Console.WriteLine("Long greet client stream");

            var longRequest = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();

            foreach (int i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(longRequest);
            }

            await stream.RequestStream.CompleteAsync();

            var longResponse = await stream.ResponseAsync;
            Console.WriteLine(longResponse.Result);

            Console.WriteLine("Average client stream");

            var averageClient = new AverageService.AverageServiceClient(channel);

            var averageStream = averageClient.Average();

            foreach (int i in Enumerable.Range(1, 10))
            {
                await averageStream.RequestStream.WriteAsync(new AverageRequest() { Number = new AverageNumber() { A = i } } );
            }

            await averageStream.RequestStream.CompleteAsync();

            var averageResponse = await averageStream.ResponseAsync;
            Console.WriteLine(averageResponse.Result);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
