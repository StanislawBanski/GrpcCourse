using Greet;
using Grpc.Core;
using Sum;
using PrimeNumbers;
using System;
using System.Threading.Tasks;
using System.Linq;
using Average;
using Max;
using Sqrt;
using System.IO;

namespace Client
{
    class Program
    {
        const string target = "127.0.0.1:50051";
        static async Task Main(string[] args)
        {
            var clientCert = File.ReadAllText("ssl/client.crt");
            var clientKey = File.ReadAllText("ssl/client.key");
            var caCrt = File.ReadAllText("ssl/ca.crt");

            var channelCredentials = new SslCredentials(caCrt, new KeyCertificatePair(clientCert, clientKey));

            Channel channel = new Channel("localhost", 50051, channelCredentials);

            await channel.ConnectAsync().ContinueWith((task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successfully");
                }
            }));

            // Unary examples
            GreetUnary(channel);
            // Sum(channel);

            // Server stream examples
            // await GreetServerStream(channel);
            // await PrimeNumbersStream(channel);

            // Client stream examples
            // await LongGreetClientStream(channel);
            // await Average(channel);

            // Bi stream examples
            // await GreetEveryoneBi(channel);
            // await FindMaxBi(channel);

            // Exception codes example
            // Sqrt(channel);

            // Deadline example
            // GreetWithDelay(channel);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }

        private static void GreetUnary(Channel channel)
        {
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
        }

        private static void Sum(Channel channel)
        {
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
        }

        private static async Task GreetServerStream(Channel channel)
        {
            Console.WriteLine("Greet stream");
            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting()
            {
                FirstName = "Stanislaw",
                LastName = "Banski"
            };

            var requestForStream = new GreetManyTimesRequest() { Greeting = greeting };

            var responseStream = client.GreetManyTimes(requestForStream);


            while (await responseStream.ResponseStream.MoveNext())
            {
                Console.WriteLine(responseStream.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            Console.WriteLine("");
        }

        private static async Task PrimeNumbersStream(Channel channel)
        {
            Console.WriteLine("Prime numbers stream");

            var primeClient = new PrimeNumbersService.PrimeNumbersServiceClient(channel);

            var primeRequest = new PrimeRequest() { Number = new PrimeNumbers.Number { A = 120 } };

            var responsePrimeStream = primeClient.ListPrimeNumbers(primeRequest);

            while (await responsePrimeStream.ResponseStream.MoveNext())
            {
                Console.WriteLine(responsePrimeStream.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            Console.WriteLine("");
        }

        private static async Task LongGreetClientStream(Channel channel)
        {
            Console.WriteLine("Long greet client stream");
            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting()
            {
                FirstName = "Stanislaw",
                LastName = "Banski"
            };

            var longRequest = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();

            foreach (int i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(longRequest);
            }

            await stream.RequestStream.CompleteAsync();

            var longResponse = await stream.ResponseAsync;
            Console.WriteLine(longResponse.Result);
        }

        private static async Task Average(Channel channel)
        {
            Console.WriteLine("Average client stream");

            var averageClient = new AverageService.AverageServiceClient(channel);

            var averageStream = averageClient.Average();

            foreach (int i in Enumerable.Range(1, 10))
            {
                await averageStream.RequestStream.WriteAsync(new AverageRequest() { Number = new AverageNumber() { A = i } });
            }

            await averageStream.RequestStream.CompleteAsync();

            var averageResponse = await averageStream.ResponseAsync;
            Console.WriteLine(averageResponse.Result);
        }

        private static async Task GreetEveryoneBi(Channel channel)
        {
            Console.WriteLine("Greet everyone bi");
            var client = new GreetingService.GreetingServiceClient(channel);

            var stream = client.GreetEveryone();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received: " + stream.ResponseStream.Current.Result);
                }
            });

            Greeting[] greetings =
            {
                new Greeting() { FirstName = "Stanislaw", LastName = "Banski" },
                new Greeting() { FirstName = "John", LastName = "Doe" },
                new Greeting() { FirstName = "Adam", LastName = "Smith" }
            };

            foreach (var item in greetings)
            {
                Console.WriteLine("Sending : " + item.ToString());
                await stream.RequestStream.WriteAsync(new GreetEveryoneRequest() { Greeting = item });
            }

            await stream.RequestStream.CompleteAsync();


            await responseReaderTask;
        }

        private static async Task FindMaxBi(Channel channel)
        {
            Console.WriteLine("Greet everyone bi");
            var client = new MaxService.MaxServiceClient(channel);

            var stream = client.Max();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received: " + stream.ResponseStream.Current.Result);
                }
            });

            int[] numbers =
            {
                1,
                5,
                3,
                6,
                2,
                20
            };

            foreach (var item in numbers)
            {
                Console.WriteLine("Sending : " + item.ToString());
                await stream.RequestStream.WriteAsync(new MaxRequest() { Number = new Max.Number() { A = item } });
            }

            await stream.RequestStream.CompleteAsync();


            await responseReaderTask;
        }

        private static void Sqrt(Channel channel)
        {
            Console.WriteLine("Sqrt service");
            var sqrtClient = new SqrtService.SqrtServiceClient(channel);

            var number = new Sqrt.Number
            {
                A = -1
            };

            try
            {
                var sqrtRequest = new SqrtRequest()
                {
                    Number = number
                };

                var sqrtResponse = sqrtClient.SquareRoot(sqrtRequest);

                Console.WriteLine(sqrtResponse.Result);
                Console.WriteLine("");
            }
            catch (RpcException e)
            {
                Console.WriteLine("Error : " + e.Status.Detail);
            }
        }

        private static void GreetWithDelay(Channel channel)
        {
            Console.WriteLine("Greet unary with delay");
            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting()
            {
                FirstName = "Stanislaw",
                LastName = "Banski"
            };

            try
            {
                var request = new GreetWithDeadlineRequest()
                {
                    Greeting = greeting
                };

                var response = client.GreetWithDeadline(request, deadline: DateTime.UtcNow.AddMilliseconds(100));

                Console.WriteLine(response.Result);
                Console.WriteLine("");
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Error : " + e.Status.Detail);
            }
        }
    }
}
