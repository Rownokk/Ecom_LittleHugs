using System;
using System.IO;
using System.Linq;
using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecom_LittleHugs.Controllers
{
    public class AdminController : Controller
    {
        private readonly myContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(myContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Dashboard
        public IActionResult Index()
        {
            string admin_session = HttpContext.Session.GetString("admin_session");
            if (!string.IsNullOrEmpty(admin_session))
                return View();
            else
                return RedirectToAction("Login");
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string adminEmail, string adminPassword)
        {
            var admin = _context.tbl_admin
                        .FirstOrDefault(a => a.admin_email == adminEmail && a.admin_password == adminPassword);

            if (admin != null)
            {
                HttpContext.Session.SetString("admin_session", admin.admin_id.ToString());
                return RedirectToAction("Index");
            }

            ViewBag.message = "Incorrect Username or Password";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_session");
            return RedirectToAction("Login");
        }

        // GET: Profile
        [HttpGet]
        public IActionResult Profile()
        {
            var adminId = HttpContext.Session.GetString("admin_session");
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login");

            if (!int.TryParse(adminId, out var id))
                return RedirectToAction("Login");

            var admin = _context.tbl_admin.FirstOrDefault(a => a.admin_id == id);
            if (admin == null)
                return RedirectToAction("Login");

            return View(admin);
        }

        // POST: Profile Update (Name, Email, Password only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Admin admin)
        {
            if (!ModelState.IsValid)
                return View(admin);

            var existingAdmin = _context.tbl_admin.FirstOrDefault(a => a.admin_id == admin.admin_id);
            if (existingAdmin != null)
            {
                existingAdmin.admin_name = admin.admin_name;
                existingAdmin.admin_email = admin.admin_email;

                if (!string.IsNullOrEmpty(admin.admin_password))
                    existingAdmin.admin_password = admin.admin_password;

                _context.tbl_admin.Update(existingAdmin);
                _context.SaveChanges();
            }

            TempData["success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        // POST: Change Profile Image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeProfileImage(int admin_id, IFormFile? admin_image)
        {
            if (admin_image == null || admin_image.Length == 0)
                return RedirectToAction("Profile", new { id = admin_id });

            var existingAdmin = _context.tbl_admin.FirstOrDefault(a => a.admin_id == admin_id);
            if (existingAdmin == null) return NotFound();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "admin_image");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(admin_image.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                admin_image.CopyTo(stream);
            }

            // Delete old file if exists
            if (!string.IsNullOrWhiteSpace(existingAdmin.admin_image))
            {
                var oldPath = Path.Combine(uploadsFolder, existingAdmin.admin_image);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            existingAdmin.admin_image = uniqueFileName;
            _context.tbl_admin.Update(existingAdmin);
            _context.SaveChanges();

            return RedirectToAction("Profile", new { id = admin_id });
        }

        public IActionResult fetchCustomer()
        {
            return View(_context.tbl_customer.ToList());
        }

        public IActionResult customerDetails(int id)
        {
            return View(_context.tbl_customer.FirstOrDefault(c => c.customer_id == id));
        }

        public IActionResult updateCustomer(int id)
        {
            return View(_context.tbl_customer.Find(id));
        }

        [HttpPost]
        public IActionResult updateCustomer(Customer customer, IFormFile? customer_image)
        {
            if (customer_image != null && customer_image.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "customer_images");
                Directory.CreateDirectory(folder);

                var imagePath = Path.Combine(folder, customer_image.FileName);
                using (var fs = new FileStream(imagePath, FileMode.Create))
                {
                    customer_image.CopyTo(fs);
                }
                customer.customer_image = customer_image.FileName;
            }

            _context.tbl_customer.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");
        }

        public IActionResult deletePermission(int id)
        {
            return View(_context.tbl_customer.FirstOrDefault(c => c.customer_id == id));
        }

        public IActionResult deleteCustomer(int id)
        {
            var customer = _context.tbl_customer.Find(id);
            if (customer != null)
            {
                _context.tbl_customer.Remove(customer);
                _context.SaveChanges();
            }
            return RedirectToAction("fetchCustomer");
        }

        public IActionResult fetchCategory()
        {
            return View(_context.tbl_category.ToList());
        }

        public IActionResult addCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addCategory(Category cat)
        {
            _context.tbl_category.Add(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }

        public IActionResult updateCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult updateCategory(Category cat)
        {
            _context.tbl_category.Update(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }

        public IActionResult deletePermissionCategory(int id)
        {
            return View(_context.tbl_category.FirstOrDefault(c => c.category_id == id));
        }

        public IActionResult deleteCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            if (category != null)
            {
                _context.tbl_category.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("fetchCategory");
        }

        public IActionResult fetchProduct()
        {
            return View(_context.tbl_product.ToList());
        }

        public IActionResult addProduct()
        {
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;
            return View();
        }

        [HttpPost]
        public IActionResult addProduct(Product prod, IFormFile? product_image)
        {
            if (product_image != null && product_image.Length > 0)
            {
                string imageName = Path.GetFileName(product_image.FileName);
                string imagePath = Path.Combine(_env.WebRootPath, "product_images", imageName);
                using (var fs = new FileStream(imagePath, FileMode.Create))
                {
                    product_image.CopyTo(fs);
                }
                prod.product_image = imageName;
            }

            _context.tbl_product.Add(prod);
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }

        public IActionResult productDetails(int id)
        {
            return View(_context.tbl_product.Include(c => c.Category).FirstOrDefault(c => c.product_id == id));
        }

        public IActionResult deletePermissionProduct(int id)
        {
            return View(_context.tbl_product.FirstOrDefault(p => p.product_id == id));
        }

        public IActionResult deleteProduct(int id)
        {
            var product = _context.tbl_product.Find(id);
            if (product != null)
            {
                _context.tbl_product.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("fetchProduct");
        }

        public IActionResult updateProduct(int id)
        {
            var product = _context.tbl_product.Find(id);
            if (product == null) return NotFound();

            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;
            ViewBag.selectedCategoryId = product.cat_id; // This is important
            return View(product);
        }

        [HttpPost]
        public IActionResult updateProduct(Product product)
        {
            _context.tbl_product.Update(product);
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }

        [HttpPost]
        public IActionResult ChangeProductImage(IFormFile? product_image, int product_id)
        {
            if (product_image != null && product_image.Length > 0)
            {
                var existingProduct = _context.tbl_product.FirstOrDefault(p => p.product_id == product_id);

                if (existingProduct != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "product_images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Path.GetFileName(product_image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        product_image.CopyTo(stream);
                    }

                    existingProduct.product_image = uniqueFileName;

                    _context.tbl_product.Update(existingProduct);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("fetchProduct");
        }
        public IActionResult fetchFeedback()
        {
            var feedbackList = _context.tbl_feedback.ToList();
            return View(feedbackList);
        }

        // GET: Admin/DeleteFeedbackConfirmation/5
        public IActionResult DeleteFeedbackConfirmation(int id)
        {
            var feedback = _context.tbl_feedback.FirstOrDefault(f => f.feedback_id == id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: Admin/DeleteFeedbackConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFeedbackConfirmed(int feedback_id)
        {
            var feedback = _context.tbl_feedback.FirstOrDefault(f => f.feedback_id == feedback_id);
            if (feedback != null)
            {
                _context.tbl_feedback.Remove(feedback);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Feedback deleted successfully!";
            }
            return RedirectToAction("fetchFeedback");
        }

        // KEEP YOUR EXISTING deletePermissionFeedback AND deleteFeedback METHODS:
        public IActionResult deletePermissionFeedback(int id)
        {
            return View(_context.tbl_feedback.FirstOrDefault(f => f.feedback_id == id));
        }

        public IActionResult deleteFeedback(int id)
        {
            var feedback = _context.tbl_feedback.Find(id);
            if (feedback != null)
            {
                _context.tbl_feedback.Remove(feedback);
                _context.SaveChanges();
            }
            return RedirectToAction("fetchFeedback");
        }
    }
}