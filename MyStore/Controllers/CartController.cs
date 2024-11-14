using System.Linq;
using System.Web.Mvc;
using MyStore.Models.ViewModels;
using MyStore.Models;

namespace MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Hàm lấy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(product.ProductID, product.ProductImage, product.ProductName, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }


        // Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        // Làm trống giỏ hàng
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng trong giỏ hàng và trả về JSON để cập nhật lại giao diện
        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            var cart = cartService.GetCart();

            var item = cart.Items.FirstOrDefault(i => i.ProductID == id);
            if (item != null && quantity > 0)
            {
                // Cập nhật số lượng sản phẩm trong giỏ hàng
                cart.UpdateQuantity(id, quantity);

                // Tính lại tổng tiền của sản phẩm và giỏ hàng
                decimal newTotalPrice = item.Quantity * item.UnitPrice;
                decimal totalValue = cart.Items.Sum(i => i.TotalPrice);

                // Trả về kết quả JSON
                return Json(new
                {
                    success = true,
                    newTotalPrice = newTotalPrice.ToString("N0"),
                    newTotalValue = totalValue.ToString("N0")
                });
            }

            return Json(new { success = false, message = "Số lượng không hợp lệ" });
        }
    }
}