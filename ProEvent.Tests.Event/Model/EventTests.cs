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
    public class EventTests
    {
        private readonly EventValidator _validator;

        public EventTests()
        {
            _validator = new EventValidator();
        }

        [Fact]
        public void Event_ValidData_ShouldPassValidation()
        {
            // Arrange
            var validEvent = new Event("Valid Event", "Description", DateTime.Now.AddDays(1), "Location", "Category", 100);

            // Act
            var result = _validator.TestValidate(validEvent);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Event_InvalidName_ShouldFailValidation()
        {
            // Arrange
            var invalidEvent = new Event("", "Description", DateTime.Now.AddDays(1), "Location", "Category", 100);

            // Act
            var result = _validator.TestValidate(invalidEvent);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.Name);
        }

        [Fact]
        public void Event_PastDate_ShouldFailValidation()
        {
            // Arrange
            var invalidEvent = new Event("Event", "Description", DateTime.Now.AddDays(-1), "Location", "Category", 100);

            // Act
            var result = _validator.TestValidate(invalidEvent);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.Date);
        }

        [Fact]
        public void Event_MaxParticipantsZero_ShouldFailValidation()
        {
            // Arrange
            var invalidEvent = new Event("Event", "Description", DateTime.Now.AddDays(1), "Location", "Category", 0);

            // Act
            var result = _validator.TestValidate(invalidEvent);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.MaxParticipants);
        }

    }
}