using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public string Text { get; set; }

        public DateTime Sent { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
    }
}
