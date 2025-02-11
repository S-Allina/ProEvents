using FluentValidation;
using ProEvent.Services.Core.Models;

namespace ProEvent.Services.Core.Validators
{
    public class EventValidator : AbstractValidator<Event>
    {
        public EventValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Название события обязательно.")
                .MaximumLength(70).WithMessage("Название события не должно превышать 70 символов.");

            RuleFor(e => e.Description)
                .NotEmpty().WithMessage("Описание события обязательно.");

            RuleFor(e => e.Date)
                .NotEmpty().WithMessage("Дата и время события обязательны.")
                .GreaterThan(DateTime.Now).WithMessage("Дата события не может быть в прошлом.");

            RuleFor(e => e.Location)
                .NotEmpty().WithMessage("Место проведения события обязательно.")
                .MaximumLength(70).WithMessage("Место проведения события не должно превышать 70 символов.");

            RuleFor(e => e.Category)
                .NotEmpty().WithMessage("Категория события обязательна.")
                .MaximumLength(50).WithMessage("Категория события не должна превышать 50 символов.");

            RuleFor(e => e.MaxParticipants)
                .GreaterThan(0).WithMessage("Максимальное количество участников должно быть больше 0.");
            RuleFor(e => e.MaxParticipants)
                       .GreaterThan(0).WithMessage("Максимальное количество участников должно быть больше 0.")
                       .LessThanOrEqualTo(3000).WithMessage("Максимальное количество участников не должно превышать 3000."); // New rule!

        }
    }
}
