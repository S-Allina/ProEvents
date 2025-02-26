using FluentValidation.TestHelper;
using ProEvent.Domain.Models;
using ProEvent.Domain.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Tests.Events.Model
{
    public class EnrollmentTests
    {
        private readonly EnrollmentValidator _validator;

        public EnrollmentTests()
        {
            _validator = new EnrollmentValidator();
        }

        [Fact]
        public void Enrollment_ValidData_ShouldPassValidation()
        {
            // Arrange
            var validEnrollment = new Enrollment
            {
                EventId = 1,
                ParticipantId = 1,
                RegistrationDate = DateTime.Now
            };

            // Act
            var result = _validator.TestValidate(validEnrollment);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Enrollment_InvalidEventId_ShouldFailValidation()
        {
            // Arrange
            var invalidEnrollment = new Enrollment
            {
                EventId = 0,
                ParticipantId = 1,
                RegistrationDate = DateTime.Now
            };

            // Act
            var result = _validator.TestValidate(invalidEnrollment);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.EventId);
        }

        [Fact]
        public void Enrollment_InvalidParticipantId_ShouldFailValidation()
        {
            // Arrange
            var invalidEnrollment = new Enrollment
            {
                EventId = 1,
                ParticipantId = 0,
                RegistrationDate = DateTime.Now
            };

            // Act
            var result = _validator.TestValidate(invalidEnrollment);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.ParticipantId);
        }

        [Fact]
        public void Enrollment_FutureRegistrationDate_ShouldPassValidation()
        {
            // Arrange
            var validEnrollment = new Enrollment
            {
                EventId = 1,
                ParticipantId = 1,
                RegistrationDate = DateTime.Now.AddDays(1) // Future date
            };

            // Act
            var result = _validator.TestValidate(validEnrollment);

            // Assert
            result.ShouldNotHaveAnyValidationErrors(); // No validation rule for RegistrationDate, so should pass
        }

        [Fact]
        public void Enrollment_PastRegistrationDate_ShouldPassValidation()
        {
            // Arrange
            var validEnrollment = new Enrollment
            {
                EventId = 1,
                ParticipantId = 1,
                RegistrationDate = DateTime.Now.AddDays(-1) // Past date
            };

            // Act
            var result = _validator.TestValidate(validEnrollment);

            // Assert
            result.ShouldNotHaveAnyValidationErrors(); // No validation rule for RegistrationDate, so should pass
        }
    }
}