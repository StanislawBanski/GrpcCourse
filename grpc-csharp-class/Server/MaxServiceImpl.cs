using Grpc.Core;
using Max;
using System;
using System.Threading.Tasks;
using static Max.MaxService;

namespace ServerApp
{
    public class MaxServiceImpl: MaxServiceBase
    {
        public override async Task Max(IAsyncStreamReader<MaxRequest> requestStream, IServerStreamWriter<MaxResponse> responseStream, ServerCallContext context)
        {
            var currentMax = 0;

            while (await requestStream.MoveNext())
            {
                Console.WriteLine("Received : " + requestStream.Current.Number.A);

                if (requestStream.Current.Number.A > currentMax)
                {
                    currentMax = requestStream.Current.Number.A;

                    await responseStream.WriteAsync(new MaxResponse() { Result = currentMax });
                }
            }
        }
    }
}
