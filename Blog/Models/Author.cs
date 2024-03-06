using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Blog.Validations;

namespace Blog.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Min 3, max 20 caratteri")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Campo obbligatorio")]
        //[Unique(AcceptedDomain = "gmail.com", ErrorMessage = "Email già utilizzata oppure non è gmail")] //TODO: creare la validazione custom
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Min 3, max 15 caratteri")]
        public string Password { get; set; }

        public string Role { get; set; } = "editor";
    }
}