using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.ViewSettings;

namespace Core.DomainModels.UserGroups
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }
        //Unique
        [Index(IsUnique = true)]
        public string Name { get; set; }
        
        public virtual ICollection<UserGroupUser> UserGroupUsers { get; set; }
        
        public virtual ICollection<UserGroupOpportunity> UserGroupOpportunities { get; set; }
        public virtual ICollection<ProductionViewSettings> ProductionViewSettings { get; set; }
        public virtual ICollection<ActivityViewSettings> ActivityViewSettingses{ get; set; }
    }
}
