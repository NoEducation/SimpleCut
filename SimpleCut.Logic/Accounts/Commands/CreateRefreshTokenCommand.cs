using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Commands
{
    public class CreateRefreshTokenCommand : ICommand
    {
        public int UserId { get; set; }
        public string? RefreshTokenKey { get; set; }
    }
}
