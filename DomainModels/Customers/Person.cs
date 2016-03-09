using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.Activities;

namespace Core.DomainModels.Customers
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string Email { get; set; }

        public DateTime? StartDate { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
    }
}
