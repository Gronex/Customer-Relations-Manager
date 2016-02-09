using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.Activities;

namespace Core.DomainModels.Comments
{
    public class ActivityComment : Comment
    {
        [ForeignKey(nameof(Activity))]
        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
