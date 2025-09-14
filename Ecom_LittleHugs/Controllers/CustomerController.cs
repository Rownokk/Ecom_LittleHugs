using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_LittleHugs.Controllers
{
    public class CustomerController : Controller
    {
        private readonly myContext _context;
        private readonly IWebHostEnvironment _env;

        public CustomerController(myContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
            if (!int.TryParse(sessionId, out var customerId))
            {
                return RedirectToAction("customerLogin");
            }

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
            if (!int.TryParse(sessionId, out var customerId))
            {
                return RedirectToAction("customerLogin");
            }

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

        [HttpPost]
        public IActionResult updateCustomerProfile(Customer customer)
        {
            _context.tbl_customer.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("customerProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeCustomerImage(int customer_id, IFormFile? customer_image)
        {
            if (customer_image == null || customer_image.Length == 0)
                return RedirectToAction("CustomerProfile", new { id = customer_id });

            var existingCustomer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customer_id);
            if (existingCustomer == null) return NotFound();

            // 🔁 use plural folder to match your disk
            var uploadsFolder = Path.Combine(_env.WebRootPath, "customer_images");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(customer_image.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                customer_image.CopyTo(stream);
            }

            // optional: delete old file
            if (!string.IsNullOrWhiteSpace(existingCustomer.customer_image))
            {
                var oldPath = Path.Combine(uploadsFolder, existingCustomer.customer_image);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            existingCustomer.customer_image = uniqueFileName;
            _context.SaveChanges();

            return RedirectToAction("CustomerProfile", new { id = customer_id });
        }
        public IActionResult feedback()
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            return View();
        }
        [HttpPost]
        public IActionResult Feedback(Feedback feedback)
        {
            _context.tbl_feedback.Add(feedback);
            _context.SaveChanges();
            return RedirectToAction("Feedback");
        }
    }

}

