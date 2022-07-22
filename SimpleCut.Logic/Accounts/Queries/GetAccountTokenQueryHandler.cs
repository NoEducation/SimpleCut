using Dapper;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Options;
using SimpleCut.Domain.Accounts;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Services.Accounts;
using SimpleCut.Resources;
using System.Security.Claims;

namespace SimpleCut.Logic.Accounts.Queries
{
    public class GetAccountTokenQueryHandler : IQueryHandler<GetAccountTokenQuery, GetAccountTokenQueryResponse>
    {
        private readonly IDbContext _context;
        private readonly TokenOptions _tokenOptions;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasherService;
        public GetAccountTokenQueryHandler(IDbContext context,
            IOptions<TokenOptions> options,
            ITokenService tokenService,
            IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _tokenOptions = options.Value;
            _tokenService = tokenService;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<OperationResult<GetAccountTokenQueryResponse>> Handle(GetAccountTokenQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<GetAccountTokenQueryResponse>();

            var user = await Validate(request, result);

            if (!result.Success)
                return result;

            var refreshTokenKey = _tokenService.GenerateRefreshToken();

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>() 
                { 
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) 
                }
            );

            return new OperationResult<GetAccountTokenQueryResponse>()
            {
                Result = new GetAccountTokenQueryResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenKey,
                    UserId = user.UserId
                }
            };
        }

        public async Task<User> Validate(GetAccountTokenQuery query, OperationResult result)
        {
            var user = await _context.Connection.QuerySingleAsync<User>(@"
    SELECT userid, 
        login,
        email,
        password, 
        isactive, 
        isconfirmed, 
        name, 
        surname,
        birthdate, 
        gender, 
        description, 
        addeddate, 
        addedbyuserid, 
        modifeddate, 
        modifiedbyuserid
    FROM public.users
    WHERE login = @login or email = @login;
            ", new { @login = query.Login });

            if (user is null)
            {
                result.AddError(AccountResources.UserDoesNotExistsErrorMessage, nameof(query.Login));
                return user;
            }

            var passwordMathHash = _passwordHasherService.CompareHashWithPassword(user.Password, query.Password, _tokenOptions.Salt);

            if (!passwordMathHash)
            {
                result.AddError("Hasło jest niepoprawne", nameof(query.Password));
            }

            return user;
        }
    }
}
