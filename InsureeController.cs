using System;
using System.Linq;
using System.Web.Mvc;
using YourNamespace.Models;

namespace YourNamespace.Controllers
{
    public class InsureeController : Controller
    {
        private YourDbContext db = new YourDbContext();

        // POST: Insuree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,SpeedingTickets,DUI,CoverageType")] Insuree insuree)
        {
            decimal baseMonthlyFee = 50m; // Start with a base fee of $50 per month
            decimal quote = baseMonthlyFee;

            // Calculate age
            int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
            if (DateTime.Now.DayOfYear < insuree.DateOfBirth.DayOfYear)
            {
                age--; // Adjust if the birthday hasn't occurred yet this year
            }

            // Age-based adjustments
            if (age <= 18)
            {
                quote += 100;
            }
            else if (age >= 19 && age <= 25)
            {
                quote += 50;
            }
            else if (age >= 26)
            {
                quote += 25;
            }

            // Car year adjustments
            if (insuree.CarYear < 2000)
            {
                quote += 25;
            }
            else if (insuree.CarYear > 2015)
            {
                quote += 25;
            }

            // Car make/model adjustments
            if (insuree.CarMake.Equals("Porsche", StringComparison.OrdinalIgnoreCase))
            {
                quote += 25;
                if (insuree.CarModel.Equals("911 Carrera", StringComparison.OrdinalIgnoreCase))
                {
                    quote += 25;
                }
            }

            // Speeding tickets adjustment
            quote += insuree.SpeedingTickets * 10;

            // DUI adjustment
            if (insuree.DUI)
            {
                quote *= 1.25m;
            }

            // Full coverage adjustment
            if (insuree.CoverageType.Equals("Full Coverage", StringComparison.OrdinalIgnoreCase))
            {
                quote *= 1.5m;
            }

            // Save the calculated quote
            insuree.Quote = quote;

            // Save to the database
            db.Insurees.Add(insuree);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Add an Admin view to display all quotes
        public ActionResult Admin()
        {
            var insurees = db.Insurees.ToList();
            return View(insurees);
        }
    }
}
