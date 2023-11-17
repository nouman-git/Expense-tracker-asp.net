﻿namespace ExpenseTrack.Models.UserProfile
{
    public class UserProfileViewModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public decimal Income { get; set; }
       
        public IFormFile PictureFile { get; set; }

    }
}