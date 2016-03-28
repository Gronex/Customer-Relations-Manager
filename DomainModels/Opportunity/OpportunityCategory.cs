using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.ViewSettings;

namespace Core.DomainModels.Opportunity
{
    public class OpportunityCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<ProductionViewSettings> ProductionViewSettings { get; set; }
    }
}
