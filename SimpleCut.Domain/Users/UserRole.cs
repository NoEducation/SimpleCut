using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.Users
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public UserRole(int userRoleId, int userId, int roleId)
        {
            UserRoleId = userRoleId;
            UserId = userId;
            RoleId = roleId;
        }

        public UserRole()
        {}
    }
}
