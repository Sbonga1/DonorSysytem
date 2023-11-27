using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class DrAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSurname { get; set; }
        public string DonorName { get; set; }
        public string DonorSurname { get; set; }
        public string DonorEmail { get; set; }
        public string Center { get; set; }
        public string status { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int appId { get; set; }
        public string drEmail { get; set; }
    }
}