using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class BloodRequestRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Blood Type")]
        public string BloodType { get; set; }
        public int DrId { get; set; }
        [DisplayName("Amount of Blood Requested (l)")]
        public double amtBloodRequested { get; set; }
        [DisplayName("Request Date")]
        public DateTime RequestDate { get; set; }
        [DisplayName("Donation Location")]
        public string DonationLocation { get; set; }
    }
}