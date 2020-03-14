using Grpc.Core;
using Sum;
using System.Threading.Tasks;
using static Sum.SumService;

namespace ServerApp
{
    public class SumServiceImpl: SumServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            var result = request.NumbersPair.A + request.NumbersPair.B;

            return Task.FromResult(new SumResponse() { Result = result });
        }
    }
}
