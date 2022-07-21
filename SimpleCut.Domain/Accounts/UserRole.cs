using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.Accounts
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AddedDate { get; set; }
        public int? AddedByUserId { get; set; }
        public DateTime? ModifedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
    }
}
