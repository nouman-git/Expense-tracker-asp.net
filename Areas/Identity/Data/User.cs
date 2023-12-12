using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTrack.Models;
using ExpenseTrack.Models.UserProfile;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrack.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public UserInfo UserInfo { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public decimal Income { get; set; }
    public decimal Balance { get; set; }
    public ICollection<Expense> Expenses { get; set; }
}

