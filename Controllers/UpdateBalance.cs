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
    public class UpdateBalance : Controller
    {
        private readonly ExpenseTrackContext _dbContext;

        public UpdateBalance(ExpenseTrackContext context)
        {
            _dbContext = context;
        }

        public void UpdateBalances()
        {
            // Your logic to update balances

            // Retrieve users from the database
            var users = _dbContext.UserInfo.ToList();

            foreach (var user in users)
            {
                // Check if today is the user's associated date
                if (DateTime.Now.Day == user.CreditDate)
                {
                    // Update the user's balance based on their income
                    user.Balance += user.Income;

                    // Save changes to the database
                    _dbContext.SaveChanges();
                }
            }
        }

    }
}
