using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Core.DomainModels.Users;

namespace customer_relations_manager.ViewModels.User
{
    public class UserOverviewViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // ReSharper disable once ConvertPropertyToExpressionBody
        public string Name {get { return $"{FirstName} {LastName}"; } } 

        [Required]
        public UserRole Role { get; set; }
    }
}
