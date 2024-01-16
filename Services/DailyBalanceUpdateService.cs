// DailyBalanceUpdateService.cs
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using ExpenseTrack.Data;
using ExpenseTrack.Areas.Identity.Data;

public class DailyBalanceUpdateService
{
    private readonly IServiceProvider _serviceProvider;

    public DailyBalanceUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void UpdateUserBalances()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseTrackContext>();

            // Get users whose CreditDate is equal to the current date
            var usersToUpdate = dbContext.Users
                .Where(u => u.CreditDate == DateTime.Now.Day)
                .ToList();

            // Update user balances
            foreach (var user in usersToUpdate)
            {
                user.Balance += user.Income;
            }

            // Save changes to the database
            dbContext.SaveChanges();
        }
    }
}
