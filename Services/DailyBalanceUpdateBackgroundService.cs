// DailyBalanceUpdateBackgroundService.cs
using System;
using System.Threading;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class DailyBalanceUpdateBackgroundService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private System.Timers.Timer _timer;

    public DailyBalanceUpdateBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("DailyBalanceUpdateBackgroundService started.");

        using (var scope = _serviceProvider.CreateScope())
        {
            var dailyBalanceUpdateService = scope.ServiceProvider.GetRequiredService<DailyBalanceUpdateService>();

            // Calculate the time until the next occurrence of the desired time
            var now = DateTime.Now;
            var desiredTime = new DateTime(now.Year, now.Month, now.Day, 7, 42, 0); // Example time: 7:27 AM
            var initialDelay = desiredTime > now ? desiredTime - now : desiredTime.AddDays(1) - now;

            Console.WriteLine($"Initial Delay: {initialDelay}");

            // Set up the timer to run the DoWork method at the specified time every day
            _timer = new System.Timers.Timer(initialDelay.TotalMilliseconds);
            _timer.Elapsed += (sender, e) =>
            {
                DoWork(null);
                _timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            };
            _timer.Start();
        }

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        Console.WriteLine("DoWork triggered at " + DateTime.Now);

        using (var scope = _serviceProvider.CreateScope())
        {
            var dailyBalanceUpdateService = scope.ServiceProvider.GetRequiredService<DailyBalanceUpdateService>();

            // Replace the following line with the actual logic to update user balances
            dailyBalanceUpdateService.UpdateUserBalances();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("DailyBalanceUpdateBackgroundService stopped.");

        _timer?.Stop();
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Console.WriteLine("DailyBalanceUpdateBackgroundService disposed.");

        _timer?.Dispose();
    }
}
