using FluentValidation;
using ProEvent.Domain.Models;

namespace ProEvent.Domain.Validators
{
    public class EnrollmentValidator : AbstractValidator<Enrollment>
    {
        public EnrollmentValidator()
        {
            RuleFor(e => e.EventId)
                .GreaterThan(0).WithMessage("EventId должен быть больше 0.");

            RuleFor(e => e.ParticipantId)
                .GreaterThan(0).WithMessage("ParticipantId должен быть больше 0.");

        }
    }
}