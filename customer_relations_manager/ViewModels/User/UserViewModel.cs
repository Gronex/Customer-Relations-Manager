using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.DomainModels.Users;

namespace customer_relations_manager.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        [Required]
        public UserRole Role { get; set; }
    }
}
