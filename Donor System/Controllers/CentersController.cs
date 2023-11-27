using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Donor_System.Models;

namespace Donor_System.Controllers
{
    public class CentersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Centers
        public ActionResult Index()
        {
            return View(db.Centers.ToList());
        }



        public ActionResult DonationCenters()
        {
            var centers = db.Centers.ToList();
            var timeSlotsDictionary = db.AvailableTimes
                                        //.Where(x => x.IsAvailable == true)
                                        .GroupBy(x => x.Center)
                                        .ToDictionary(x => x.Key, x => x.ToList());
            var combinedData = centers
                .Select(d => new CenterTimeslotsViewModel
                {
                    Center = d,
                    AvailableTime = new AvailableTime // Initialize TimeSlot
                    {
                        AvailableTimeSlots = timeSlotsDictionary.ContainsKey(d.Email) ? timeSlotsDictionary[d.Email] : new List<AvailableTime>()
                    }
                })
                .ToList();

            return View(combinedData);
        }

        // GET: Centers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Center center = db.Centers.Find(id);
            if (center == null)
            {
                return HttpNotFound();
            }
            return View(center);
        }

        // GET: Centers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Centers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,City,Province,ZipCode,Phone,Email,Picture")] Center center, HttpPostedFileBase pictureFile)
        {
            if (ModelState.IsValid)
            {
                // Save the picture file on the server
                string pictureFileName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName);
                string picturePath = Path.Combine(Server.MapPath("~/"), pictureFileName);
                pictureFile.SaveAs(picturePath);

                // Set the picture path in the record
                center.Picture = pictureFileName;

                db.Centers.Add(center);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(center);
        }

        // GET: Centers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Center center = db.Centers.Find(id);
            if (center == null)
            {
                return HttpNotFound();
            }
            return View(center);
        }

        // POST: Centers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,City,Province,ZipCode,Phone,Email,Picture")] Center center)
        {
            if (ModelState.IsValid)
            {
                db.Entry(center).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(center);
        }

        // GET: Centers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Center center = db.Centers.Find(id);
            if (center == null)
            {
                return HttpNotFound();
            }
            return View(center);
        }

        // POST: Centers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Center center = db.Centers.Find(id);
            db.Centers.Remove(center);
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
