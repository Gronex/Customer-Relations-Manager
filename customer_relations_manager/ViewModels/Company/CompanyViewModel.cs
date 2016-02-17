using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels
{
    public class CompanyViewModel
    {
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public int PostalCode { get; set; }
        public string WebSite { get; set; }
    }
}
