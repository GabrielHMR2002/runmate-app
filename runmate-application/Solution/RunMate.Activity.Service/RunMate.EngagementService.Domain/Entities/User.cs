using RunMate.EngagementService.RunMate.EngagementService.Domain.Enums;

namespace RunMate.EngagementService.Domain.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public string? FullName { get; private set; }
        public DateTime BirthDate { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? LastLogin { get; private set; }
        public bool IsActive { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        private User() { }

        public User(
            string username,
            string passwordHash,
            string email,
            string? fullName,
            DateTime birthDate,
            UserRole role = UserRole.User)
        {
            Id = Guid.NewGuid();
            Username = username ?? throw new ArgumentNullException(nameof(username));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FullName = fullName;
            BirthDate = birthDate;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Update(
            string? email = null,
            string? fullName = null,
            DateTime? birthDate = null,
            UserRole? role = null)
        {
            if (email != null) Email = email;
            if (fullName != null) FullName = fullName;
            if (birthDate.HasValue) BirthDate = birthDate.Value;
            if (role.HasValue) Role = role.Value;

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string passwordHash)
        {
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordLogin()
        {
            LastLogin = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRole(UserRole role)
        {
            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            UpdatedAt = DateTime.UtcNow;
        }
    }
}