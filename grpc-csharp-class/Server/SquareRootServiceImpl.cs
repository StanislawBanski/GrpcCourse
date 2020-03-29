using Grpc.Core;
using Sqrt;
using System;
using System.Threading.Tasks;
using static Sqrt.SqrtService;

namespace ServerApp
{
    public class SquareRootServiceImpl: SqrtServiceBase
    {
        public override async Task<SqrtResponse> SquareRoot(SqrtRequest request, ServerCallContext context)
        {
            int number = request.Number.A;

            if (number >= 0)
            {
                return new SqrtResponse() { Result = Math.Sqrt(number) };
            } 
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Number below 0"));
            }
        }
    }
}
