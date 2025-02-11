using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProEvent.Services.Core.Models;
using ProEvent.Services.Identity.DTOs;
using ProEvent.Services.Identity.Interfeces;
using ProEvent.Services.Identity.Models;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _dbContext; // Добавьте зависимость от DbContext

        public AuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _dbContext = dbContext;
        }

        public async Task<AuthenticationResponseDTO> RegisterUser(UserRegistrationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Registration model cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Username, email, and password are required.");
            }

            // Проверка уникальности Email
            var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserWithEmail != null)
            {
                throw new ArgumentException("Email is already registered.");
            }

            // Проверка уникальности Username
            var existingUserWithUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserWithUsername != null)
            {
                throw new ArgumentException("Username is already taken.");
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Создаем Participant
                var participant = new Participant
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email,
                    UserId = user.Id // Связываем с новым пользователем
                };

                _dbContext.Participants.Add(participant);
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Обработка ошибок при сохранении в базу данных, например, нарушение ограничений
                    //  или другие проблемы с БД
                    //  Удаляем пользователя, если не удалось создать Participant, чтобы не было inconsistencies.
                    await _userManager.DeleteAsync(user);
                    throw new Exception("Failed to create participant.", ex);  // Rethrow exception, possibly wrap in custom exception
                }
                await _userManager.AddToRoleAsync(user, "User");
                return await GenerateAuthenticationResponse(user);
            }
            else
            {
                // Обработка ошибок регистрации
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }
        }

        public async Task<AuthenticationResponseDTO> LoginUser(UserLoginModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Неверный формат данных.");
            }

            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Требуется имя пользователя и пароль.");
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    throw new Exception("Пользователь не найден после успешного входа в систему."); // Дополнительная проверка
                }
                return await GenerateAuthenticationResponse(user);
            }
            else
            {
                // Обработка ошибок входа
                throw new Exception("Пользователя с таким логином не существует.");
            }
        }

        public async Task<bool> LogoutUser()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        private async Task<AuthenticationResponseDTO> GenerateAuthenticationResponse(ApplicationUser user)
        {
            var token = _tokenService.GenerateToken(user);
            var role =  _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            return new AuthenticationResponseDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
        
            };
        }
    } }