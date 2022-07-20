using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.Users
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }
        public int UserId { get; set; }
        public string? Token { get; set; }
        public DateTimeOffset ExpiresDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? RevokedDate { get; set; }
        public string? ReplacedByToken { get; set; }

        [Computed]
        public bool IsExpired => DateTime.UtcNow >= ExpiresDate;

        [Computed]
        public bool IsActive => RevokedDate == null && !IsExpired;
    }
}
