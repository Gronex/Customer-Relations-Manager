using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels.User
{
    public class UserCredentialsViewModel
    {
        [EmailAddress, Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
