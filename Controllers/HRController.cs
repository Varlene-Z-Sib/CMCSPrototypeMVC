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
            return View(lecturers); // lecturers is a List<Lecturer>, which can be treated as IEnumerable<Lecturer>
        }


        // GET: /HR/HRDashboard
        public IActionResult HRDashboard()
        {
            ViewBag.TotalLecturers = lecturers.Count;
            ViewBag.TotalClaims = claims.Count;
            ViewBag.ApprovedClaims = claims.Count(c => c.StatusId == 2); // Approved claims
            ViewBag.PendingClaims = claims.Count(c => c.StatusId == 3); // Pending claims

            return View(); // HR Dashboard view
        }

        // POST: /HR/GenerateReport
        
            [HttpGet]
            public IActionResult GenerateReports()
            {
                return View();  // Display the report generation form or page
            }

            // POST: /HR/GenerateReports
            [HttpPost]
        public IActionResult GenerateReport()
        {
            // Generate a list of approved claims (StatusId == 2 is "Approved")
            var approvedClaims = claims.Where(c => c.StatusId == 2).ToList();

            // Check if there are any approved claims
            if (!approvedClaims.Any())
            {
                TempData["ErrorMessage"] = "No approved claims found.";
                return RedirectToAction("HRDashboard");
            }

            // Prepare the report
            var report = approvedClaims.Select(c => new
            {
                c.Id,
                c.DateClaimed,
                c.LecturerName,
                c.HoursWorked,
                c.HourlyRate,
                Status = "Approved",  // Hardcoded as "Approved" since we filtered by this status
                c.TotalPayment,
                SupportingDocuments = c.SupportingDocuments.Select(doc => doc.FileName).ToList()
            }).ToList();

            // Store the report in TempData or ViewData for use in the view
            ViewData["Report"] = report;

            return View(); // Render the report view
        }
        public IActionResult DownloadReport()
        {
            // Get approved claims from the existing list
            var approvedClaims = claims.Where(c => c.StatusId == 2).ToList();

            if (!approvedClaims.Any())
            {
                return NotFound("No approved claims found.");
            }

            // Generate CSV content
            var csvContent = "Claim ID,Date Claimed,Lecturer Name,Hours Worked,Hourly Rate,Status,Total Payment,Supporting Documents\n";

            foreach (var claim in approvedClaims)
            {
                var documentNames = claim.SupportingDocuments.Any() ? string.Join(", ", claim.SupportingDocuments.Select(doc => doc.FileName)) : "No Documents";
                csvContent += $"{claim.Id},{claim.DateClaimed:MM/dd/yyyy},{claim.LecturerName},{claim.HoursWorked},{claim.HourlyRate:C},Approved,{claim.TotalPayment:C},{documentNames}\n";
            }

            // Return the CSV file as a downloadable response
            var fileName = "ApprovedClaimsReport.csv";
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(fileBytes, "text/csv", fileName);
        }
        public IActionResult AddLecturer()
        {
            return View(); // Display the Add Lecturer form
        }

        // POST: /HR/AddLecturer
        [HttpPost]
        public IActionResult AddLecturer(string name, string contactInfo)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contactInfo))
            {
                TempData["ErrorMessage"] = "Both name and contact information are required.";
                return RedirectToAction("AddLecturer");
            }

            var newLecturer = new Lecturer
            {
                Id = lecturers.Count + 1, // Assign a new ID (in a real-world scenario, this would be handled by a database)
                Name = name,
                ContactInfo = contactInfo
            };

            lecturers.Add(newLecturer); // Add the new lecturer to the list

            TempData["SuccessMessage"] = "Lecturer added successfully!";
            return RedirectToAction("ManageLecturers"); // Redirect to ManageLecturers view after successful addition
        }
        public IActionResult DeleteLecturer(int id)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }

            lecturers.Remove(lecturer); // Remove the lecturer from the list

            TempData["SuccessMessage"] = "Lecturer deleted successfully.";
            return RedirectToAction("ManageLecturers"); // Redirect back to ManageLecturers view
        }
        public IActionResult EditLecturer(int id)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers"); // If the lecturer doesn't exist, go back to ManageLecturers
            }

            return View(lecturer); // Pass the lecturer details to the EditLecturer view
        }

        // POST: /HR/EditLecturer/{id}
        [HttpPost]
        public IActionResult EditLecturer(int id, string name, string contactInfo)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers"); // If the lecturer doesn't exist, go back to ManageLecturers
            }

            // Validation: Ensure name and contact info are provided
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contactInfo))
            {
                TempData["ErrorMessage"] = "Both name and contact information are required.";
                return RedirectToAction("EditLecturer", new { id = id }); // Return to the edit form if validation fails
            }

            // Update the lecturer's information
            lecturer.Name = name;
            lecturer.ContactInfo = contactInfo;

            TempData["SuccessMessage"] = "Lecturer updated successfully!";
            return RedirectToAction("ManageLecturers"); // Redirect to the ManageLecturers page after successful update
        }
    }
}



      

