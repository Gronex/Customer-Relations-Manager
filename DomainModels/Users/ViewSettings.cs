using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Users
{
    public class ViewSettings
    {
        [Key, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int Month { get; set; }
    }
}
