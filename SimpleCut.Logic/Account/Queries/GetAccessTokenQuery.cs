using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Logic.Account.Queries
{
    // It should be renamed
    public class GetAccessTokenQuery : IRequest<OperationResult<GetAccessTokenQueryResponse>>
    {
        public string? Password { get; set; }

        public GetAccessTokenQuery()
        {}

        public GetAccessTokenQuery(string password)
        {
            Password = password;
        }
    }
}
