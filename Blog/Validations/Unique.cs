using Blog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Blog.Validations
{
    public class Unique : ValidationAttribute
    {
        public string AcceptedDomain { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!value.ToString().Contains(AcceptedDomain)) return new ValidationResult(ErrorMessage);

            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var command = new SqlCommand(@"
                SELECT *
                FROM Authors
                WHERE Email = @email
            ", conn);
            command.Parameters.AddWithValue("@email", (string)value);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                // valore non valido
                return new ValidationResult(ErrorMessage);
            }
            else
            {
                // valore valido
                return ValidationResult.Success;
            }
        }
    }
}