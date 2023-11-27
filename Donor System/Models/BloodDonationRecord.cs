using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class BloodDonationRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Donor Email")]
        public string DonorEmail { get; set; }
        [DisplayName("ID Number")]
        public string IdNumber { get; set; }
        [DisplayName("Blood Type")]
        public string BloodType { get; set; }
        public int DrId { get; set; }
        [DisplayName("Amount of Blood Donated (l)")]
        public double amtBloodDonated { get; set; }
        [DisplayName("Donation Date")]
        public DateTime DonationDate { get; set; }
        [DisplayName("Donation Location")]
        public string DonationLocation { get; set; }
        [DisplayName("Donation Frequency")]
        public int DonationFrequency { get; set; }
    }
}