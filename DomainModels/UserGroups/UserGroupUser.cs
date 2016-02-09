using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.Users;

namespace Core.DomainModels.UserGroups
{
    public class UserGroupUser
    {
        //Composit key of {UserGroupId, UserId} configured with fluent api
        [ForeignKey(nameof(UserGroup))]
        public int UserGroupId { get; set; }
        public virtual UserGroup UserGroup { get; set; }

        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
