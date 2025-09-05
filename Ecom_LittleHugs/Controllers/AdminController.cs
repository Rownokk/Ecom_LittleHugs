using Ecom_LittleHugs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_LittleHugs.Controllers
{
    public class AdminController : Controller
    {
        private readonly myContext _context;

        public AdminController(myContext context)
        {
            _context = context;
        }

        // GET: Admin Dashboard
        public IActionResult Index()
        {
            string admin_session = HttpContext.Session.GetString("admin_session");
            if (!string.IsNullOrEmpty(admin_session))
                return View();
            else
                return RedirectToAction("Login");
        }

        // GET: Admin Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin Login
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

        // POST: Profile Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Admin admin)
        {
            if (!ModelState.IsValid)
                return View(admin);

            // Optionally, you can check if password is empty and not update it
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
    }
}
