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

        public IActionResult customerProfile()
        {
            var sessionId = HttpContext.Session.GetString("customerSession");
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("customerLogin");
            }

            int customerId = int.Parse(sessionId);
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customerId);

            if (customer == null)
            {
                return RedirectToAction("customerLogin");
            }

            ViewData["category"] = _context.tbl_category.ToList();
            return View(customer);
        }

        // POST: Update Customer Profile
        [HttpPost]
        public IActionResult customerProfile(Customer updatedCustomer)
        {
            var sessionId = HttpContext.Session.GetString("customerSession");
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("customerLogin");
            }

            int customerId = int.Parse(sessionId);
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customerId);

            if (customer != null)
            {
                customer.customer_name = updatedCustomer.customer_name;
                customer.customer_phone = updatedCustomer.customer_phone;
                customer.customer_email = updatedCustomer.customer_email;
                customer.customer_password = updatedCustomer.customer_password;
                customer.customer_country = updatedCustomer.customer_country;
                customer.customer_city = updatedCustomer.customer_city;
                customer.customer_address = updatedCustomer.customer_address;

                _context.SaveChanges();
            }

            return RedirectToAction("customerProfile");
        }
    }
}
