﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrack.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseID { get; set; } // Primary Key

        [Required(ErrorMessage = "Expense Name is required.")]
        public string ExpenseName { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; } // Foreign Key

        public Category Category { get; set; }
    }
}
