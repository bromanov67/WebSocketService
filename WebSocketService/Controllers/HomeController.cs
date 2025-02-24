using Microsoft.AspNetCore.Mvc;

namespace WebSocketService.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendMessage()
        {
            return View();
        }

        public IActionResult LiveMessages()
        {
            return View();
        }

        public IActionResult MessageHistory()
        {
            return View();
        }
    }
}
