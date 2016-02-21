using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Core.DomainModels.UserGroups;

namespace customer_relations_manager.ViewModels
{
    public class GroupViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
