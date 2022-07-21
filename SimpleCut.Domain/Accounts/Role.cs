using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.Accounts
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public Role()
        {}

        public Role(int roleId, string name, string? description)
        {
            RoleId = roleId;
            Name = name;
            Description = description;
        }
    }
}
