using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Core.DomainModels.Comments;
using Core.DomainModels.Customers;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;

namespace Core.DomainModels.Opportunity
{
    public class Opportunity
    {
        public Opportunity()
        {
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Amount { get; set; }
        [Required]
        public double HourlyPrice { get; set; }

        [Required]
        public int Percentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime ExpectedClose { get; set; }

        [Required, ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; }
        public virtual User Owner { get; set; }

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual OpportunityCategory Category { get; set; }

        [ForeignKey(nameof(Stage))]
        public int StageId { get; set; }
        public virtual Stage Stage { get; set; }

        [ForeignKey(nameof(Contact))]
        public int? ContactId { get; set; }
        public virtual Person Contact { get; set; }

        public virtual ICollection<UserGroupOpportunity> UserGroups { get; set; } = new HashSet<UserGroupOpportunity>();
        public virtual ICollection<OpportunityComment> Comments { get; set; } = new HashSet<OpportunityComment>();



        public static Expression<Func<Opportunity, bool>> InTimeRange(DateTime from, DateTime to)
        {
            return o =>
                from <= o.StartDate && to >= o.StartDate ||
                from <= o.EndDate && to >= o.EndDate ||
                from >= o.StartDate && to <= o.EndDate;
        }

        public static Expression<Func<Opportunity, bool>> ListFilter(
            int[] departmentIds, int[] stageIds, int[] categoriesIds, int[] userGroupsIds, string[] userEmails) => o =>
                (!departmentIds.Any() || departmentIds.Contains(o.DepartmentId)) &&
                (!stageIds.Any() || stageIds.Contains(o.StageId)) &&
                (!categoriesIds.Any() || categoriesIds.Contains(o.CategoryId)) &&
                (!userGroupsIds.Any() || o.UserGroups.Any(ug => userGroupsIds.Contains(ug.UserGroupId))) &&
                (!userEmails.Any() || userEmails.Contains(o.Owner.Email));
    }
}
