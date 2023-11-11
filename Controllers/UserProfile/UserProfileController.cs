using ExpenseTrack.Areas.Identity.Data;
using ExpenseTrack.Data;
using ExpenseTrack.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExpenseTrack.Controllers.UserProfile
{
    public class UserProfileController : Controller
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ExpenseTrackContext _context;

        public UserProfileController(ILogger<UserProfileController> logger, UserManager<User> userManager, ExpenseTrackContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // Get the logged-in user ID
            var loggedInUserId = _userManager.GetUserId(this.User);

            // Retrieve user info for the logged-in user
            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);

           
                // Get the user details using UserManager
                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

                // Prepare data for the view
                var model = new UserProfileViewModel
                {
                    Name = $"{loggedInUser.firstName} {loggedInUser.lastName}",
                    Email = loggedInUser.Email
                    //Income = userInfo.Income,
                    //Picture = userInfo.UserProfilePicture
                };

                return View("~/Views/UserProfile/UserProfileView.cshtml", model);
            

          //  return View("~/Views/UserProfile/UserProfileView.cshtml");
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            // Get the logged-in user
            var loggedInUser = await _userManager.GetUserAsync(User);

            // Update the user's income
            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUser.Id);
            if (userInfo != null)
            {
                userInfo.Income = model.Income;
                _context.SaveChanges();
            }

            // Handle picture upload here

            return RedirectToAction("Index");
        }
    }
}
