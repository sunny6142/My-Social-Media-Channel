
using My_Social_Media_Channel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace My_Social_Media_Channel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PostController postController = new PostController();
            var post = postController.GetAllPost();
            return View(post);
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {

            return View();
        }
    }
}
