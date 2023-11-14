using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int CategoryID { get; set; } // Primary Key

    [Required]
    public string CategoryName { get; set; } 
}
