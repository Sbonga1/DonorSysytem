using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class ExternalDrReg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public string declineReason { get; set; }
        public string Status { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}