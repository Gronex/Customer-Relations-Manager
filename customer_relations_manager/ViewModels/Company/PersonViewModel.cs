using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels.Company
{
    public class PersonViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
