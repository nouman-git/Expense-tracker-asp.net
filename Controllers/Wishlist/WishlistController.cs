using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTrack.Data;
using ExpenseTrack.Models;
using System.Security.Claims;

public class WishlistController : Controller
{
    private readonly ExpenseTrackContext _context;

    public WishlistController(ExpenseTrackContext context)
    {
        _context = context;
    }

    public IActionResult Index(DateTime? filterDate, int page = 1, int pageSize = 5)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var wishlistItemsQuery = _context.WishlistItems
            .Where(w => w.UserId == userId);

        if (filterDate.HasValue)
        {
            wishlistItemsQuery = wishlistItemsQuery
                .Where(w => w.Date.Date == filterDate.Value.Date);
        }
        var userBalance = GetUserBalance(userId);
        ViewBag.UserBalance = userBalance;


        var totalWishlistItems = wishlistItemsQuery.Count();
        var wishlistItems = wishlistItemsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var paginationModel = new PaginationModel<WishlistItem>
        {
            Items = wishlistItems,
            TotalItems = totalWishlistItems,
            CurrentPage = page,
            PageSize = pageSize
        };

        return View("WishlistIndex", paginationModel);
    }



    public IActionResult AddPage()

    {
        var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Health and Wellbeing", "Debt Payments", "Other" }; // Add your hardcoded categories
        ViewBag.Categories = categories.Select(c => new SelectListItem
        {
            Value = c,
            Text = c
        }).ToList();
        return View("WishlistAdd");
    }

    [HttpPost]
    public IActionResult AddWishlistItem(WishlistItem wishlistItem)
    {
        try
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                wishlistItem.UserId = userId;

                // Add the wishlist item
                _context.WishlistItems.Add(wishlistItem);
                _context.SaveChanges();

                return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
            var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Health and Wellbeing", "Debt Payments", "Other" }; // Add your hardcoded categories
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();
            return View("WishlistAdd", wishlistItem);
        }
    }

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

                if (user != null && wishlistItem.Amount <= user.Balance)
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

                    return RedirectToAction("Index");
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as appropriate for your application
            return BadRequest("An error occurred while processing your request.");
        }

        // Redirect back to the wishlist if the operation is not successful
        return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as appropriate for your application
            return BadRequest("An error occurred while processing your request.");
        }
    }

 
    private decimal GetUserBalance(string userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        return user?.Balance ?? 0;
    }
}
