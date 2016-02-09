using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.UserGroups
{
    public class UserGroupOpportunity
    {
        //Composit key of {UserGroupId, OpportunityId} configured with fluent api
        [ForeignKey(nameof(UserGroup))]
        public int UserGroupId { get; set; }
        public virtual UserGroup UserGroup { get; set; }

        [ForeignKey(nameof(Opportunity))]
        public int OpportunityId { get; set; }
        public virtual Opportunity.Opportunity Opportunity { get; set; }
    }
}
