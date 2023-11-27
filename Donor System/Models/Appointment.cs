using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Center { get; set; }
        public string status { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        [MaxLength(10)]
        [MinLength(10)]
        public string PhoneNumber { get; set; }
        public string Reason { get; set; }
        public string drEmail { get; set; }
        [MaxLength(13)]
        [MinLength(13)]
        public string IdNumber { get; set; }
        public string declineReason { get; set; }
        public string cancelReason { get; set; }

        public string GetYearFromSAID(string idNumber)
        {
            // Extract the YYMMDD portion from the ID number
            string datePart = idNumber.Substring(0, 6);

            // Extract the year from YYMMDD
            int year = int.Parse(datePart.Substring(0, 2)); // Extract the first two digits for the year
            int currentYear = DateTime.Now.Year; // Get the current year
            int century = (year >= currentYear % 100) ? 19 : 20; // Determine the century based on current year

            // Combine the century and year to get the full birth year
            int fullYear = century * 100 + year;

            return fullYear.ToString();
        }
    }
}