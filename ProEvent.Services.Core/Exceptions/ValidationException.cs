using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(IEnumerable<ValidationFailure> failures) : base("Произошла одна или несколько ошибок проверки.")
        {
            Errors = failures.Select(e => e.ErrorMessage).ToList();
        }

        public ValidationException(string message) : base(message)
        {
            Errors = new List<string> { message }; 
        }

        public ValidationException(IEnumerable<string> errorMessages) : base("Произошла одна или несколько ошибок проверки.")
        {
            Errors = errorMessages.ToList();
        }
    }
}
