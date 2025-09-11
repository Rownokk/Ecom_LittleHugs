using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            ViewBag.checkSession = HttpContext.Session.GetString("customerSession");
            return View();
        }

        public IActionResult customerLogin()
        {
  
            return View();

        }
             [HttpPost]
        public IActionResult customerLogin(string customerEmail, string customerPassword)
        {
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_email == customerEmail);
            if (customer != null && customer.customer_password == customerPassword)
            {
           
                HttpContext.Session.SetString("customerSession", customer.customer_id.ToString());

        
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "Incorrect Username or Password";
                return View();
            }
        }

        public IActionResult CustomerRegistration()
        {
            return View();

        }
        [HttpPost]
        public IActionResult CustomerRegistration(Customer customer)
        {
            _context.tbl_customer.Add(customer);
            _context.SaveChanges();

            return RedirectToAction("customerLogin");

        }

        public IActionResult customerLogout()
        {
            HttpContext.Session.Remove("customerSession");
            return RedirectToAction("index");

        }


    }
}
