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
                    FirstName = loggedInUser.firstName,
                    LastName=loggedInUser.lastName,
                    Email = loggedInUser.Email,
                    Income = userInfo?.Income ?? 0,
                    //   PictureFile = userInfo.UserProfilePicture
                };

                return View("Index", model);
            
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var loggedInUserId = _userManager.GetUserId(this.User);
            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

            if (loggedInUser != null)
            {
                loggedInUser.firstName = model.FirstName;
                loggedInUser.lastName = model.LastName;
                await _userManager.UpdateAsync(loggedInUser);
            }
            if (userInfo == null)
            {
                // Create a new UserInfo record for if not exists
                var newUserInfo = new UserInfo
                {
                    UserId = loggedInUserId,
                    Income = model.Income,
                   // UserProfilePicture = "abc"

                };
                _context.UserInfo.Add(newUserInfo);
            }
            else
            {
                // Update the existing UserInfo record
                if (model.Income != 0)
                {
                    userInfo.Income = model.Income;
                  //  userInfo.UserProfilePicture = "abc";

                }
                if (model.PictureFile != null)
                {
                    var imagePath = Path.Combine("images", "profile", model.PictureFile.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.PictureFile.CopyToAsync(stream);
                    }
                    userInfo.UserProfilePicture = imagePath; // Save the image path
                }
                _context.UserInfo.Update(userInfo);
            }


            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
