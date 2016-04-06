using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;

namespace Core.DomainModels.ViewSettings
{
    public class ActivityViewSettings
    {
        [Key]
        public int Id { get; set; }
        
        public bool Private { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; }
        public virtual User Owner { get; set; }
        
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
