// WishlistController.cs

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrack.Data;
using ExpenseTrack.Models;
using System.Security.Claims;

namespace ExpenseTrack.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ExpenseTrackContext _context;

        public WishlistController(ExpenseTrackContext context)
        {
            _context = context;
        }

        // GET: Wishlist
        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var wishlistItems = _context.WishlistItems
                    .Where(w => w.UserId == userId)
                    .ToList();

                return View("WishlistIndex", wishlistItems);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                return BadRequest("An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public IActionResult DeleteWishlistItem(int wishlistItemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var wishlistItem = _context.WishlistItems.FirstOrDefault(w => w.WishlistItemId == wishlistItemId && w.UserId == userId);

                if (wishlistItem != null)
                {
                    _context.WishlistItems.Remove(wishlistItem);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                return BadRequest("An error occurred while processing your request.");
            }
        }
    }
}
