using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Min 2, max 20 caratteri")]
        public string Name { get; set; }

        [StringLength(2500, ErrorMessage = "Max 2500 caratteri")]
        public string Description { get; set; }

    }
}