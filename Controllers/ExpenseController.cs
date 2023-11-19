using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrack.Data;
using ExpenseTrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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
            // Get the ID of the currently logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Filter expenses based on the user ID
            var expenses = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .ToList();

            if (filterDate.HasValue)
            {
                expenses = expenses.Where(e => e.Date.Date == filterDate.Value.Date).ToList();
            }

            return View(expenses);
        }

        public IActionResult AddPage()
		{
			var categories = _context.Categories.Select(c => new SelectListItem
			{
				Value = c.CategoryID.ToString(),
			    Text = c.CategoryName
		 }).ToList();

			ViewBag.Categories = categories;

			return View("Add");
		}

        [HttpPost]
        public IActionResult AddExpense(Expense expense)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                expense.UserId = userId;

                _context.Expenses.Add(expense);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View("Add", expense);
            }
        }



        public IActionResult EditPage(int ExpenseID)
		{
			// Retrieve the expense details from the database based on ExpenseID
			var expense = _context.Expenses.FirstOrDefault(e => e.ExpenseID == ExpenseID);

			if (expense == null)
			{
				return NotFound(); 
			}

			// Populate the ViewBag.Categories as in your "AddPage" action
			var categories = _context.Categories.Select(c => new SelectListItem
			{
				Value = c.CategoryID.ToString(),
				Text = c.CategoryName
			}).ToList();

			ViewBag.Categories = categories;

			return View("Edit", expense);
		}


		[HttpPost]
        public IActionResult EditExpense(Expense expense)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            expense.UserId = userId;

            // Update the expense in the database
            _context.Expenses.Update(expense);
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



    }
}
