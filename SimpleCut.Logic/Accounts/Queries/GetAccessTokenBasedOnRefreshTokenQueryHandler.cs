using Dapper;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Services.Accounts;
using System.Security.Claims;

namespace SimpleCut.Logic.Accounts.Queries
{
    public class GetAccessTokenBasedOnRefreshTokenQueryHandler
        : IQueryHandler<GetAccessTokenBasedOnRefreshTokenQuery, GetAccessTokenBasedOnRefreshTokenQueryResponse>
    {
        private readonly IDbContext _context;
        private readonly ITokenService _tokenService;

        public GetAccessTokenBasedOnRefreshTokenQueryHandler(IDbContext context, 
            ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<OperationResult<GetAccessTokenBasedOnRefreshTokenQueryResponse>> Handle(GetAccessTokenBasedOnRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<GetAccessTokenBasedOnRefreshTokenQueryResponse>()
            {
                Result = new GetAccessTokenBasedOnRefreshTokenQueryResponse()
            };

            await Validate(request, result);

            if (!result.Success)
                return result;

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>()
                { new Claim(ClaimTypes.NameIdentifier, request.UserId.ToString())});

            result.Result.AccessToken = accessToken;

            return result;
        }

        private async Task Validate(GetAccessTokenBasedOnRefreshTokenQuery query, OperationResult<GetAccessTokenBasedOnRefreshTokenQueryResponse> result)
        {
            var parameters = new 
            { 
                @userId = query.UserId, 
                @refreshTokenKey = query.RefreshToken
            };

            var refresTokenExists = await this._context.Connection.QuerySingleAsync<bool>(@"
                  SELECT CASE
                        WHEN EXISTS(SELECT 1 FROM public.RefresTokens 
                                WHERE userId = @userId and token = @refreshTokenKey and isActive = true )
                            THEN 1
                       ELSE 0
                  END RefresTokenExists;
            ", parameters);


            if (!refresTokenExists)
                result.AddError("Refresh token does not exists", nameof(query.RefreshToken));
        }
    }
}
