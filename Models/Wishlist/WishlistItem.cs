// WishlistItem.cs

using ExpenseTrack.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrack.Models
{
    public class WishlistItem
    {
        [Key]
        public int WishlistItemId { get; set; }

        [Required(ErrorMessage = "Expense Name is required.")]
        public string ExpenseName { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } // Foreign Key for User
        public User User { get; set; }
    }
}
