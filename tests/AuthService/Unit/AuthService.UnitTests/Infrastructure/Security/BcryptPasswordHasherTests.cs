using PS.FreeBookHub_Lite.AuthService.Infrastructure.Security;

namespace AuthService.UnitTests.Infrastructure.Security
{
    public class BcryptPasswordHasherTests
    {
        private readonly BcryptPasswordHasher _hasher = new();

        [Fact]
        public void Hash_ShouldReturn_NonEmptyString()
        {
            var hash = _hasher.Hash("mypassword");
            Assert.False(string.IsNullOrWhiteSpace(hash));
        }

        [Fact]
        public void Verify_ShouldReturn_True_ForCorrectPassword()
        {
            var password = "securePassword!";
            var hash = _hasher.Hash(password);

            Assert.True(_hasher.Verify(password, hash));
        }

        [Fact]
        public void Verify_ShouldReturn_False_ForIncorrectPassword()
        {
            var hash = _hasher.Hash("correctPassword");

            Assert.False(_hasher.Verify("wrongPassword", hash));
        }
    }
}
