using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
