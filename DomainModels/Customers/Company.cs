using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.DomainModels.Activities;

namespace Core.DomainModels.Customers
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string Address { get; set; }
        
        public string City { get; set; }
        
        public string Country { get; set; }

        //String in order to support all countries, not sure if any has non number codes
        public string PostalCode { get; set; }
        
        public string WebSite { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}
