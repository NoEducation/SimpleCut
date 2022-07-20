using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.Users
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTimeOffset AddedDate { get; set; }
        public int? AddedByUserId { get; set; }
        public DateTimeOffset? ModifedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
    }
}
