using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Core.Validators;
using ProEvent.Services.Identity.DTOs;
using ProEvent.Services.Identity.Interfeces;
using ProEvent.Services.Identity.Models;
using ProEvent.Services.Identity.Repository;
using ProEvent.Services.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProEvent.Services.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<Participant> _participantValidator;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            ITokenService tokenService,
            ApplicationDbContext dbContext,
            IValidator<Participant> participantValidator)
        {
            _authenticationRepository = authenticationRepository;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _participantValidator = participantValidator;
        }

        public async Task<AuthenticationResponseDTO> RegisterUser(UserRegistrationModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var existingUserWithEmail = await _authenticationRepository.FindUserByEmailAsync(model.Email, cancellationToken);
            if (existingUserWithEmail != null)
            {
                throw new ArgumentException("Пользователь с таким email уже зарегистрирован.");
            }

            var existingUserWithUsername = await _authenticationRepository.FindUserByNameAsync(model.UserName, cancellationToken);
            if (existingUserWithUsername != null)
            {
                throw new ArgumentException("Логин пользователя не уникален.");
            }

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await _authenticationRepository.CreateUserAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"Ошибки регистрации: {errors}");
            }

            var participant = new Participant
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                UserId = user.Id
            };

            var participantValidationResult = await _participantValidator.ValidateAsync(participant, cancellationToken);
            if (!participantValidationResult.IsValid)
            {
                await _authenticationRepository.DeleteUserAsync(user);
                var errors = participantValidationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(" ", errors));
            }

            _dbContext.Participants.Add(participant);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                await _authenticationRepository.DeleteUserAsync(user);
                throw new ArgumentException("Ошибка при регистрации.", ex);
            }

            await _authenticationRepository.AddToRoleAsync(user, "User");
            return await GenerateAuthenticationResponse(user);
        }

        public async Task<AuthenticationResponseDTO> LoginUser(UserLoginModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Неверный формат данных.");
            }

            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Требуется имя пользователя и пароль.");
            }

            var result = await _authenticationRepository.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ArgumentException("Неверный логин или пароль.");
            }

            var user = await _authenticationRepository.FindUserByNameAsync(model.UserName, cancellationToken);
            if (user == null)
            {
                throw new Exception("Пользователь не найден после успешного входа в систему.");
            }
            return await GenerateAuthenticationResponse(user);
        }

        public async Task<bool> LogoutUser(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _authenticationRepository.SignOutAsync();
            return true;
        }

        private async Task<AuthenticationResponseDTO> GenerateAuthenticationResponse(ApplicationUser user)
        {
            var token = await _tokenService.GenerateToken(user);
            var role = await _authenticationRepository.GetRoleAsync(user).ConfigureAwait(false);
            return new AuthenticationResponseDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
            };
        }
    }
}