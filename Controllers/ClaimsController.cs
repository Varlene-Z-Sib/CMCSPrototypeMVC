using CMCSPrototypeMVC.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMCSPrototypeMVC.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private static List<Claim> claims = new List<Claim>(); // Store claims in memory for now
        private readonly IValidator<Claim> _claimValidator;

        public ClaimsController(IValidator<Claim> claimValidator)
        {
            _claimValidator = claimValidator;
        }

        // GET: /Claims/SubmitClaim
        [Authorize(Policy = "Lecturer")]
        public IActionResult SubmitClaim()
        {
            return View(new Claim());
        }

        // POST: /Claims/SubmitClaim
        [Authorize(Policy = "Lecturer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(Claim claim, List<IFormFile> documents)
        {
            try
            {
                // Validate claim using FluentValidation
                var validationResult = _claimValidator.Validate(claim);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    TempData["ErrorMessage"] = "Validation failed. Please correct the highlighted errors.";
                    return View(claim);
                }

                // Handle document uploads
                if (documents != null && documents.Any())
                {
                    foreach (var document in documents)
                    {
                        if (document.Length > 0 &&
                            (Path.GetExtension(document.FileName).ToLower() == ".pdf" ||
                             Path.GetExtension(document.FileName).ToLower() == ".docx" ||
                             Path.GetExtension(document.FileName).ToLower() == ".xlsx"))
                        {
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            var filePath = Path.Combine(uploadsFolder, document.FileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                document.CopyTo(stream);
                            }

                            claim.SupportingDocuments.Add(new SupportingDocument
                            {
                                FileName = document.FileName,
                                FilePath = "/uploads/" + document.FileName
                            });
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Unsupported file type: {document.FileName}";
                        }
                    }
                }

                // Set initial status
                claim.StatusId = VerifyClaim(claim) ? 2 : 3; // 2 = Approved, 3 = Pending
                claim.Id = claims.Count + 1; // Simulate auto-increment ID
                claim.DateClaimed = DateTime.Now;
                claim.SubmittedBy = User.Identity.Name; // Capture username
                claims.Add(claim);

                TempData["SuccessMessage"] = "Your claim was submitted successfully.";
                return RedirectToAction("ClaimSubmitted");
            }
            catch (Exception ex)
            {
                // Log exception (use a logging library in a real project)
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(claim);
            }
        }


        // GET: /Claims/ClaimSubmitted
        [Authorize(Policy = "Lecturer")]
        public IActionResult ClaimSubmitted()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }

        // GET: /Claims/ReviewClaims
        [Authorize(Policy = "Admin")]
        public IActionResult ReviewClaims()
        {
            return View(claims); // Return the list of claims for admin review
        }

        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.StatusId = 2; // Set to Approved
                claim.NotificationMessage = $"Your claim (ID: {claim.Id}) has been approved. Total Payment: {claim.TotalPayment:C}.";
            }
            return RedirectToAction("ReviewClaims");
        }

        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.StatusId = 1; // Set to Rejected
                claim.NotificationMessage = $"Your claim (ID: {claim.Id}) has been rejected. Please review and resubmit if necessary.";
            }
            return RedirectToAction("ReviewClaims");
        }
        [Authorize(Policy = "Lecturer")]
        public IActionResult Notification()
        {
            var userClaims = claims
                .Where(c => c.SubmittedBy == User.Identity.Name)
                .ToList(); // Ensure it's a List<Claim>

            if (!userClaims.Any())
            {
                TempData["InfoMessage"] = "You have no submitted claims.";
            }

            return View(userClaims);
        }




        // Automated claim verification logic
        private bool VerifyClaim(Claim claim)
        {
            // Customizable rules for automated verification
            return claim.HoursWorked > 0 &&
                   claim.HoursWorked <= 40 &&
                   claim.HourlyRate >= 50 &&
                   claim.HourlyRate <= 200;
        }
    }
}
