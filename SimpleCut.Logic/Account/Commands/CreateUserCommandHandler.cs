using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Account.Commands
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IDbContext _dbContext;

        public CreateUserCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<OperationResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //var emailIsInUse = await this._context.Users.AnyAsync(x => x.Email == model.Email);
            //if (emailIsInUse)
            //    throw new ValidationArchitectureException(nameof(User.Name), "Email is in use");

            //var nameIsInUse = await this._context.Users.AnyAsync(x => x.Name == model.Name);
            //if (nameIsInUse)
            //    throw new ValidationArchitectureException(nameof(User.Name), "Name is in use");

            //var user = new User()
            //{
            //    AddedDate = DateTime.Now,
            //    Email = model.Email,
            //    Name = model.Name,
            //    RoleId = Roles.NormalUser,
            //};

            //var passwordHash = _passwordHasherService.GenerateHash(model.Password, GetSalt(user.AddedDate));

            //user.Password = passwordHash;

            //await this._context.Users.AddAsync(user);
            //await this._context.SaveChangesAsync();
        }
    }
}
