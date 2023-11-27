using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class BloodDonationScreening
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Personal Information
        public string FullName { get; set; }
       
        public string DateOfBirth { get; set; }
        public string IdNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }




        // Lifestyle and Risk Assessment
        public string RecentTattooOrPiercing { get; set; }
        public string IsPregnant { get; set; }
        public string InfectiousDiseaseExposure { get; set; }
        public string MalariaRiskAssessment { get; set; }
        public string RecentCOVID19DiagnosisOrExposure { get; set; }

        // Health Assessment
        public double BloodPressure { get; set; }
        public double PulseRate { get; set; }
        public double HemoglobinOrHematocrit { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }

        // Donor Consent
        public string ConsentToDonateBlood { get; set; }
        public string ConsentToShareHealthInfo { get; set; }

        // Signature and Date
        //public string DonorSignature { get; set; }
       
        public string DonationDate { get; set; }
        public string status { get; set; }

        public DateTime GetDOBFromSAID(string idNumber)
        {
            // Extract the YYMMDD portion from the ID number
            string datePart = idNumber.Substring(0, 6);

            // Extract the year, month, and day from YYMMDD
            int year = int.Parse(datePart.Substring(0, 2)); // Extract the first two digits for the year
            int month = int.Parse(datePart.Substring(2, 2)); // Extract the next two digits for the month
            int day = int.Parse(datePart.Substring(4, 2)); // Extract the last two digits for the day

            // Determine the century based on current year
            int currentYear = DateTime.Now.Year;
            int century = (year >= currentYear % 100) ? 19 : 20;

            // Combine the century, year, month, and day to get the full Date of Birth
            int fullYear = century * 100 + year;
            DateTime dob = new DateTime(fullYear, month, day);

            return dob;
        }




    }
}