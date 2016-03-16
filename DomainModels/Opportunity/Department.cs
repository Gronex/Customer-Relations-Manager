using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.DomainModels.ViewSettings;

namespace Core.DomainModels.Opportunity
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<ProductionViewSettings> ProductionViewSettings { get; set; }
    }
}
