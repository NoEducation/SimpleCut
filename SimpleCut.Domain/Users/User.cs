using Dapper.Contrib.Extensions;
using SimpleCut.Domain.Users.Enums;

namespace SimpleCut.Domain.Users
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
        public DateTimeOffset BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset AddedDate { get; set; }
        public int? AddedByUserId { get; set; }
        public DateTimeOffset? ModifedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
    }
}
