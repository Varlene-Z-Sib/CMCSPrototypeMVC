using CMCSPrototypeMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMCSPrototypeMVC.Controllers
{
    [Authorize(Policy = "Admin")] // Only Admins can access HR operations
    public class HRController : Controller
    {
        // Static lists to simulate data storage
        private static List<Claim> claims = ClaimsController.claims; // Reuse existing claims list
        private static List<Lecturer> lecturers = new List<Lecturer>(); // Store lecturers in memory

        // GET: /HR/ViewClaims
        public IActionResult ViewClaims()
        {
            return View(claims); // Display claims for HR processing
        }

        // POST: /HR/UpdateLecturer
        [HttpPost]
        public IActionResult UpdateLecturer(int id, string name, string contact)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }

            lecturer.Name = name;
            lecturer.ContactInfo = contact;
            TempData["SuccessMessage"] = "Lecturer information updated successfully.";
            return RedirectToAction("ManageLecturers");
        }

        // GET: /HR/ManageLecturers
        public IActionResult ManageLecturers()
        {
            return View(lecturers); // Display list of lecturers
        }

        // POST: /HR/GenerateReport
        [HttpPost]
        public IActionResult GenerateReport()
        {
            // Generate a simple summary report
            var report = claims.Where(c => c.StatusId == 2) // Approved claims
                               .Select(c => new
                               {
                                   c.LecturerName,
                                   TotalPayment = c.TotalPayment.ToString("C")
                               })
                               .ToList();

            TempData["ReportMessage"] = "Report generated successfully.";
            return Json(report);
        }
    }

    // Lecturer model
    public class Lecturer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
    }
}
