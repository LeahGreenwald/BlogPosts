using Blog_posts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_posts.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=BlogPostsPage;Integrated Security=true;";
        public IActionResult Index(int page)
        {
            if (page < 1)
            {
                page = 1;
            }
            BlogPostsDb db = new(_connectionString);
            BlogPostsViewModel vm = new BlogPostsViewModel
            {
                blogPosts = db.GetAllBlogPosts(page),
                currentPage = page,
                totalPages = db.GetTotalPages()
            };

            return View(vm);
        }
        public IActionResult ViewBlog(int id)
        {
            string name = Request.Cookies["name"];
            if (name == null)
            {
                name = "";
            }
            BlogPostsDb db = new(_connectionString);
            BlogViewModel vm = new BlogViewModel
            {
                BlogPost = db.GetBlogPostForId(id),
                comments = db.GetComments(id),
                Name = name
            };
            return View(vm);
        }
        public IActionResult MostRecent()
        {
            BlogPostsDb db = new(_connectionString);
            return Redirect($"/home/viewblog?id={db.GetMostRecentPost()}");
        }
        public IActionResult AddPost()
        {
            return View();
        }
        public IActionResult SubmitPost(BlogPost post)
        {
            BlogPostsDb db = new(_connectionString);
            db.AddPost(post);
            return Redirect("/home/mostRecent");
        }
        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            Response.Cookies.Append("name", comment.Name);
            BlogPostsDb db = new(_connectionString);
            db.AddComment(comment);
            return Redirect($"/home/viewBlog?id={comment.BlogPostId}");
        }
    }

}

