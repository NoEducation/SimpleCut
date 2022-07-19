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

        public User(int userId,
         string login,
         string email,
         string password,
         bool isActive,
         bool isConfirmed,
         string name,
         string surname,
         DateTimeOffset birthDate,
         Gender gender,
         string? description)
        {
            UserId = userId;
            Login = login;
            Email = email;
            Password = password;
            IsActive = isActive;
            IsConfirmed = isConfirmed;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            Gender = gender;
            Description = description;
        }

        public User()
        {}
    }
}
