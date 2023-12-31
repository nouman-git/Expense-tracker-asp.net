using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrack.Data;
using ExpenseTrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ExpenseTrack.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrack.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ExpenseTrackContext _context;

        public ExpenseController(ExpenseTrackContext context)
        {
            _context = context;
        }

        // GET: Expense
        public IActionResult Index(DateTime? filterDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expenses = _context.Expenses
            .Where(e => e.UserId == userId)
            .ToList();

            if (filterDate.HasValue)
            {
                expenses = expenses
                    .Where(e => e.Date.Date == filterDate.Value.Date)
                    .ToList();
            }

            return View("Index", expenses);
        }




        public IActionResult AddPage()
        {
            var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Personal Care" , "Debt Payments" , "Other" }; // Add your hardcoded categories
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

            return View("Add");
        }
        [HttpPost]
        public IActionResult AddExpense(Expense expense)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                expense.UserId = userId;

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                // bool addToWishlist = HttpContext.Request.Form["addToWishlist"] == "true";
                //System.Diagnostics.Debug.WriteLine($"addToWishlist: {addToWishlist}");

                if (user != null)
                {
                    if (expense.Amount > user.Balance)
                    {
                        // Expense amount exceeds user's balance
                        ViewBag.ExceedsBalance = true;
                        var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Personal Care", "Debt Payments", "Other" }; // Add your hardcoded categories
                        ViewBag.Categories = categories.Select(c => new SelectListItem
                        {
                            Value = c,
                            Text = c
                        }).ToList();
                        return View("Add", new Expense());
                    }

                    else
                    {
                        // Deduct the amount from the user's balance
                        user.Balance -= expense.Amount;
                        _context.SaveChanges();

                        // Continue with the original logic
                        _context.Expenses.Add(expense);
                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Personal Care", "Debt Payments", "Other" }; // Add your hardcoded categories
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
                return View("Add", expense);
            }
        }
        public IActionResult AddToWishlist(Expense expense)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItem = new WishlistItem
            {
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                Category = expense.Category,
                UserId = userId
            };
            _context.WishlistItems.Add(wishlistItem);
            _context.SaveChanges();
            return RedirectToAction("Index");


        }




        public IActionResult EditPage(int ExpenseID)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.ExpenseID == ExpenseID);

            if (expense == null)
            {
                return NotFound();
            }

            var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Personal Care", "Debt Payments", "Other" }; // Add your hardcoded categories
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

            return View("Edit", expense);
        }


        [HttpPost]
        public IActionResult EditExpense(Expense updatedExpense)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            updatedExpense.UserId = userId;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (updatedExpense.Amount > user.Balance)
            {
                // Expense amount exceeds user's balance
                ViewBag.ExceedsBalance = true;
                var categories = new List<string> { "Housing", "Transportation", "Entertainment", "Personal Care", "Debt Payments", "Other" }; // Add your hardcoded categories
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
                return View("Edit", new Expense());
            }

            // Retrieve the original expense from the database
            var originalExpense = _context.Expenses.AsNoTracking().FirstOrDefault(e => e.ExpenseID == updatedExpense.ExpenseID);

            // Adjust the user's balance
            user.Balance += originalExpense.Amount;
            user.Balance -= updatedExpense.Amount;

            // Update the expense in the database
            _context.Update(updatedExpense);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }




        [HttpPost]
        public IActionResult DeleteExpense(int ExpenseID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expense = _context.Expenses.FirstOrDefault(e => e.ExpenseID == ExpenseID && e.UserId == userId);

            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult ExpensesByCategory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var numberOfExpensesByCategory = _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Category, x => x.Count);

            ViewBag.numberOfExpensesByCategory = numberOfExpensesByCategory;

            return View("Home/Index");
        }

    }
   

}


//public IActionResult ExpensePartialView(DateTime? filterDate)
//{
//    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//    var expenses = _context.Expenses
//        .Include(e => e.Category)
//        .Where(e => e.UserId == userId)
//        .ToList();

//    if (filterDate.HasValue)
//    {
//        expenses = expenses.Where(e => e.Date.Date == filterDate.Value.Date).ToList();
//    }

//    return PartialView("_ExpenseIndexPartial", expenses);
//}


