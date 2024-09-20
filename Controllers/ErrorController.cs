using Microsoft.AspNetCore.Mvc;

namespace Project2.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error")]
        public IActionResult Error()
        {
            return View();
        }

        [Route("/NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
