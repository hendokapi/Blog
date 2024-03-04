using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Display(Name = "Titolo")]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Il titolo deve avere min 10 e max 100 caratteri")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Contenuto")]
        public string Contents { get; set; }

        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }

        [Display(Name = "Autore")]
        public int AuthorId { get; set; }
    }
}