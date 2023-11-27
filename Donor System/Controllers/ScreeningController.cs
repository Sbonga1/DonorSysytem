using Donor_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Donor_System.Controllers
{
    public class ScreeningController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Screening
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create(int appId, int assignId)
        {
            Session["ScreenassignId"] = assignId.ToString();
            Session["ScreenappId"] = appId.ToString();
            var appointment = db.Appointments.Find(appId);
            BloodDonationScreening Screening = new BloodDonationScreening();
            DateTime dob = Screening.GetDOBFromSAID(appointment.IdNumber);
            BloodDonationScreening b = new BloodDonationScreening()
            {
                FullName = appointment.Name + " " + appointment.Surname,
                PhoneNumber = appointment.PhoneNumber,
                Email = appointment.Email,
                IdNumber = appointment.IdNumber,
                DateOfBirth = dob.ToShortDateString()
            };
            return View(b);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BloodDonationScreening model)
        {
            model.FullName = model.FullName;
            model.DateOfBirth = model.DateOfBirth;
            model.Gender = model.Gender;
            model.Address = model.Address;
            model.PhoneNumber = model.PhoneNumber;
            model.Email = model.Email;
            model.IdNumber = model.IdNumber;
            db.BloodDonationScreenings.Add(model);
            db.SaveChanges();
            Session["ScreenId"] = model.Id.ToString();
            return RedirectToAction("LifestyleRiskAssessment");
        }
        public ActionResult LifestyleRiskAssessment()
        {
            return View();

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LifestyleRiskAssessment(BloodDonationScreening model)
        {
            string assignId = Session["ScreenassignId"] as string;
            int assignid = int.Parse(assignId);
            var assignment = db.DrAssignments.Find(assignid);
            string screenId = Session["ScreenId"] as string;
            int id = int.Parse(screenId);
            var screening = db.BloodDonationScreenings.Find(id);
            screening.RecentTattooOrPiercing = model.RecentTattooOrPiercing;
            screening.IsPregnant = model.IsPregnant;
            screening.InfectiousDiseaseExposure = model.InfectiousDiseaseExposure;
            screening.MalariaRiskAssessment = model.MalariaRiskAssessment;
            screening.RecentCOVID19DiagnosisOrExposure = model.RecentCOVID19DiagnosisOrExposure;
            if(screening.RecentTattooOrPiercing == "Yes" || screening.IsPregnant =="Yes"|| screening.InfectiousDiseaseExposure =="Yes" || screening.MalariaRiskAssessment == "Yes"|| screening.RecentCOVID19DiagnosisOrExposure == "Yes")
            {
                string appid = Session["ScreenappId"] as string;
                int appId = int.Parse(appid);
                var appointment = db.Appointments.Find(appId);
                // Prepare email message
                var email = new MailMessage();
                email.From = new MailAddress("BloodDonorAC@outlook.com");
                email.To.Add(screening.Email);
                email.Subject = "Screening";
                email.Body = "Dear, " + appointment.Name + " " + appointment.Surname + ".\n\nYou have successfully completed our screening process.\n\nResults: Failed" + ".\n" +
            "\n\n\n" +
            "Thank you. Your blood saves lives." +
            "\n" +
            "\n\nWarm Regards,\n" + appointment.Center;
                // Use the SMTP settings from web.config
                var smtpClient2 = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient2.Send(email);
                appointment.status = "Settled";
                assignment.status = "Settled";
                db.Entry(assignment).State = EntityState.Modified;
                db.Entry(appointment).State = EntityState.Modified;
                db.BloodDonationScreenings.Remove(screening);
                db.SaveChanges();
                TempData["ScreeningFailure"] = "Screening Failed, Please note that the potential donor did not pass our screening process.";
                return RedirectToAction("MyAppointments", "DrAssignments");
            }

            db.Entry(screening).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("HealthAssessment");
        }
        public ActionResult HealthAssessment()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HealthAssessment(BloodDonationScreening model)
        {
            string assignId = Session["ScreenassignId"] as string;
            int assignid = int.Parse(assignId);
            var assignment = db.DrAssignments.Find(assignid);
            string screenId = Session["ScreenId"] as string;
            int id = int.Parse(screenId);
            var screening = db.BloodDonationScreenings.Find(id);
            screening.BloodPressure = model.BloodPressure;
            screening.PulseRate = model.PulseRate;
            screening.HemoglobinOrHematocrit = model.HemoglobinOrHematocrit;
            screening.Weight = model.Weight;
            screening.Height = model.Height;

            if(screening.Weight <50 || screening.BloodPressure>100 || !(50 < screening.PulseRate && screening.PulseRate < 100))
            {
                string appid = Session["ScreenappId"] as string;
                int appId = int.Parse(appid);
                var appointment = db.Appointments.Find(appId);
                // Prepare email message
                var email = new MailMessage();
                email.From = new MailAddress("BloodDonorAC@outlook.com");
                email.To.Add(screening.Email);
                email.Subject = "Screening";
                email.Body = "Dear, " + appointment.Name + " " + appointment.Surname + ".\n\nYou have successfully completed our screening process.\n\nResults: Failed" + ".\n" +
            "\n\n\n" +
            "Thank you. Your blood saves lives." +
            "\n" +
            "\n\nWarm Regards,\n" + appointment.Center;
                // Use the SMTP settings from web.config
                var smtpClient2 = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient2.Send(email);
                appointment.status = "Settled";
                assignment.status = "Settled";
                db.Entry(assignment).State = EntityState.Modified;
                db.Entry(appointment).State = EntityState.Modified;
                db.BloodDonationScreenings.Remove(screening);
                db.SaveChanges();
                TempData["ScreeningFailure"] = "Screening Failed, Please note that the potential donor did not pass our screening process.";
                return RedirectToAction("MyAppointments", "DrAssignments");

            }
            db.Entry(screening).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DonorConsent");
        }
        public ActionResult DonorConsent()
        {
            BloodDonationScreening b = new BloodDonationScreening
            {
                DonationDate = DateTime.Now.ToShortDateString()
            };
            return View(b);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DonorConsent(BloodDonationScreening model)
        {
            try
            {
                string assignId = Session["ScreenassignId"] as string;
                int assignid = int.Parse(assignId);
                var assignment = db.DrAssignments.Find(assignid);

                string appid = Session["ScreenappId"] as string;
                int appId = int.Parse(appid);
                var appointment = db.Appointments.Find(appId);
                string screenId = Session["ScreenId"] as string;
                int id = int.Parse(screenId);
                var screening = db.BloodDonationScreenings.Find(id);
                screening.ConsentToDonateBlood = model.ConsentToDonateBlood;
                screening.ConsentToShareHealthInfo = model.ConsentToShareHealthInfo;
                if(model.ConsentToDonateBlood == "No")
                {

                    // Prepare email message
                    var email3 = new MailMessage();
                    email3.From = new MailAddress("BloodDonorAC@outlook.com");
                    email3.To.Add(screening.Email);
                    email3.Subject = "Screening";
                    email3.Body = "Dear, " + appointment.Name + " " + appointment.Surname + ".\n\nYou have successfully completed our screening process.\n\nResults: Failed" + ".\n" +
                "\n\n\n" +
                "Thank you. Your blood saves lives." +
                "\n" +
                "\n\nWarm Regards,\n" + appointment.Center;
                    // Use the SMTP settings from web.config
                    var smtpClient3 = new SmtpClient();
                    // The SmtpClient will automatically use the settings from web.config
                    smtpClient3.Send(email3);
                    appointment.status = "Settled";
                    assignment.status = "Settled";
                    db.Entry(assignment).State = EntityState.Modified;
                    db.Entry(appointment).State = EntityState.Modified;
                    db.BloodDonationScreenings.Remove(screening);
                    db.SaveChanges();
                    TempData["ScreeningFailure"] = "Screening Failed, Please note that the potential donor did not pass our screening process.";
                    return RedirectToAction("MyAppointments", "DrAssignments");
                }
                if (model.ConsentToShareHealthInfo == "No")
                {
                 
                        // Prepare email message
                        var email = new MailMessage();
                        email.From = new MailAddress("BloodDonorAC@outlook.com");
                        email.To.Add(screening.Email);
                        email.Subject = "Screening";
                        email.Body = "Dear, " + appointment.Name + " " + appointment.Surname + ".\n\n You have successfully completed our screening process.\n\nResults: Failed" + ".\n" +
                    "\n\n\n" +
                    "Thank you. Your blood saves lives." +
                    "\n" +
                    "\n\nWarm Regards,\n" + appointment.Center;
                        // Use the SMTP settings from web.config
                        var smtpClient2 = new SmtpClient();
                        // The SmtpClient will automatically use the settings from web.config
                        smtpClient2.Send(email);
                    appointment.status = "Settled";
                    assignment.status = "Settled";
                    db.Entry(assignment).State = EntityState.Modified;
                    db.Entry(appointment).State = EntityState.Modified;
                    db.BloodDonationScreenings.Remove(screening);
                    db.SaveChanges();
                    TempData["ScreeningFailure"] = "Screening Failed, Please note that the potential donor did not pass our screening process.";
                    return RedirectToAction("MyAppointments", "DrAssignments");
                    
                }

                //screening.DonorSignature = model.DonorSignature;
                screening.DonationDate = model.DonationDate;
                screening.status = "Settled";
                appointment.status = "Screening";
                assignment.status = "Passed";
                db.Entry(assignment).State = EntityState.Modified;
                db.Entry(appointment).State = EntityState.Modified;
                db.Entry(screening).State = EntityState.Modified;
                
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(screening.Email);
                email2.Subject = "Screening";
                email2.Body = "Dear, " + appointment.Name + " " + appointment.Surname + ".\n\nYou have successfully completed our screening process.\n\nResults: Passed" + ".\n" +
            "\n\n\n" +
            "Thank you. Your blood saves lives." +
            "\n" +
            "\n\nWarm Regards,\n" + appointment.Center;
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);

                db.SaveChanges();
           
                TempData["ScreeningSuccess"] = "Screening Process Completed Success Fully, Email sent to Donor";
                Session["ScreenId"] = null;
                return RedirectToAction("MyAppointments", "DrAssignments");
            }
            catch
            {
                string screenId = Session["ScreenId"] as string;
                int id = int.Parse(screenId);
                var screening = db.BloodDonationScreenings.Find(id);
                db.BloodDonationScreenings.Remove(screening);
                db.SaveChanges();
                TempData["ScreeningFailure"] = "Something went wrong while processing your request, Please try again later.";
                return RedirectToAction("MyAppointments", "DrAssignments");
            }

           
        }


    }
}