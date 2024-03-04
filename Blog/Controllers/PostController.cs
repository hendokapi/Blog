using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Post post)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            ViewBag.isValid = false;
            return View(post);
        }

    }
}