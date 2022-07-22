
using Dapper;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Options;
using SimpleCut.Domain.Accounts.Consts;
using SimpleCut.Domain.Accounts.Enums;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Services.Accounts;

namespace SimpleCut.Logic.Accounts.Commands
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly TokenOptions _tokenOptions;
        public CreateUserCommandHandler(IDbContext dbContext,
            IPasswordHasherService passwordHasher,
            IOptions<TokenOptions> tokenOptions)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _tokenOptions = tokenOptions.Value;
        }

        public async Task<OperationResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await Validate(request, result);

            if (!result.Success)
                return result;


            var passwordHash = _passwordHasher.GenerateHash(request.Password, _tokenOptions.Salt);

            var parameters = new
            {
                @email = request.Email,
                @login = request.Login,
                @password = passwordHash,
                @name = request.Name,
                @surname = request.Surname,
                @birthDate = request.BrithDate,
                @isActive = true,
                @isConfirmed = false,
                @gender = Gender.Male,
            };

            await _dbContext.Connection.ExecuteAsync(SqlInsert, parameters);

            return result;
        }

        private async Task Validate(CreateUserCommand command, OperationResult result)
        {
            var validation = await _dbContext.Connection.QuerySingleAsync<UserValidation>(@"
                SELECT 
                  CASE WHEN EXISTS(SELECT 1 FROM public.Users U2 WHERE u2.email = @email)
		   	            THEN 1
                      ELSE 0
                  END IsEmailInUse,
	              CASE WHEN EXISTS(SELECT 1 FROM public.Users U3 WHERE u3.login = @login)
		   	            THEN 1
                      ELSE 0
                  END IsNameInUse;
            ", new { @login = command.Login, @email = command.Email});

            if (validation.IsEmailInUse)
                result.AddError("Email jest już w użyciu", nameof(command.Email));

            if (validation.IsNameInUse)
                result.AddError("Login jest już w użyciu", nameof(command.Login));
        }

        private string SqlInsert => $@"
            INSERT INTO public.users(
               login, 
               email,
               password, 
               isactive,
               isconfirmed,
               name,
               surname,
               birthdate,
               gender,
               addeddate)
	         VALUES (
                @login, 
                @email,
                @password,
                @isConfirmed,
                @isActive,
                @name,
                @surname,
                @birthdate,
                @gender,
                now()
             );

            INSERT INTO public.userRoles(
                 userId,
                 roleId)
			 WITH usersCTE(userId) AS (
				SELECT CURRVAL(pg_get_serial_sequence('public.users','userid')) 
			 )
			 SELECT 
                (select userId from usersCTE) userId,
                RoleId
			 FROM public.Roles
			 WHERE Name = '{Roles.Appointment}';
            
        ";

        private class UserValidation
        {
            public bool IsEmailInUse { get; set; }
            public bool IsNameInUse { get; set; }
        }
    }
}
