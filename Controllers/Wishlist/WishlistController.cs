using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrack.Controllers.Wishlist
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
