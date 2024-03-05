using Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            var command = new SqlCommand("SELECT * FROM Posts", conn);
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

        // dettagli per gli utenti non loggati
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Title,Contents")] Post post)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();
                var command = new SqlCommand(@"
                    INSERT INTO Posts
                    (Title, Contents, CategoryId, AuthorId)
                    VALUES (@title, @contents, @categoryId, @authorId)", conn);
                command.Parameters.AddWithValue("@title", post.Title);
                command.Parameters.AddWithValue("@contents", post.Contents);
                command.Parameters.AddWithValue("@categoryId", 1); // TODO: update this
                command.Parameters.AddWithValue("@authorId", HttpContext.User.Identity.Name);
                var numRows = command.ExecuteNonQuery();

                return RedirectToAction("Index"); // TODO: ridirezionare alla show
            }
            ViewBag.isValid = false;
            return View(post);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            // TODO: valutare se mantenere i join
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
                if (HttpContext.User.Identity.Name.ToString() != reader["AuthorId"].ToString()) return RedirectToAction("Index");

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

            reader.Close();

            var commandListCategories = new SqlCommand("SELECT * FROM Categories", conn);
            var readerCategories = command.ExecuteReader();

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
            ViewBag.Categories = categories;
            conn.Close();

            return View(post);
        }

        [HttpPost]
        public ActionResult Edit(Post post)
        {
            // TODO: bisogna fare i controlli sull'Author
            // if (HttpContext.User.Identity.Name.ToString() != reader["AuthorId"].ToString()) return RedirectToAction("Index");
            return View();
        }
    }
}