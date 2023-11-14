using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrack.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseID { get; set; } // Primary Key

        [Required]
        public string ExpenseName { get; set; } 

        [Required]
        public decimal Amount { get; set; } 

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; } 

        [ForeignKey("Category")]
        public int CategoryID { get; set; } //Foreign Key

        public Category Category { get; set; } 
    }
}
