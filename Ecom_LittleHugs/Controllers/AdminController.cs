using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_LittleHugs.Controllers
{
    public class AdminController : Controller
    {
        private myContext _context;
        public AdminController(myContext context) 
        {
           _context = context;  
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
           return View();
        }
        [HttpPost]
        public IActionResult Login(string adminEmail, string adminPassword)
        {
          var row=  _context.tbl_admin
                .FirstOrDefault(a => a.admin_email == adminEmail && a.admin_password == adminPassword);
            if (row != null && row.admin_password == adminPassword)
            {
              HttpContext.Session.SetString("admin_session",row.admin_id.ToString());
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "Incorrect Username or Password";
                return View();
            }
                
        }
    }
}
