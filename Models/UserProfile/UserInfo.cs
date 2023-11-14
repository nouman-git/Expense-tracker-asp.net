using ExpenseTrack.Areas.Identity.Data;

namespace ExpenseTrack.Models.UserProfile
{
    public class UserInfo
    {
        public int UserInfoId { get; set; }
        public string UserProfilePicture { get; set; }
        public decimal Income { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
