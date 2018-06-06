using System.Web.Mvc;

namespace Perspective.Controllers
{
    public class HomeController : Controller
    {
        public static string updateDate;

        public ActionResult Index()
        {
            return View();
        }
    }
}
    