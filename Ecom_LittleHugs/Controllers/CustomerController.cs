using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Ecom_LittleHugs.Controllers
{
    public class CustomerController : Controller
    { private myContext _context;
        public CustomerController(myContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {

            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            return View();
        }
    }
}
