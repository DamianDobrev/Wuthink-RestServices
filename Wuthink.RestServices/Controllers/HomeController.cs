using System;
using System.Web.Http;
using System.Web.Mvc;

namespace Wuthink.RestServices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Redirect to help page
            return this.Redirect("Help");
        }
    }
}
