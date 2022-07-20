using Dapper;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Options;
using SimpleCut.Domain.Users;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Infrastructure.Services.Accounts;
using SimpleCut.Resources;
using System.Security.Claims;

namespace SimpleCut.Logic.Account.Queries
{
    public class GetAccountTokenQueryHandler : IQueryHandler<GetAccountTokenQuery, GetAccountTokenQueryResponse>
    {
        private readonly IDbContext _context;
        private readonly IDispatcher _dispatcher;
        private readonly TokenOptions _tokenOptions;
        private readonly ITokenService _tokenService;
        public GetAccountTokenQueryHandler(IDbContext context, 
            IDispatcher dispatcher,
            IOptions<TokenOptions> options,
            ITokenService tokenService)
        {
            _context = context;
            _dispatcher = dispatcher;
            _tokenOptions = options.Value;
            _tokenService = tokenService;
        }

        public async Task<OperationResult<GetAccountTokenQueryResponse>> Handle(GetAccountTokenQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<GetAccountTokenQueryResponse>();

            var user = await _context.Connection.QuerySingleAsync<User>(@"
    SELECT userid, login, email, password, isactive, isconfirmed, name, surname, birthdate, gender, description
	    FROM public.users
    WHERE login = @login or email = @login;
            ", new { @login = request.Login });

            if (user is null)
            {
                result.AddError(AccountResources.UserDoesNotExistsErrorMessage);
                return result;
            }

            var accessTokenResponse = await _dispatcher.SendAsync(new GetAccessTokenQuery() { Password = request.Password });

            if (accessTokenResponse?.Result?.PasswordHash != user.Password)
            {
                result.AddError(AccountResources.UserDoesNotExistsErrorMessage);
                return result;
            }

            var refreshTokenKey = _tokenService.GenerateRefreshToken();
            var refreshToken = CreateRefreshToken(user, refreshTokenKey);
            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>() 
                { 
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) 
                }
            );

            var refreshTokens = await this._context.Connection.QueryAsync<RefreshToken>(@"
                SELECT refreshtokenid, userid, token, expiresdate, createddate, revokeddate, replacedbytoken
	            FROM public.refreshtokens
                WHERE userid = @userId
            ", new { @userId = user.UserId});

            if (refreshTokens?.Any() == true)
            {
                var lastAvailableToken = refreshTokens.SingleOrDefault(x => x.IsActive);

                if (lastAvailableToken != null)
                {
                    lastAvailableToken.RevokedDate = DateTimeOffset.Now;
                    lastAvailableToken.ReplacedByToken = refreshTokenKey;

                    await this._context.Connection.ExecuteAsync(@"
                        UPDATE public.refreshtokens
                            SET RevokedDate = NOW(),
                                ReplacedByToken = @refreshTokenKey
                        WHERE refreshtokenid = @refreshtokenid
                    ", new { @refreshTokenKey = refreshTokenKey, @refreshtokenid = lastAvailableToken.RefreshTokenId });
                }
            }

            await this._context.Connection.ExecuteAsync(@"
                INSERT INTO public.refreshtokens(
	                userid, token, expiresdate, createddate, revokeddate, replacedbytoken)
	            VALUES (@userid, @token, @expiresdate, @createddate, null, null);
            ", new { @userid  = refreshToken.UserId, @token = refreshToken.Token, @expiresdate = refreshToken.Token, @createddate = refreshToken.CreatedDate });

            return new OperationResult<GetAccountTokenQueryResponse>()
            {
                Result = new GetAccountTokenQueryResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenKey
                }
            };
        }

        private RefreshToken CreateRefreshToken(User user, string refreshTokenKey)
        {
            var entity = new RefreshToken()
            {
                CreatedDate = DateTimeOffset.Now,
                ExpiresDate = DateTimeOffset.Now.AddDays(_tokenOptions.RefreshTokenTimeValid),
                Token = refreshTokenKey,
                UserId = user.UserId,
            };

            return entity;
        }
    }
}
