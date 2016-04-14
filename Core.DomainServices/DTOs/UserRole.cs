using Core.DomainModels.Users;

namespace Core.DomainServices.DTOs
{
    public class UserRole
    {
        public User User { get; set; }
        public DomainModels.Users.UserRole Role { get; set; } 
    }
}
