using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Donor_System.Models;

namespace Donor_System.Controllers
{
    public class BloodRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BloodRequests
        public ActionResult Index()
        {
            return View(db.BloodRequests.ToList());
        }
        public ActionResult MyRequests()
        {
            var requests = db.BloodRequests.Where(x => x.DoctorEmail == User.Identity.Name);
            return View(requests.ToList());
        }
        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var request = db.BloodRequests.Find(id);
                BloodRequest br = new BloodRequest();
                bool IsEnough = br.IsEnoughBood(request.BloodType,double.Parse(request.AmtRequired) ,request.DonationLocation);
                if(IsEnough)
                {
                    request.Status = "Approved";
                    db.Entry(request).State = EntityState.Modified;

                    // Prepare email message
                    var email2 = new MailMessage();
                    email2.From = new MailAddress("BloodDonorAC@outlook.com");
                    email2.To.Add(request.DoctorEmail);
                    email2.Subject = "Blood Request Approved";
                    email2.Body = "Dear, DR " + request.DoctorSurname + " " + request.DoctorName + "\n\nYour blood request for " + request.PatientSurname + " " + request.PatientName + " has been approved" + ".\n" +
                "\n\n\n" +
                "Thank you." +
                "\n" +
                "\n\nWarm Regards,\n" + "Durban Donation";
                    // Use the SMTP settings from web.config
                    var smtpClient = new SmtpClient();
                    // The SmtpClient will automatically use the settings from web.config
                    smtpClient.Send(email2);
                    db.SaveChanges();
                    TempData["ApproveSuccess"] = "Request approved successfully, Email sent to doctor.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ApproveSuccess"] = "Request could not be approved at this time, There is no enough blood type " + request.BloodType + " available.";
                    return RedirectToAction("Index");
                }
               
            }
            catch
            {
                TempData["ApproveFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                return RedirectToAction("Index");
            }


           
        }

        public ActionResult DeclineRequest(int id, string reason)
        {
            try
            {
                var request = db.BloodRequests.Find(id);
                request.Status = "Declined";
                request.declineReason = reason;
                db.Entry(request).State = EntityState.Modified;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(request.DoctorEmail);
                email2.Subject = "Blood Request Declined";
                email2.Body = "Dear, DR " + request.DoctorSurname + " " + request.DoctorName + "\n\n Your blood request for " + request.PatientSurname + " " + request.PatientName + " was declined due to " + reason +  ".\n" +
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
                var responseData = new { message = "Blood request declined successfully, Email sent to doctor." };
                return Json(responseData);

            }
            catch (Exception ex)
            {
                //TempData["CancelFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                //return RedirectToAction("MyRequest");
                return Json(new { error = ex.Message });
            }


        }

        public ActionResult CancelRequest(int id, string reason)
        {
            try
            {
                var request = db.BloodRequests.Find(id);
                request.Status = "Cancelled";
                request.cancelReason = reason;
                db.Entry(request).State = EntityState.Modified;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(User.Identity.Name);
                email2.Subject = "Blood Request Cancelled";
                email2.Body = "Dear, DR " + request.DoctorSurname + " " + request.DoctorName + "\n\n Your blood request for " + request.PatientSurname + " " + request.PatientName + " has been canceled successfully" + ".\n" +
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
                var responseData = new { message = "Blood request cancelled successfully, Please check your emails for more info." };
                return Json(responseData);

            }
            catch (Exception ex)
            {
                //TempData["CancelFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                //return RedirectToAction("MyRequest");
                return Json(new { error = ex.Message });
            }
            

        }

        //public ActionResult DownloadPdf(int id)
        //{
        //    var pdfFile = db.BloodRequests.FirstOrDefault(f => f.Id == id);
        //    if (pdfFile != null)
        //    {
        //        return File(pdfFile.FileContent, "application/pdf", pdfFile.FileName);
        //    }
        //    return HttpNotFound();
        //}
        public ActionResult DownloadConsentPdf(int id)
        {
            var pdfFile = db.BloodRequests.FirstOrDefault(f => f.Id == id);
            if (pdfFile != null)
            {
                return File(pdfFile.ConsentFileContent, "application/pdf", pdfFile.ConsentFileName);
            }
            return HttpNotFound();
        }

        // GET: BloodRequests/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodRequest bloodRequest = db.BloodRequests.Find(id);
            if (bloodRequest == null)
            {
                return HttpNotFound();
            }
            return View(bloodRequest);
        }

        // GET: BloodRequests/Create
        public ActionResult Create()
        {
            ViewBag.Name = new SelectList(db.Centers.ToList(), "Email", "Email");
            var Dr = db.ExternalDrRegs.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            BloodRequest b = new BloodRequest
            {
                DoctorName = Dr.Name,
                DoctorEmail = Dr.Email,
                DoctorSurname = Dr.Surname
             

            };
            return View(b);
        }

        // POST: BloodRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientName,PatientSurname,PatientIDNumber,DoctorName,DoctorSurname,DoctorEmail,BloodType, DonationLocation,AmtRequired,ConsentFileName,ConsentFileContent")] BloodRequest bloodRequest,/* HttpPostedFileBase file,*/ HttpPostedFileBase Consentfile)
        {
            if (ModelState.IsValid)
            {
                //if (file != null && file.ContentLength > 0)
                //{
                //    byte[] pdfData;
                //    using (var binaryReader = new BinaryReader(file.InputStream))
                //    {
                //        pdfData = binaryReader.ReadBytes(file.ContentLength);
                //    }

                //    // Create a PdfFile object and save it to the database

                //    bloodRequest.FileName = file.FileName;
                //    bloodRequest.FileContent = pdfData;
                //}
                if (Consentfile != null && Consentfile.ContentLength > 0)
                {
                    byte[] pdfData;
                    using (var binaryReader = new BinaryReader(Consentfile.InputStream))
                    {
                        pdfData = binaryReader.ReadBytes(Consentfile.ContentLength);
                    }

                    // Create a PdfFile object and save it to the database

                    bloodRequest.ConsentFileName = Consentfile.FileName;
                    bloodRequest.ConsentFileContent = pdfData;
                }

                bloodRequest.Status = "Awaiting Approval";
                db.BloodRequests.Add(bloodRequest);
                db.SaveChanges();
                return RedirectToAction("MyRequests");
            }

            return View(bloodRequest);
        }

        // GET: BloodRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodRequest bloodRequest = db.BloodRequests.Find(id);
            if (bloodRequest == null)
            {
                return HttpNotFound();
            }
            return View(bloodRequest);
        }

        // POST: BloodRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientName,PatientSurname,PatientIDNumber,DoctorName,DoctorSurname,DoctorEmail,BloodType,AmtRequired,FileName,FileContent")] BloodRequest bloodRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bloodRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bloodRequest);
        }

        // GET: BloodRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodRequest bloodRequest = db.BloodRequests.Find(id);
            if (bloodRequest == null)
            {
                return HttpNotFound();
            }
            return View(bloodRequest);
        }

        // POST: BloodRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BloodRequest bloodRequest = db.BloodRequests.Find(id);
            db.BloodRequests.Remove(bloodRequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
