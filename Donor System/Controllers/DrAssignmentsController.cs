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
    public class DrAssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DrAssignments
        public ActionResult Index()
        {
            return View(db.DrAssignments.ToList());
        }
        public ActionResult MyAppointments()
        {
            var assignments = db.DrAssignments.Where(x => x.drEmail == User.Identity.Name);

            return View(assignments.ToList());
        }

        // GET: DrAssignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DrAssignment drAssignment = db.DrAssignments.Find(id);
            if (drAssignment == null)
            {
                return HttpNotFound();
            }
            return View(drAssignment);
        }

        // GET: DrAssignments/Create
        public ActionResult Create(int drId)
        {
           
            var doctor = db.Doctors.Find(drId);
            string appId = Session["AssignAppId"] as string;
            int convAppId = int.Parse(appId);
            var appointment = db.Appointments.Find(convAppId);
            DrAssignment b = new DrAssignment
            {
                DoctorName = doctor.Name,
                DoctorSurname = doctor.Surname,
                Start = appointment.Start,
                End = appointment.End,
                drEmail = doctor.Email

            };

            return View(b);
        }

        // POST: DrAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DoctorName,DoctorSurname,Start,End,drEmail")] DrAssignment drAssignment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string appId = Session["AssignAppId"] as string;
                    int convAppId = int.Parse(appId);
                    var appointment = db.Appointments.Find(convAppId);
                    drAssignment.appId = convAppId;
                    drAssignment.DonorEmail = appointment.Email;
                    drAssignment.DonorName = appointment.Name;
                    drAssignment.DonorSurname = appointment.Surname;
                    drAssignment.status = "Assigned";
                    appointment.status = "Approved";
                    appointment.drEmail = drAssignment.drEmail;
                    drAssignment.Center = appointment.Center;
                    db.Entry(appointment).State = EntityState.Modified;
                    db.DrAssignments.Add(drAssignment);

                    DateTime start = DateTime.Parse(drAssignment.Start);
                    DateTime end =DateTime.Parse(drAssignment.End);

                    // Prepare email message
                    var email1 = new MailMessage();
                    email1.From = new MailAddress("BloodDonorAC@outlook.com");
                    email1.To.Add(appointment.drEmail);
                    email1.Subject = "Appointment Assignment";
                    email1.Body = "Dear, DR " + appointment.Surname + "\n\nPlease note that you have been assigned to an appointment at "  + start.ToShortDateString()+ " from" + start.ToShortTimeString()+ " to "+ end.ToShortTimeString()+".\n" +
                "\n\n\n" +
                "Thank you." +
                "\n" +
                "\n\nWarm Regards,\n" + appointment.Center;
                    // Use the SMTP settings from web.config
                    var smtpClient = new SmtpClient();
                    // The SmtpClient will automatically use the settings from web.config
                    smtpClient.Send(email1);


                    // Prepare email message
                    var email2 = new MailMessage();
                    email2.From = new MailAddress("BloodDonorAC@outlook.com");
                    email2.To.Add(User.Identity.Name);
                    email2.Subject = "Donor Appointment";
                    email2.Body = "Dear, " + appointment.Name + " " + appointment.Surname + "\n\n Please note that your appointment has been approved." + ".\n" +
                "\n\n\n" +
                "Thank you. Your blood saves lives." +
                "\n" +
                "\n\nWarm Regards,\n" + appointment.Center;
                    // Use the SMTP settings from web.config
                    var smtpClient2 = new SmtpClient();
                    // The SmtpClient will automatically use the settings from web.config
                    smtpClient2.Send(email2);

                    db.SaveChanges();
                    Session["AssignAppId"] = null;
                    TempData["AssignmentSuccess"] = "Doctor " + drAssignment.DoctorSurname + " is successfully assigned to Appointment, Email sent to donor.";
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["AssignmentFailure"] = "Something went wrong, Failed to assign" + drAssignment.DoctorSurname + "to Appointment.";
                    return View(drAssignment);
                }
            }

            return View(drAssignment);
        }

        // GET: DrAssignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DrAssignment drAssignment = db.DrAssignments.Find(id);
            if (drAssignment == null)
            {
                return HttpNotFound();
            }
            return View(drAssignment);
        }

        // POST: DrAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Email,Start,End,drEmail")] DrAssignment drAssignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(drAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(drAssignment);
        }

        // GET: DrAssignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DrAssignment drAssignment = db.DrAssignments.Find(id);
            if (drAssignment == null)
            {
                return HttpNotFound();
            }
            return View(drAssignment);
        }

        // POST: DrAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DrAssignment drAssignment = db.DrAssignments.Find(id);
            db.DrAssignments.Remove(drAssignment);
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
