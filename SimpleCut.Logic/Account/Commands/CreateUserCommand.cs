using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Account.Commands
{
    public class CreateUserCommand : ICommand
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
