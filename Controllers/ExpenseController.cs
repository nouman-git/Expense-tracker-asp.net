﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrack.Data;
using ExpenseTrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Index()
        {
            var expenses = _context.Expenses.Include(e => e.Category).ToList();
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
		
			_context.Expenses.Add(expense);
			_context.SaveChanges();
			return RedirectToAction("Index"); 
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
			
				// Update the expense in the database
				_context.Expenses.Update(expense);
				_context.SaveChanges();
				return RedirectToAction("Index"); 
		}


		[HttpPost]
		public IActionResult DeleteExpense(int ExpenseID)
		{
			var expense = _context.Expenses.Find(ExpenseID);

			_context.Expenses.Remove(expense);
			_context.SaveChanges();


			return RedirectToAction("Index");
		}


	}
}