using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [NewEmail]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be longer than 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Weddings created by this user
        public List<Wedding> Weddings { get; set; }
        // RSVPs to weddings by this user
        public List<Rsvp> Rsvps { get; set; }
    }

    public class NewEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            WeddingPlannerContext db = (WeddingPlannerContext)validationContext.GetService(typeof(WeddingPlannerContext));

            User user = db.Users.FirstOrDefault(u => u.Email == (string)value);
            
            if (user != null)
            {
                return new ValidationResult("Email already taken");
            }

            return ValidationResult.Success;
        }
    }
}