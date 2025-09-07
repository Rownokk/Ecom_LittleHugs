using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Mvc;

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

            var admin = _context.tbl_admin
                        .FirstOrDefault(a => a.admin_id == int.Parse(adminId));

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
        public IActionResult ChangeProfileImage(IFormFile admin_image, int admin_id)
        {
            if (admin_image != null && admin_image.Length > 0)
            {
                var existingAdmin = _context.tbl_admin.FirstOrDefault(a => a.admin_id == admin_id);

                if (existingAdmin != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "admin_image");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Path.GetFileName(admin_image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        admin_image.CopyTo(stream);
                    }

                    existingAdmin.admin_image = uniqueFileName;

                    _context.tbl_admin.Update(existingAdmin);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Profile");
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
        public IActionResult updateCustomer(Customer customer, IFormFile customer_image)
        {
            string ImagePath = Path.Combine(_env.WebRootPath, "customer_images",
                customer_image.FileName);
            FileStream fs = new FileStream(ImagePath, FileMode.Create);
            customer_image.CopyTo(fs);
            customer.customer_image = customer_image.FileName;
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
            _context.tbl_customer.Remove(customer);
            _context.SaveChanges();
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
            return View();
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
            return View();
        }

        public IActionResult deletePermissionCategory(int id)
        {
            return View(_context.tbl_category.FirstOrDefault(c => c.category_id == id));

        }

        public IActionResult deleteCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            _context.tbl_category.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }

    }
}
