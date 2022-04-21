using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        [Display(Name = "Wedder One")]
        public string Wedder1Name { get; set; }

        [Required]
        [Display(Name = "Wedder Two")]
        public string Wedder2Name { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Wedding date must be in the future")]
        [DataType(DataType.Date)]
        [Display(Name = "Wedding Date")]
        public DateTime Date { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string WeddingName { get => Wedder1Name + " & " + Wedder2Name; }

        public int UserId { get; set; }
        public User WeddingCreator { get; set; }
        public List<Rsvp> Rsvps { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (DateTime.Now.CompareTo((DateTime)value) > 0)
            {
                return new ValidationResult("Date must be in the future");
            }

            return ValidationResult.Success;
        }
    }
}