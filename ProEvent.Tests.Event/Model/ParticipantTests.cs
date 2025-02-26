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
    public class ParticipantTests
    {
        private readonly ParticipantValidator _validator;

        public ParticipantTests()
        {
            _validator = new ParticipantValidator();
        }

        [Fact]
        public void Participant_ValidData_ShouldPassValidation()
        {
            // Arrange
            var validParticipant = new Participant
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Email = "john.doe@example.com"
            };

            // Act
            var result = _validator.TestValidate(validParticipant);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Participant_InvalidFirstName_ShouldFailValidation()
        {
            // Arrange
            var invalidParticipant = new Participant
            {
                FirstName = "",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Email = "john.doe@example.com"
            };

            // Act
            var result = _validator.TestValidate(invalidParticipant);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.FirstName);
        }

        [Fact]
        public void Participant_InvalidLastName_ShouldFailValidation()
        {
            // Arrange
            var invalidParticipant = new Participant
            {
                FirstName = "John",
                LastName = "",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Email = "john.doe@example.com"
            };

            // Act
            var result = _validator.TestValidate(invalidParticipant);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.LastName);
        }

        [Fact]
        public void Participant_InvalidDateOfBirth_ShouldFailValidation()
        {
            // Arrange
            var invalidParticipant = new Participant
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-10), // Младше 16 лет
                Email = "john.doe@example.com"
            };

            // Act
            var result = _validator.TestValidate(invalidParticipant);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.DateOfBirth);
        }

        [Fact]
        public void Participant_InvalidEmail_ShouldFailValidation()
        {
            // Arrange
            var invalidParticipant = new Participant
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Email = "invalid-email"
            };

            // Act
            var result = _validator.TestValidate(invalidParticipant);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.Email);
        }

        [Fact]
        public void Participant_FirstNameTooLong_ShouldFailValidation()
        {
            // Arrange
            var invalidParticipant = new Participant
            {
                FirstName = new string('A', 51), // Более 50 символов
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Email = "john.doe@example.com"
            };

            // Act
            var result = _validator.TestValidate(invalidParticipant);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.FirstName);
        }

    }
}