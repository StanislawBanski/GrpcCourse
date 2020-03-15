using Grpc.Core;
using PrimeNumbers;
using System;
using System.Threading.Tasks;
using static PrimeNumbers.PrimeNumbersService;

namespace ServerApp
{
    public class PrimeNumbersServiceImpl: PrimeNumbersServiceBase
    {
        public async override Task ListPrimeNumbers(PrimeRequest request, IServerStreamWriter<PrimeResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request: ");
            Console.WriteLine(request.ToString());

            var n = request.Number.A;
            var k = 2;

            while (request.Number.A > 1)
            {
                if (n % k == 0)
                {
                    n = n / k;
                    await responseStream.WriteAsync(new PrimeResponse() { Result = k });
                } 
                else
                {
                    k = k + 1;
                }
            }
        }
    }
}
