using Dapper;
using SimpleCut.Domain.Accounts;
using SimpleCut.Domain.Accounts.Consts;
using SimpleCut.Domain.Accounts.Enums;
using SimpleCut.Infrastructure.Context;

namespace SimpleCut.IntegrationTests.ObjectMothers
{
    public class UserObjectMother
    {
        public static async Task<User> CreateUserAsync(IDbContext context,
            string role = Roles.Appointment,
            string login = "Test",
            string email = "test@test.pl",
            string password = "test1234",
            bool isConfirmed = false,
            bool isActive = true,
            string name = "test",
            string surname = "test",
            DateTime? birthDate= null,
            Gender gender = Gender.Male,
            string descipiton = null,
            DateTime? addedDate = null)
        {
            var user = new User()
            {
                Login = login,
                Email = email,
                Password = password,
                IsActive = isActive,
                IsConfirmed = isConfirmed,
                Name = name,
                Surname = surname,
                BirthDate = birthDate ?? DateTime.Now,
                Gender = gender,
                Description = descipiton,
                AddedDate = addedDate ?? DateTime.Now
            };

            var userId = await context.Connection.ExecuteScalarAsync<int>(@"
    INSERT INTO public.users(
        login,
        email,
        password, 
        isactive, 
        isconfirmed, 
        name, 
        surname,
        birthdate, 
        gender, 
        description, 
        addeddate)
	VALUES (@login,
        @email,
        @password,
        @isActive,
        @isConfirmed,
        @name,
        @surname,
        @birthDate,
        @gender,
        @descipiton,
        @addedDate);

        SELECT CURRVAL(pg_get_serial_sequence('public.users','userid');
", ToSqlParameters(user));

            user.UserId = userId;

            await context.Connection.ExecuteAsync($@"
            INSERT INTO public.userroles(userid, roleid, addeddate)
	        VALUES (userId,
    	        SELECT 
                    RoleId
			    FROM public.Roles
			    WHERE Name = '{Roles.Appointment}',    
            now());", new { userId = userId});

            return user;
        }
        private static object ToSqlParameters(User user)
        {
            var result = new
            {
                @login = user.Login,
                @email = user.Email,
                @password = user.Password,
                @isActive = user.IsActive,
                @isConfirmed = user.IsConfirmed,
                @name = user.Name,
                @surname = user.Surname,
                @birthDate = user.BirthDate,
                @gender = user.Gender,
                @descipiton = user.Description,
                @addedDate = user.AddedDate
            };

            return result;
        }
    }
}
