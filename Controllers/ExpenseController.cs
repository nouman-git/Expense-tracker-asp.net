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




        public IActionResult AddPage()
		{
            var categories = new List<string> { "Category1", "Category2", "Category3" }; // Add your hardcoded categories
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

                _context.Expenses.Add(expense);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                var categories = new List<string> { "Category1", "Category2", "Category3" }; // Add your hardcoded categories
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
                return View("Add", expense);
            }
        }



        public IActionResult EditPage(int ExpenseID)
		{
            var expense = _context.Expenses.FirstOrDefault(e => e.ExpenseID == ExpenseID);

            if (expense == null)
            {
                return NotFound();
            }

            var categories = new List<string> { "Category1", "Category2", "Category3" }; // Add your hardcoded categories
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

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
