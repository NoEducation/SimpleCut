using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.Accounts
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }
        public int UserId { get; set; }
        public string? Token { get; set; }
        public DateTime ExpiresDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? ReplacedByToken { get; set; }

        [Computed]
        public bool IsExpired => DateTime.UtcNow >= ExpiresDate;

        [Computed]
        public bool IsActive => RevokedDate == null && !IsExpired;
    }
}
