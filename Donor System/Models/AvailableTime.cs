using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class AvailableTime
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string BookedBy { get; set; }
        public string Center { get; set; }
        public string Day { get; set; }
        public List<AvailableTime> AvailableTimeSlots { get; set; }
    }
}