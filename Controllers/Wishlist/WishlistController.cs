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

                // Get the user balance from your data source (replace this with your actual logic)
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                decimal userBalance = user?.Balance ?? 0;

                ViewBag.UserBalance = userBalance;

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

        // WishlistController.cs

        [HttpPost]
        public IActionResult AddToExpense(int wishlistItemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get the wishlist item
                var wishlistItem = _context.WishlistItems.FirstOrDefault(w => w.WishlistItemId == wishlistItemId && w.UserId == userId);

                if (wishlistItem != null)
                {
                    // Check if the user balance is sufficient
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                    if (user != null && wishlistItem.Amount < user.Balance)
                    {
                        // Deduct the amount from the user's balance
                        user.Balance -= wishlistItem.Amount;
                        _context.SaveChanges();

                        // Add the item to expenses
                        var expense = new Expense
                        {
                            ExpenseName = wishlistItem.ExpenseName,
                            Amount = wishlistItem.Amount,
                            Date = wishlistItem.Date,
                            Description = wishlistItem.Description,
                            Category = wishlistItem.Category,
                            UserId = userId
                        };

                        _context.Expenses.Add(expense);
                        _context.SaveChanges();

                        // Remove the item from the wishlist
                        _context.WishlistItems.Remove(wishlistItem);
                        _context.SaveChanges();

                        return RedirectToAction("Index", "Wishlist");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                return BadRequest("An error occurred while processing your request.");
            }

            return RedirectToAction("Index", "Wishlist");
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
