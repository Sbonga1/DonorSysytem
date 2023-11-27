using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class BloodAvailability
    {
        public string BloodType { get; set; }
        public double AvailableAmount { get; set; }
        public double DonatedAmount { get; set; }
        public double RequestedAmount { get; set; }
    }

}