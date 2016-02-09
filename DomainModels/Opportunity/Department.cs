using System.ComponentModel.DataAnnotations;

namespace Core.DomainModels.Opportunity
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }    
    }
}
