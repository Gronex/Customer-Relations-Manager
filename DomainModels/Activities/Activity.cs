using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.DomainModels.Comments;
using Core.DomainModels.Customers;
using Core.DomainModels.Users;

namespace Core.DomainModels.Activities
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual ActivityCategory Category { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Done { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? Time { get; set; }

        [Required, ForeignKey(nameof(Responsible))]
        public string ResponsibleId { get; set; }
        public virtual User Responsible { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }


        [ForeignKey(nameof(Person))]
        public int? PersonId { get; set; }
        public virtual Person Person { get; set; }

        public virtual  ICollection<ActivityComment> Comments { get; set; }
    }
}
