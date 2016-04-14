using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;

namespace Core.DomainModels.ViewSettings
{
    public class ProductionViewSettings
    {
        [Key]
        public int Id { get; set; }

        public bool Weighted { get; set; }
        public bool Private { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public virtual ICollection<Stage> Stages { get; set; }
        public virtual ICollection<OpportunityCategory> Categories { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
