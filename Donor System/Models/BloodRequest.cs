using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace Donor_System.Models
{
    public class BloodRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string PatientSurname { get; set; }
        public string PatientIDNumber { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSurname { get; set; }
        public string DoctorEmail { get; set; }
        public string BloodType { get; set; }
        public string DonationLocation { get; set; }
        public string AmtRequired { get; set; }
        public string Status { get; set; }
        //public string FileName { get; set; }
        //public byte[] FileContent { get; set; }
        public string ConsentFileName { get; set; }
        public byte[] ConsentFileContent { get; set; }
        public string cancelReason { get; set; }
        public string declineReason { get; set; }

        public bool IsEnoughBood(string bloodType,double amt, string centerEmail)
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    string center = db.Centers.Where(x => x.Email == centerEmail).FirstOrDefault().Name;
                    var matchingRequests = db.BloodRequestRecords
                   .Where(x => x.BloodType == bloodType && x.DonationLocation == centerEmail)
                   .ToList();


                    double sumReq;
                    if (matchingRequests != null)
                    {
                        sumReq = matchingRequests
                       .Select(x => x.amtBloodRequested)
                       .Sum();
                    }
                    else
                    {
                        sumReq = 0;
                    }


                    var matchingDonation = db.BloodDonationRecords
                    .Where(x => x.BloodType == bloodType && x.DonationLocation == center)
                    .ToList();
                    double sumDon;
                    if (matchingDonation != null)
                    {
                        sumDon = matchingDonation
                        .Select(x => x.amtBloodDonated)
                        .Sum();
                    }
                    else
                    {
                        sumDon = 0;
                    }


                    double sumIncNew = sumReq + amt;
                    if (sumDon >= sumIncNew)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }


        }
        }

    }
}