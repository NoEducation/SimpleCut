using SimpleCut.Domain.Accounts.Enums;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Commands
{
    public class CreateUserCommand : ICommand
    {
        public string? Login { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public Gender Gender { get; set; }
        public DateTime BrithDate { get; set; }
    }
}
