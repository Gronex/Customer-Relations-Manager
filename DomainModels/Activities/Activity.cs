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

        public DateTime? DueTime { get; set; }
        
        [Required, ForeignKey(nameof(PrimaryResponsible))]
        public string PrimaryResponsibleId { get; set; }
        [InverseProperty(nameof(User.Activities))]
        public virtual User PrimaryResponsible { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [ForeignKey(nameof(PrimaryContact))]
        public int? PrimaryContactId { get; set; }
        [InverseProperty(nameof(Person.Activities))]
        public virtual Person PrimaryContact { get; set; }

        [InverseProperty(nameof(Person.SecondaryActivities))]
        public virtual ICollection<Person> SecondaryContacts { get; set; }
        [InverseProperty(nameof(User.SecondaryActivities))]
        public virtual ICollection<User> SecondaryResponsibles { get; set; }
        public virtual ICollection<ActivityComment> Comments { get; set; }
    }
}
