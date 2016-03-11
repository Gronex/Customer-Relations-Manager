using System.ComponentModel.DataAnnotations;

namespace customer_relations_manager.ViewModels.Company
{
    public class CompanyViewModel
    {
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        public int? PostalCode { get; set; }
        public string WebSite { get; set; }
    }
}
