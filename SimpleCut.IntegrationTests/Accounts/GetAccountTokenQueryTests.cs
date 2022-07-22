using NUnit.Framework;
using SimpleCut.IntegrationTests.ObjectMothers;
using SimpleCut.Logic.Accounts.Queries;

namespace SimpleCut.IntegrationTests.Accounts
{
    public class GetAccountTokenQueryTests : IntegrationTestBase
    {
        [Test]
        public async Task ValidRequest_UserExists_CorrectTokenReturned()
        {
            var user = await UserObjectMother.CreateUserAsync(this.Context);

            var query  = new GetAccountTokenQuery()
            {
                Login = user.Login,
                Password = user.Password
            };

            var result = await Dispatcher.SendAsync(query);

            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Errors);

            Assert.That(result.Result.UserId, Is.EqualTo(user.UserId));
            Assert.IsNotNull(result.Result.AccessToken);
            Assert.IsNotNull(result.Result.RefreshToken);
        }

        [Test]
        public void EmptyTest()
        {

        }
    }
}
