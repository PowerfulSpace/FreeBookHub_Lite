using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role;

namespace AuthService.UnitTests.Domain
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_ShouldSetAllFieldsCorrectly()
        {
            var email = "user@example.com";
            var passwordHash = "hashed_pwd";

            var user = new User(email, passwordHash);

            Assert.Equal(email, user.Email);
            Assert.Equal(passwordHash, user.PasswordHash);
            Assert.Equal(UserRole.User, user.Role);
            Assert.True(user.IsActive);
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_ShouldThrow()
        {
            var user = new User("email", "hash");
            user.Deactivate();

            var ex = Assert.Throws<InvalidOperationException>(() => user.Deactivate());
            Assert.Equal("The user is already deactivated.", ex.Message);
        }

        [Fact]
        public void PromoteTo_SameRole_ShouldThrow()
        {
            var user = new User("email", "hash");

            var ex = Assert.Throws<RoleAssignmentException>(() => user.PromoteTo(UserRole.User));
            Assert.Equal("The user already has a role User", ex.Message);
        }

        [Fact]
        public void PromoteTo_AdminFromUser_ShouldThrow()
        {
            var user = new User("email", "hash", UserRole.User);

            var ex = Assert.Throws<InvalidRolePromotionException>(() => user.PromoteTo(UserRole.Admin));
            Assert.Equal("Only a moderator can be promoted to administrator.", ex.Message);
        }

        [Fact]
        public void PromoteTo_AdminFromModerator_ShouldSucceed()
        {
            var user = new User("email", "hash", UserRole.Moderator);

            user.PromoteTo(UserRole.Admin);

            Assert.Equal(UserRole.Admin, user.Role);
        }
    }
}