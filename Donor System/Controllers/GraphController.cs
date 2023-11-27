using Donor_System.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Donor_System.Controllers
{
    public class GraphController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Graph
        public ActionResult Index()
        {
            
            var donationData = db.BloodDonationRecords.ToList();

            
           var requestData = db.BloodRequestRecords.ToList();

            // Process the donation data as needed for your graph
            var groupedDonationData = donationData
                .GroupBy(s => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(s.DonationDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Week = g.Key,
                    TotalBloodDonated = g.Sum(s => s.amtBloodDonated)
                })
                .ToList();

            
            var groupedRequestData = requestData
                .GroupBy(s => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(s.RequestDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Week = g.Key,
                    TotalBloodRequested = g.Sum(s => s.amtBloodRequested)
                })
                .ToList();

            // Merge donation and request data by week
            var mergedData = groupedDonationData
                .Join(groupedRequestData,
                      d => d.Week,
                      r => r.Week,
                      (d, r) => new
                      {
                          Week = d.Week,
                          TotalBloodDonated = d.TotalBloodDonated,
                          TotalBloodRequested = r.TotalBloodRequested
                      })
                .ToList();

            var labels = mergedData.Select(g => $"Week {g.Week}").ToArray(); // Example: Week labels
            var donatedData = mergedData.Select(g => g.TotalBloodDonated).ToArray(); // Example: Total donated amounts by week
            var requestedData = mergedData.Select(g => g.TotalBloodRequested).ToArray(); // Example: Total requested amounts by week

            // Pass data to the view
            ViewData["ChartLabels"] = labels;
            ViewData["DonatedData"] = donatedData;
            ViewData["RequestedData"] = requestedData;

            return View();
        }

        public ActionResult BloodTypeChart()
        {
            var donationData = db.BloodDonationRecords.ToList();
            var requestData = db.BloodRequestRecords.ToList();

            var bloodAvailabilityData = donationData
                .GroupBy(s => s.BloodType)
                .Select(g => new BloodAvailability
                {
                    BloodType = g.Key,
                    AvailableAmount = g.Sum(s => s.amtBloodDonated) - requestData
                        .Where(r => r.BloodType == g.Key)
                        .Sum(r => r.amtBloodRequested),
                    DonatedAmount = g.Sum(s => s.amtBloodDonated),
                    RequestedAmount = requestData
                        .Where(r => r.BloodType == g.Key)
                        .Sum(r => r.amtBloodRequested)
                })
                .ToList();

            var bloodTypes = bloodAvailabilityData.Select(d => d.BloodType).ToArray();
            var availableAmounts = bloodAvailabilityData.Select(d => d.AvailableAmount).ToArray();
            var donatedAmounts = bloodAvailabilityData.Select(d => d.DonatedAmount).ToArray();
            var requestedAmounts = bloodAvailabilityData.Select(d => d.RequestedAmount).ToArray();

            ViewData["ChartLabels"] = bloodTypes;
            ViewData["AvailableData"] = availableAmounts;
            ViewData["DonatedData"] = donatedAmounts;
            ViewData["RequestedData"] = requestedAmounts;

            return View();
        }



    }
}