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
    public class ExternalDrRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ExternalDrRequests
        public ActionResult Index()
        {
            return View(db.ExternalDrRegs.ToList());
        } 
        public ActionResult MyRequests()
        {
            var requests = db.ExternalDrRegs.Where(x => x.Email == User.Identity.Name);
            return View(requests.ToList());
        }
        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var request = db.ExternalDrRegs.Find(id);
                request.Status = "Approved";
                db.Entry(request).State = EntityState.Modified;

                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(request.Email);
                email2.Subject = "Registration Request Approved";
                email2.Body = "Dear, DR " + request.Surname + " " + request.Name + ".\n\nPlease note that your Request for "  + "Registration " + " has been Approved" + ".\n" +
            "\n\n\n" +
            "Thank you." +
            "\n" +
            "\n\nWarm Regards,\n" + "Durban Donation";
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);
                db.SaveChanges();
                TempData["ReqApproveSuccess"] = "Request approved successfully, Email sent to doctor.";
                return RedirectToAction("Index");
            }
            catch
            {
               
                TempData["ReqApproveFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                return RedirectToAction("Index");
            }



        }

        public ActionResult DeclineRequest(int id, string reason)
        {
            try
            {
                var request = db.ExternalDrRegs.Find(id);
                request.Status = "Declined";
                request.declineReason = reason;
                db.Entry(request).State = EntityState.Modified;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(request.Email);
                email2.Subject = "Registration Request Declined";
                email2.Body = "Dear, DR " + request.Surname + " " + request.Name + "\n\n Your Registration Request has been declined due to "  + reason + ".\n" +
            "\n\n\n" +
            "Thank you." +
            "\n" +
            "\n\nWarm Regards,\n" + "Durban Donation";
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);
                db.SaveChanges();
                //TempData["CancelSuccess"] = "";
                //return RedirectToAction("MyRequest");
                var responseData = new { message = "Account request declined successfully, Email sent to doctor." };
                return Json(responseData);

            }
            catch (Exception ex)
            {
                //TempData["CancelFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                //return RedirectToAction("MyRequest");
                return Json(new { error = ex.Message });
            }


        }

        public ActionResult CancelRequest(int id, string reason = "No reason")
        {
            try
            {
                var request = db.Appointments.Find(id);
                request.status = "Cancelled";
                request.cancelReason = reason;
                db.Entry(request).State = EntityState.Modified;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(User.Identity.Name);
                email2.Subject = "Request Cancelled";
                email2.Body = "Dear, " + request.Name + " " + request.Surname + "\n\n Your request has been canceled successfully" + ".\n" +
            "\n\n\n" +
            "Thank you." +
            "\n" +
            "\n\nWarm Regards,\n" + "Durban Donation";
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);
                db.SaveChanges();
                //TempData["CancelSuccess"] = "";
                //return RedirectToAction("MyRequest");
                var responseData = new { message = "Request cancelled successfully, Please check your emails for more info." };
                return Json(responseData);

            }
            catch (Exception ex)
            {
                //TempData["CancelFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                //return RedirectToAction("MyRequest");
                return Json(new { error = ex.Message });
            }


        }


        public ActionResult DownloadPdf(int id)
        {
            var pdfFile = db.ExternalDrRegs.FirstOrDefault(f => f.Id == id);
            if (pdfFile != null)
            {
                return File(pdfFile.FileContent, "application/pdf", pdfFile.FileName);
            }
            return HttpNotFound();
        }
    }
}