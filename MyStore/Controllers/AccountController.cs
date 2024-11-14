using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModels;
using System.Web.Security;

namespace MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên người dùng đã tồn tại chưa
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên người dùng đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp nhau không
                if (model.Password != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Mật khẩu và xác nhận mật khẩu không khớp.");
                    return View(model);
                }

                // Tạo bản ghi User mà không mã hóa mật khẩu
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password, // Lưu mật khẩu dưới dạng văn bản thuần
                    UserRole = "C"  // Bạn có thể thay đổi role nếu cần
                };
                db.Users.Add(user);

                // Tạo bản ghi Customer
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username
                };
                db.Customers.Add(customer);

                // Lưu thông tin vào CSDL
                db.SaveChanges();

                // Sau khi đăng ký thành công, chuyển đến trang đăng nhập
                TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }
        // GET: Account/Login

        public ActionResult Login()

        {
            return View();

        }

        // POST: Account/Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            // Tìm người dùng theo tên đăng nhập
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username
                                    && u.Password == model.Password
                                    && u.UserRole == "C");
                if (user != null)
                {
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(" ", "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            return View(model);
        }
        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // Trang thông tin người dùng (cần đăng nhập)
        [Authorize]
        public ActionResult ProfileInfo()
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterVM
            {
                Username = user.Username,
                CustomerName = customer.CustomerName,
                CustomerEmail = customer.CustomerEmail,
                CustomerPhone = customer.CustomerPhone,
                CustomerAddress = customer.CustomerAddress
            };

            return View(model);
        }

        // Thay đổi mật khẩu (cần đăng nhập)
        [Authorize(Roles = "C")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user != null)
                {
                    // Kiểm tra mật khẩu hiện tại không mã hóa
                    if (user.Password != currentPassword)
                    {
                        ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không đúng.");
                        return View();
                    }

                    // Kiểm tra mật khẩu mới và xác nhận mật khẩu có khớp không
                    if (newPassword != confirmPassword)
                    {
                        ModelState.AddModelError("confirmPassword", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                        return View();
                    }

                    // Cập nhật mật khẩu mới
                    user.Password = newPassword; // Lưu mật khẩu mới dưới dạng văn bản thuần
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
                    return RedirectToAction("ProfileInfo");
                }
            }

            return View();
        }

    }
}