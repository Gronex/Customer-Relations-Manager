using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Comments
{
    public class OpportunityComment : Comment
    {
        [ForeignKey(nameof(Opportunity))]
        public int OpportunityId { get; set; }
        public virtual Opportunity.Opportunity Opportunity { get; set; }
    }
}
