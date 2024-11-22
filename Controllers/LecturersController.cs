using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Models;
using System.Collections.Generic;
using System.Linq;

namespace YourProjectNamespace.Controllers
{
    public class LecturersController : Controller
    {
        private static List<Lecturer> lecturers = new List<Lecturer>
        {
            new Lecturer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123456789", Address = "123 Elm Street" },
            new Lecturer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "987654321", Address = "456 Oak Street" }
        };

        [Authorize(Policy = "HR")]
        public IActionResult HRDashboard()
        {
            return View();
        }

        [Authorize(Policy = "Lecturer")]
        public IActionResult UpdateLecturerDetails()
        {
            var currentLecturerId = 1; // Replace with logged-in lecturer ID logic
            var lecturer = lecturers.FirstOrDefault(l => l.Id == currentLecturerId);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("Index", "Home");
            }
            return View(lecturer);
        }

        [HttpPost]
        [Authorize(Policy = "Lecturer")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLecturerDetails(Lecturer updatedLecturer)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == updatedLecturer.Id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("Index", "Home");
            }

            lecturer.Name = updatedLecturer.Name;
            lecturer.Email = updatedLecturer.Email;
            lecturer.Phone = updatedLecturer.Phone;
            lecturer.Address = updatedLecturer.Address;

            TempData["SuccessMessage"] = "Details updated successfully.";
            return RedirectToAction("UpdateLecturerDetails");
        }

        [Authorize(Policy = "HR")]
        public IActionResult ManageLecturers()
        {
            return View(lecturers);
        }

        [Authorize(Policy = "HR")]
        public IActionResult EditLecturer(int id)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }
            return View(lecturer);
        }

        [HttpPost]
        [Authorize(Policy = "HR")]
        [ValidateAntiForgeryToken]
        public IActionResult EditLecturer(Lecturer updatedLecturer)
        {
            var lecturer = lecturers.FirstOrDefault(l => l.Id == updatedLecturer.Id);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }

            lecturer.Name = updatedLecturer.Name;
            lecturer.Email = updatedLecturer.Email;
            lecturer.Phone = updatedLecturer.Phone;
            lecturer.Address = updatedLecturer.Address;

            TempData["SuccessMessage"] = "Lecturer information updated successfully.";
            return RedirectToAction("ManageLecturers");
        }

        [Authorize(Policy = "HR")]
        public IActionResult AddLecturer()
        {
            return View(new Lecturer());
        }

        [HttpPost]
        [Authorize(Policy = "HR")]
        [ValidateAntiForgeryToken]
        public IActionResult AddLecturer(Lecturer newLecturer)
        {
            newLecturer.Id = lecturers.Count + 1;
            lecturers.Add(newLecturer);

            TempData["SuccessMessage"] = "Lecturer added successfully.";
            return RedirectToAction("ManageLecturers");
        }
    }
}
