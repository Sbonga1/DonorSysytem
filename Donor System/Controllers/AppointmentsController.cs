using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Donor_System.Models;

namespace Donor_System.Controllers
{
    public class AppointmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Appointments
        public ActionResult Index()
        {
            return View(db.Appointments.ToList());
        }
        public ActionResult CenterAppointments(string center)
        {
            var appointments = db.Appointments.Where(c => c.Center == center);
            return View(appointments.ToList());
        }

        //public ActionResult DoctorAppointments(string center)
        //{
        //    var appointments = db.Appointments.Where(c => c.drEmail == center); ;
        //    return View(appointments.ToList());
        //}


        public ActionResult DonorAppointments()
        {
            var appointments = db.Appointments.Where(x => x.Email == User.Identity.Name);
            return View(appointments.ToList());
        }

        public ActionResult DeclineRequest(int id, string reason)
        {
            try
            {
                var request = db.Appointments.Find(id);
                request.status = "Declined";
                request.declineReason = reason;
                db.Entry(request).State = EntityState.Modified;
                // Prepare email message
                var email2 = new MailMessage();
                email2.From = new MailAddress("BloodDonorAC@outlook.com");
                email2.To.Add(request.Email);
                email2.Subject = "Appointment Declined";
                email2.Body = "Dear, " + request.Name + " " + request.Surname + "\n\nAppointment for donating blood was declined due to " + reason + ".\n" +
            "\n\n\n" +
            "Thank you." +
            "\n" +
            "\n\nWarm Regards,\n" + "Durban Donation";
                // Use the SMTP settings from web.config
                var smtpClient = new SmtpClient();
                // The SmtpClient will automatically use the settings from web.config
                smtpClient.Send(email2);
                db.SaveChanges();
                
                var responseData = new { message = "Blood donation appointment declined successfully, Email sent to donor." };
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
                email2.Subject = "Appointment Cancelled";
                email2.Body = "Dear, " + request.Name + " " + request.Surname + "\n\n Your appointment has been canceled successfully" + ".\n" +
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
                var responseData = new { message = "Appointment cancelled successfully, Please check your emails for more info." };
                return Json(responseData);

            }
            catch (Exception ex)
            {
                //TempData["CancelFailure"] = "Sorry we couldn't process your request at the moment, Please try again later.";
                //return RedirectToAction("MyRequest");
                return Json(new { error = ex.Message });
            }


        }

       

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create(string center, int timeId)
        {
            Session["TimeslotId"] = timeId.ToString();
            var timeslot = db.AvailableTimes.Where(x => x.Id == timeId).FirstOrDefault();
            string startTime = timeslot.Date.ToShortDateString() + " " + timeslot.StartTime.ToShortTimeString();
            string endTime = timeslot.Date.ToShortDateString() + " " + timeslot.EndTime.ToShortTimeString(); 
            Appointment d = new Appointment()
            {
                Start = startTime,
                End = endTime,
                Center = center,
                Email = User.Identity.Name
            };
            return View(d);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Surname,Email,Center,Start,End,PhoneNumber,IdNumber")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string year = appointment.GetYearFromSAID(appointment.IdNumber);

                    int age = ((int)DateTime.Now.Year) - int.Parse(year);
                    if (age < 18)
                    {
                        TempData["AppointmentFailure"] = "Sorry persons under the age of 18 are not allowed to donate ,your request could not be submitted due to age restriction.";
                        return View(appointment);
                    }
                    string id = Session["TimeslotId"] as string;
                    
                    int timeid = int.Parse(id);
                    var timeslot = db.AvailableTimes.Where(x => x.Id == timeid).FirstOrDefault();
                    // timeslot.IsAvailable = false;
                    // timeslot.BookedBy = appointment.Email;
                    db.AvailableTimes.Remove(timeslot);
                    
                    appointment.status = "Awaiting Approval";

                    // Prepare email message
                    var email2 = new MailMessage();
                    email2.From = new MailAddress("BloodDonorAC@outlook.com");
                    email2.To.Add(User.Identity.Name);
                    email2.Subject = "Donor Appointment";
                    email2.Body = "Dear, " + appointment.Name + " " + appointment.Surname + "\n\n Your appointment for " + timeslot.Date.ToShortDateString() + " has been successfully captured, one of our staff members will approve it ASP" + ".\n" +
                "\n\n\n" +
                "Thank you. Your blood saves lives." +
                "\n" +
                "\n\nWarm Regards,\n" + appointment.Center;
                    // Use the SMTP settings from web.config
                    var smtpClient = new SmtpClient();
                    // The SmtpClient will automatically use the settings from web.config
                    smtpClient.Send(email2);

                    db.Appointments.Add(appointment);
                    db.SaveChanges();

                    TempData["AppointmentSuccess"] = "Appointment successfully submitted, Please check your emails for more info.";
                    return RedirectToAction("DonorAppointments");
                   
                }
                catch
                {
                    TempData["AppointmentFailure"] = "Something went wrong, Your request could not be submitted please try again later.";
                    return View(appointment);
                }
                
            }

            return View(appointment);
        }
        

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Email,Start,End,PhoneNumber")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
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
