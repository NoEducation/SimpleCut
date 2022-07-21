using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Commands
{
    public class RevokeRefreshTokenCommand : ICommand
    {
        public string? RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
