using PS.AuthService.Domain.Enums;
using PS.AuthService.Domain.Exceptions.Role;

namespace PS.AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        protected User() { }

        public User(string email, string passwordHash, UserRole role = UserRole.User)
        {
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("The user is already deactivated.");

            IsActive = false;
        }

        public void PromoteTo(UserRole newRole)
        {
            if (Role == newRole)
                throw new RoleAssignmentException($"The user already has a role {newRole}");

            if (newRole == UserRole.Admin && Role != UserRole.Moderator)
                throw new InvalidRolePromotionException("Only a moderator can be promoted to administrator.");

            Role = newRole;
        }
    }
}
