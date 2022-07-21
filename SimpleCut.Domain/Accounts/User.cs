using Dapper.Contrib.Extensions;
using SimpleCut.Domain.Accounts.Enums;

namespace SimpleCut.Domain.Accounts
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfirmed { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? Description { get; set; }
        public DateTime AddedDate { get; set; }
        public int? AddedByUserId { get; set; }
        public DateTime? ModifedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
    }
}
