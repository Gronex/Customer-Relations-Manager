using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Core.DomainModels.Users
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public virtual ViewSettings ViewSettings { get; set; }

        public virtual ICollection<ProductionGoal> Goals { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Opportunity.Opportunity> Opportunities { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
