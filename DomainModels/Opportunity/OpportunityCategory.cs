using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Opportunity
{
    public class OpportunityCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
