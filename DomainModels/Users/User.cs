using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Core.DomainModels.UserGroups;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Core.DomainModels.Users
{
    public enum UserRole
    {
        Standard,
        Executive,
        Super,
    }

    public class User : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
        public bool Active { get; set; }
        public DateTime? EndDate { get; set; }

        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        public virtual ViewSettings ViewSettings { get; set; }

        public virtual ICollection<ProductionGoal> Goals { get; set; }
        [InverseProperty(nameof(Activity.PrimaryResponsible))]
        public virtual ICollection<Activity> Activities { get; set; }
        [InverseProperty(nameof(Activity.SecondaryResponsibles))]
        public virtual ICollection<Activity> SecondaryActivities { get; set; }
        public virtual ICollection<Opportunity.Opportunity> Opportunities { get; set; }

        public virtual ICollection<UserGroupUser> Groups { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
