using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace WeddingPlanner.Models
{
    public class Rsvp
    {
        [Key]
        public int RsvpId { get; set; }

        public int WeddingId { get; set; }
        public Wedding WeddingRsvp { get; set; }
        public int UserId { get; set; }
        public User UserRsvp { get; set; }
    }
}