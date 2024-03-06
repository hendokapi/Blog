using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using Blog.Models;

namespace Blog.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Prova");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Author author, bool keepLogged)
        {
            // cercare l'utente con author.Username e verificare che abbia author.Password nel DB
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var command = new SqlCommand(@"
                SELECT *
                FROM Authors
                WHERE Username = @username AND Password = @password
            ", conn);
            command.Parameters.AddWithValue("@username", author.Username);
            command.Parameters.AddWithValue("@password", author.Password);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                FormsAuthentication.SetAuthCookie(reader["AuthorId"].ToString(), keepLogged);
                return RedirectToAction("Index", "Post"); // TODO: alla pagina di pannello
            }

            TempData["ErrorLogin"] = true;
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Prova()
        {
            var authorId = HttpContext.User.Identity.Name;
            ViewBag.AuthorId = authorId;
            return View();
        }

        [Authorize, HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // sloggare l'utente
            FormsAuthentication.SignOut();

            // ridirezionarlo da qualche parte
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Prova");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude="AuthorId,Role")] Author author)
        {
            // verificare se tutti i campi sono validi
            if(ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();
                var command = new SqlCommand(@"
                    INSERT INTO Authors
                    (Username, Email, Password, Role)
                    VALUES (@username, @email, @password, @role)
                ", conn);
                command.Parameters.AddWithValue("@username", author.Username);
                command.Parameters.AddWithValue("@email", author.Email);
                command.Parameters.AddWithValue("@password", author.Password);
                command.Parameters.AddWithValue("@role", author.Role);
                var countRows = command.ExecuteNonQuery();
                return RedirectToAction("Index");
            }
            // se almeno un campo non è valido si restituisce la view che presenterà anche gli errori
            // NO redirect in questo caso
            return View();
        }
    }
}