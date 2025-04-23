using RunMate.RunMate.Domain.Enums;

namespace RunMate.User.RunMate.Application.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
