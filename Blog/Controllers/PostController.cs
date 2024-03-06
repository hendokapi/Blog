using Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Blog.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        // index per gli utenti non loggati
        [AllowAnonymous]
        public ActionResult Index()
        {
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            // TODO: fare la paginazione
            var command = new SqlCommand(@"
                SELECT *
                FROM Posts
            ", conn);
            var reader = command.ExecuteReader();
            
            List<Post> posts = new List<Post>();
            if (reader.HasRows)
            {
                while(reader.Read())
                {
                    var post = new Post();
                    post.PostId = (int)reader["PostId"];
                    post.Title = (string)reader["Title"];
                    post.Contents = (string)reader["Contents"];
                    post.CategoryId = (int)reader["CategoryId"];
                    post.AuthorId = (int)reader["AuthorId"];
                    posts.Add(post);
                }
            }

            return View(posts);
        }

        // pagina dettaglio dei post per gli utenti non loggati
        [AllowAnonymous]
        public ActionResult Show(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var command = new SqlCommand(@"
                SELECT * FROM Posts
                INNER JOIN Categories ON (Posts.CategoryId = Categories.CategoryId)
                INNER JOIN Authors ON (Posts.AuthorId = Authors.AuthorId)
                WHERE Posts.PostId = @postId
            ", conn);
            command.Parameters.AddWithValue("@postId", id);
            var reader = command.ExecuteReader();

            var post = new Post();
            if (reader.HasRows)
            {
                reader.Read();

                post.Category = new Category()
                {
                    CategoryId = (int)reader["CategoryId"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                };

                post.Author = new Author()
                {
                    AuthorId = (int)reader["AuthorId"],
                    Username = (string)reader["Username"],
                };

                post.PostId = (int)reader["PostId"];
                post.Title = (string)reader["Title"];
                post.Contents = (string)reader["Contents"];
                post.CategoryId = (int)reader["CategoryId"];
                post.AuthorId = (int)reader["AuthorId"];
            }

            return View(post);
        }


        public ActionResult Add()
        {
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var commandCategories = new SqlCommand("SELECT * FROM Categories", conn);
            var reader = commandCategories.ExecuteReader();

            var categories = new List<Category>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    categories.Add(new Category()
                    {
                        CategoryId = (int)reader["CategoryId"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                    });
                }
            }

            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Title,Contents,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();
                var command = new SqlCommand(@"
                    INSERT INTO Posts
                    (Title, Contents, CategoryId, AuthorId)
                    OUTPUT INSERTED.PostId
                    VALUES (@title, @contents, @categoryId, @authorId)
                ", conn);

                command.Parameters.AddWithValue("@title", post.Title);
                command.Parameters.AddWithValue("@contents", post.Contents);
                command.Parameters.AddWithValue("@categoryId", post.CategoryId);
                command.Parameters.AddWithValue("@authorId", HttpContext.User.Identity.Name);
                var postId = command.ExecuteScalar();

                return RedirectToAction("Show", new {id = postId});
            }

            // TODO: serve anche la lista delle categorie risolvere con TempData?
            return View(post);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var command = new SqlCommand(@"
                SELECT * FROM Posts
                WHERE Posts.PostId = @postId
            ", conn);
            command.Parameters.AddWithValue("@postId", id);
            var reader = command.ExecuteReader();

            var post = new Post();
            if (reader.HasRows)
            {
                reader.Read();
                if (HttpContext.User.Identity.Name.ToString() != reader["AuthorId"].ToString()) return RedirectToAction("Index");
                post.PostId = (int)reader["PostId"];
                post.Title = (string)reader["Title"];
                post.Contents = (string)reader["Contents"];
                post.CategoryId = (int)reader["CategoryId"];
                post.AuthorId = (int)reader["AuthorId"];
            }
            reader.Close();

            var commandListCategories = new SqlCommand("SELECT * FROM Categories", conn);
            var readerCategories = commandListCategories.ExecuteReader();

            var categories = new List<Category>();
            if (readerCategories.HasRows)
            {
                while (readerCategories.Read())
                {
                    var category = new Category()
                    {
                        CategoryId = (int)readerCategories["CategoryId"],
                        Name = (string)readerCategories["Name"],
                        Description = (string)readerCategories["Description"]
                    };
                    categories.Add(category);
                }
            }
            conn.Close();

            ViewBag.Categories = categories;
            return View(post);
        }

        [HttpPost]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();
                var command = new SqlCommand(@"
                    SELECT * FROM Posts
                    WHERE Posts.PostId = @postId
                ", conn);
                command.Parameters.AddWithValue("@postId", post.PostId);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    if (HttpContext.User.Identity.Name.ToString() != reader["AuthorId"].ToString()) return RedirectToAction("Index");
                }
                reader.Close();

                var commandUpdate = new SqlCommand(@"
                    UPDATE Posts
                    SET Title = @title, Contents = @contents, CategoryId = @categoryId
                    WHERE  Posts.PostId = @postId
                ", conn);
                commandUpdate.Parameters.AddWithValue("@title", post.Title);
                commandUpdate.Parameters.AddWithValue("@contents", post.Contents);
                commandUpdate.Parameters.AddWithValue("@categoryId", post.CategoryId);
                commandUpdate.Parameters.AddWithValue("@postId", post.PostId);

                commandUpdate.ExecuteNonQuery();

                return RedirectToAction("Show", new { id = post.PostId });
            }
            
            // TODO: centralizzare il metodo che recupera le categorie
            return View(post);
        }

        //[HttpPost]
        [AllowAnonymous]
        public JsonResult CheckTitle(string title)
        {
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var command = new SqlCommand(@"
                    SELECT * FROM Posts
                    WHERE Posts.Title = @title
                ", conn);
            command.Parameters.AddWithValue("@title", title);
            var reader = command.ExecuteReader();

            if (reader.HasRows) return Json(new { used = true }, JsonRequestBehavior.AllowGet);

            return Json(new { used = false }, JsonRequestBehavior.AllowGet);
        }
    }
}