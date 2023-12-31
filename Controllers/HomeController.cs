using ExpenseTrack.Areas.Identity.Data;
using ExpenseTrack.Data;
using ExpenseTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpenseTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ExpenseTrackContext _context;


        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager , ExpenseTrackContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var fullName = $"{user?.firstName} {user?.lastName}";
            var balance = $"{user?.Balance}";
            ViewData["FullName"] = fullName;
            ViewData["Balance"] = balance;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var numberOfExpensesByCategory = _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();
            var costOfExpensesByCategory = _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(e => e.Amount) })
                .ToList();

            TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone).Date;
            DateTime startDateOfCurrentWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);

            // Number of weeks to include, including the current week
            int numberOfWeeks = 5;

            var expensesLastFourWeeks = _context.Expenses
                .Where(e => e.UserId == userId && e.Date >= startDateOfCurrentWeek.AddDays(-numberOfWeeks * 7))
                .ToList();

            var expensesByWeek = expensesLastFourWeeks
                .GroupBy(e => (int)Math.Floor((e.Date - startDateOfCurrentWeek).TotalDays / 7) + 1)
                .Select(g => new { Week = g.Key, Amount = g.Sum(e => e.Amount) })
                .ToList();



            ViewBag.expensesByWeek = expensesByWeek;
            ViewBag.expensesByWeek = expensesByWeek;
            ViewBag.costOfExpensesByCategory = costOfExpensesByCategory;
            ViewBag.numberOfExpensesByCategory = numberOfExpensesByCategory;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [Authorize] // Ensure only authenticated users can increase balance
        public async Task<IActionResult> IncreaseBalance(decimal amount)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null && amount > 0)
                {
                    if (IsValidAmountFormat(amount))
                    {
                        user.Balance += amount;
                        await _userManager.UpdateAsync(user);
                        Console.WriteLine($"User {user.UserName}'s balance increased by {amount}. New balance: {user.Balance}");
                    }
                }

                return Ok(); // Return a success status code
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error increasing balance: {ex.Message}");
                return BadRequest("Error increasing balance");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsValidAmountFormat(decimal amount)
        {
            // Check if the amount has a valid format (e.g., non-negative, not too large)
            return amount >= 0 && amount < decimal.MaxValue;
        }
    }
}
