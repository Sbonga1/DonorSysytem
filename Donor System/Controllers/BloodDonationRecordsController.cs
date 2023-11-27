using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Donor_System.Models;

namespace Donor_System.Controllers
{
    public class BloodDonationRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BloodDonationRecords
        public ActionResult Index()
        {
            return View(db.BloodDonationRecords.ToList());
        }
        public ActionResult DonorRecords()
        {
            var records = db.BloodDonationRecords.Where(x => x.DonorEmail == User.Identity.Name);
            return View(records.ToList());
        }

        // GET: BloodDonationRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodDonationRecord bloodDonationRecord = db.BloodDonationRecords.Find(id);
            if (bloodDonationRecord == null)
            {
                return HttpNotFound();
            }
            return View(bloodDonationRecord);
        }

        // GET: BloodDonationRecords/Create
        public ActionResult Create(int appId,string drEmail, int drAppId)
        {
            Session["RecDrEmail"] = drEmail;
            Session["RecAppId"] = appId.ToString();
            Session["drAppId"] = drAppId.ToString();
            var appointment = db.Appointments.Find(appId);
            
            BloodDonationRecord b = new BloodDonationRecord
            {
                DonorEmail = appointment.Email,
                IdNumber = appointment.IdNumber,
            };
            return View(b);
        }

        // POST: BloodDonationRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DonorEmail,IdNumber,BloodType,amtBloodDonated")] BloodDonationRecord bloodDonationRecord)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string drAppID = Session["drAppId"] as string;
                    int drAppId = int.Parse(drAppID);
                    var drAssign = db.DrAssignments.Find(drAppId);
                    drAssign.status = "Settled";
                    var IsExist = db.BloodDonationRecords.Where(x => x.IdNumber == bloodDonationRecord.IdNumber).FirstOrDefault();
                    string appid = Session["RecAppId"] as string;
                    int appId = int.Parse(appid);
                    var appointment = db.Appointments.Find(appId);
                    appointment.status = "Settled";
                    db.Entry(appointment).State = EntityState.Modified;
                    db.Entry(drAssign).State = EntityState.Modified;
                    string drEmail = Session["RecDrEmail"] as string;
                    if (IsExist != null)
                    {
                        IsExist.DonationDate = DateTime.Now.Date;
                        IsExist.DonationFrequency += 1;
                        IsExist.amtBloodDonated += bloodDonationRecord.amtBloodDonated;
                        db.Entry(IsExist).State = EntityState.Modified;


                        //Send Email
                        // Prepare email message
                        var email2 = new MailMessage();
                        email2.From = new MailAddress("BloodDonorAC@outlook.com");
                        email2.To.Add(appointment.Email);
                        email2.Subject = "Donation Record";
                        email2.Body = "Dear, " + appointment.Name + " " + appointment.Surname + "\n\n Your Donation Record Has been Updated Successfully" + ".\n" +
                    "\n\n\n" +
                    "Thank you. Your blood saves lives." +
                    "\n" +
                    "\n\nWarm Regards,\n" + appointment.Center;
                       
                        var smtpClient = new SmtpClient();
                       
                        smtpClient.Send(email2);
                        TempData["DonationRecordSuccess"] = "Donation Record successfully Updated, Email Sent to Donor.";
                        db.SaveChanges();
                        return RedirectToAction("Index");

                    }
                    else
                    {
                       

                        var doctor = db.Doctors.Where(x => x.Email == drEmail).FirstOrDefault();
                        bloodDonationRecord.BloodType = bloodDonationRecord.BloodType;

                        bloodDonationRecord.DrId = doctor.Id;
                        bloodDonationRecord.DonationDate = DateTime.Now.Date;
                        bloodDonationRecord.DonationLocation = doctor.center;
                        bloodDonationRecord.DonationFrequency = 1;
                        db.BloodDonationRecords.Add(bloodDonationRecord);

                        //Send Email
                        // Prepare email message
                        var email2 = new MailMessage();
                        email2.From = new MailAddress("BloodDonorAC@outlook.com");
                        email2.To.Add(appointment.Email);
                        email2.Subject = "Donation Record";
                        email2.Body = "Dear, " + appointment.Name + " " + appointment.Surname + "\n\n Your Donation Record Has been Created Successfully" + ".\n" +
                    "\n\n\n" +
                    "Thank you. Your blood saves lives." +
                    "\n" +
                    "\n\nWarm Regards,\n" + appointment.Center;
                        // Use the SMTP settings from web.config
                        var smtpClient = new SmtpClient();
                        // The SmtpClient will automatically use the settings from web.config
                        smtpClient.Send(email2);
                        TempData["DonationRecordSuccess"] = "Donation Record successfully Created, Email Sent to Donor.";
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData["DonationRecordFailure"] = "Something went wrong, Your request could not be submitted please try again later. " + ex.Message;
                    return View(bloodDonationRecord);
                }
            }

            return View(bloodDonationRecord);
        }

        // GET: BloodDonationRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodDonationRecord bloodDonationRecord = db.BloodDonationRecords.Find(id);
            if (bloodDonationRecord == null)
            {
                return HttpNotFound();
            }
            return View(bloodDonationRecord);
        }

        // POST: BloodDonationRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DonorEmail,IdNumber,DrId,amtBloodDonated,DonationDate,DonationLocation,DonationFrequency")] BloodDonationRecord bloodDonationRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bloodDonationRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bloodDonationRecord);
        }

        // GET: BloodDonationRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodDonationRecord bloodDonationRecord = db.BloodDonationRecords.Find(id);
            if (bloodDonationRecord == null)
            {
                return HttpNotFound();
            }
            return View(bloodDonationRecord);
        }

        // POST: BloodDonationRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BloodDonationRecord bloodDonationRecord = db.BloodDonationRecords.Find(id);
            db.BloodDonationRecords.Remove(bloodDonationRecord);
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
