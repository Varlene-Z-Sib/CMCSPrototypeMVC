namespace CMCSPrototypeMVC.Validators
{
    using FluentValidation;
    using CMCSPrototypeMVC.Models;

    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator()
        {
            RuleFor(c => c.HoursWorked)
                .GreaterThan(0).WithMessage("Hours worked must be greater than zero.")
                .LessThanOrEqualTo(40).WithMessage("Hours worked must not exceed 40.");

            RuleFor(c => c.HourlyRate)
                .GreaterThanOrEqualTo(50).WithMessage("Hourly rate must be at least $50.")
                .LessThanOrEqualTo(200).WithMessage("Hourly rate must not exceed $200.");

            RuleFor(c => c.LecturerName)
                .NotEmpty().WithMessage("Lecturer name is required.");
        }
    }
}
