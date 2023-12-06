﻿using ExpenseTrack.Areas.Identity.Data;
using ExpenseTrack.Data;
using ExpenseTrack.Models.UserProfile;
using Magnum.FileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTrack.Controllers.UserProfile
{
    public class UserProfileController : Controller
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ExpenseTrackContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserProfileController(ILogger<UserProfileController> logger, UserManager<User> userManager, ExpenseTrackContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = _userManager.GetUserId(this.User);
            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

            var model = new UserProfileViewModel
            {
                FirstName = loggedInUser?.firstName,
                LastName = loggedInUser?.lastName,
                Email = loggedInUser?.Email,
                Income = loggedInUser?.Income ?? 0,
                UserProfilePicture = userInfo?.UserProfilePicture // Display profile picture URL
            };

            return View("_UserProfilePartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var loggedInUserId = _userManager.GetUserId(this.User);
            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

            var model = new UserProfileViewModel
            {
                FirstName = loggedInUser?.firstName,
                LastName = loggedInUser?.lastName,
                Email = loggedInUser?.Email,
                Income = loggedInUser?.Income ?? 0,
                UserProfilePicture = userInfo?.UserProfilePicture // Display profile picture URL
            };

            return PartialView("_UserProfilePartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var loggedInUserId = _userManager.GetUserId(this.User);
            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

            string fileName = null;

            if (model.RemovePicture == "true")
            {
                // Remove the current picture from the database
                if (userInfo != null)
                {
                    // Update the UserProfilePicture to default
                    userInfo.UserProfilePicture = "/images/profile/default.jpg";
                    _context.UserInfo.Update(userInfo);
                    await _context.SaveChangesAsync();
                }

                // Update the UserProfilePicture in the model for display
                model.UserProfilePicture = "/images/profile/default.jpg";
            }

            if (model.PictureFile != null)
            {
                // Check if the uploaded file is a valid image file
                if (!IsImageFile(model.PictureFile))
                {
                    // If it's not a valid image file, add a model state error
                    ModelState.AddModelError("PictureFile", "Please upload a valid jpg, png, or jpeg image file.");

                    // Set UserProfilePicture to the current value so it's not overridden
                    model.UserProfilePicture = userInfo?.UserProfilePicture;

                    // Handle the validation error, redirect to the same view
                    return View("_UserProfilePartial", model);
                }

                // Get the root path of the wwwroot folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Generate a unique filename for the uploaded image
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.PictureFile.FileName);

                // Combine the root path with the profile picture folder path
                string profilePicturePath = Path.Combine(wwwRootPath, @"images\profile");

                // If the user already has a profile picture, delete the old image file
                if (!string.IsNullOrEmpty(userInfo?.UserProfilePicture))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, userInfo.UserProfilePicture.TrimStart('\\'));

                    // Check if the old image file exists before attempting to delete
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save the uploaded image to the profile picture folder with the new filename
                using (var fileStream = new FileStream(Path.Combine(profilePicturePath, fileName), FileMode.Create))
                {
                    await model.PictureFile.CopyToAsync(fileStream);
                }
            }


            if (loggedInUser != null)
            {
                loggedInUser.firstName = model.FirstName;
                loggedInUser.lastName = model.LastName;
                loggedInUser.Income = model.Income;
                await _userManager.UpdateAsync(loggedInUser);
            }

            if (userInfo == null)
            {
                var newUserInfo = new UserInfo
                {
                    UserId = loggedInUserId,
                    UserProfilePicture = fileName != null ? @"\images\profile\" + fileName : @"\images\profile\default.jpg" // Default value when PictureFile is null
                                                                                                                           
                };
                _context.UserInfo.Add(newUserInfo);
            }
            else
            {
                if (model.Income != 0)
                {
                    userInfo.Income = model.Income;
                }

                if (model.PictureFile != null)
                {
                    userInfo.UserProfilePicture = @"\images\profile\" + fileName;
                }
                else if (userInfo.UserProfilePicture == null) // Handle the case where UserProfilePicture is null in the database
                {
                    userInfo.UserProfilePicture = @"\images\profile\default.jpg";
                }

                _context.UserInfo.Update(userInfo);
            }

            await _context.SaveChangesAsync();

            // Redirect to the home screen
            return RedirectToAction("Index", "Home");
        }

        private bool IsImageFile(IFormFile file)
        {
            // To check if the file content type to determine if it's a jpg, png, or jpeg image
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
            return allowedContentTypes.Contains(file.ContentType);
        }
    }
}











//using ExpenseTrack.Areas.Identity.Data;
//using ExpenseTrack.Data;
//using ExpenseTrack.Models.UserProfile;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ExpenseTrack.Controllers.UserProfile
//{
//    public class UserProfileController : Controller
//    {
//        private readonly ILogger<UserProfileController> _logger;
//        private readonly UserManager<User> _userManager;
//        private readonly ExpenseTrackContext _context;

//        public UserProfileController(ILogger<UserProfileController> logger, UserManager<User> userManager, ExpenseTrackContext context)
//        {
//            _logger = logger;
//            _userManager = userManager;
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            // Get the logged-in user ID
//            var loggedInUserId = _userManager.GetUserId(this.User);

//            // Retrieve user info for the logged-in user
//            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);

//            // Get the user details using UserManager
//            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

//            // Prepare data for the view
//            var model = new UserProfileViewModel
//            {
//                FirstName = loggedInUser.firstName,
//                LastName = loggedInUser.lastName,
//                Email = loggedInUser.Email,
//                Income = userInfo?.Income ?? 0,
//                //   PictureFile = userInfo.UserProfilePicture
//            };

//            return View("_UserProfilePartial", model);
//        }
//        [HttpGet]
//        public async Task<IActionResult> UpdateProfile()
//        {
//            var loggedInUserId = _userManager.GetUserId(this.User);
//            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);
//            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

//            var model = new UserProfileViewModel
//            {
//                FirstName = loggedInUser.firstName,
//                LastName = loggedInUser.lastName,
//                Email = loggedInUser.Email,
//                Income = userInfo?.Income ?? 0,
//                // Add other properties as needed
//            };

//            return PartialView("_UserProfilePartial", model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
//        {

//            var loggedInUserId = _userManager.GetUserId(this.User);
//            var userInfo = _context.UserInfo.FirstOrDefault(u => u.UserId == loggedInUserId);
//            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

//            if (loggedInUser != null)
//            {
//                loggedInUser.firstName = model.FirstName;
//                loggedInUser.lastName = model.LastName;
//                await _userManager.UpdateAsync(loggedInUser);
//            }

//            if (userInfo == null)
//            {
//                var newUserInfo = new UserInfo
//                {
//                    UserId = loggedInUserId,
//                    Income = model.Income,
//                    UserProfilePicture = "abc"
//                    // Add other properties as needed
//                };
//                _context.UserInfo.Add(newUserInfo);
//            }
//            else
//            {
//                if (model.Income != 0)
//                {
//                    userInfo.Income = model.Income;
//                    userInfo.UserProfilePicture = "abc";
//                    // Update other properties as needed
//                }

//                _context.UserInfo.Update(userInfo);
//            }

//            await _context.SaveChangesAsync();

//            // Redirect to the home screen
//            return RedirectToAction("Index", "Home");
//        }
//    }
//}