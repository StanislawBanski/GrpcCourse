using Average;
using Grpc.Core;
using System.Threading.Tasks;
using static Average.AverageService;

namespace ServerApp
{
    public class AverageServiceImpl: AverageServiceBase
    {
        public override async Task<AverageResponse> Average(IAsyncStreamReader<AverageRequest> requestStream, ServerCallContext context)
        {
            var result = 0.0;
            var count = 0;

            while (await requestStream.MoveNext())
            {
                result += requestStream.Current.Number.A;
                count ++;
            }

            return new AverageResponse() { Result = result / count };
        }
    }
}
