using FluentValidation;
using ProEvent.Domain.Models;

namespace ProEvent.BLL.Validators
{
    public class ParticipantValidator : AbstractValidator<Participant>
    {
        public ParticipantValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("Имя участника обязательно.")
                .MaximumLength(50).WithMessage("Имя участника не должно превышать 50 символов.");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("Фамилия участника обязательна.")
                .MaximumLength(50).WithMessage("Фамилия участника не должна превышать 50 символов.");

            RuleFor(p => p.DateOfBirth)
                .NotEmpty().WithMessage("Дата рождения участника обязательна.")
                .LessThan(DateTime.Now.AddYears(-16)).WithMessage("Участнику должно быть не менее 16 лет.");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Email участника обязателен.")
                .EmailAddress().WithMessage("Неверный формат Email.");
        }
    }
}
