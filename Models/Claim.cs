using System;
using System.Collections.Generic;

namespace CMCSPrototypeMVC.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public DateTime DateClaimed { get; set; }
        public int HourlyRate { get; set; }
        public int HoursWorked { get; set; }
        public string LecturerName { get; set; } // Add Lecturer Name here
        public string Notes { get; set; } // Any additional notes
        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>(); // List of supporting documents
        public string SubmittedBy { get; set; } // Who submitted the claim

        public int StatusId { get; set; }
        public ClaimStatus Status { get; set; }
        public int PersonId { get; set; }
        public decimal TotalPayment => HoursWorked * HourlyRate;
        public string NotificationMessage { get; set; }


        // Relationship



    }
    public class SupportingDocument
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

    
}
