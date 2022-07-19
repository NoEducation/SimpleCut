using MediatR;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Common.Dtos;
using Dapper;
using SimpleCut.Domain.Users;
using SimpleCut.Resources;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SimpleCut.Common.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Account.Queries
{
    public class GetAccountTokenQueryHandler : IRequestHandler<GetAccountTokenQuery, OperationResult<GetAccountTokenQueryResponse>>
    {
        private readonly IDbContext _context;
        private readonly IDispatcher _dispatcher;
        private readonly TokenOptions _tokenOptions;
        public GetAccountTokenQueryHandler(IDbContext context, IDispatcher dispatcher, IOptions<TokenOptions> options)
        {
            _context = context;
            _dispatcher = dispatcher;
            _tokenOptions = options.Value;
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

            var accessTokenResponse = await _dispatcher.Send(new GetAccessTokenQuery() { Password = request.Password });

            if (accessTokenResponse?.Result?.PasswordHash != user.Password)
            {
                result.AddError(AccountResources.UserDoesNotExistsErrorMessage);
                return result;
            }

            var refreshTokenKey = GenerateRefreshToken();
            var refreshToken = CreateRefreshToken(user, refreshTokenKey);
            var accessToken = GenerateAccessToken(new List<Claim>() 
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

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenTimeValid),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Secrete)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtHandler.CreateToken(descriptor);
            return jwtHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace(" - ", "");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
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
