﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantReviewPlatformWithNLP.Application.Common.Models;
using RestaurantReviewPlatformWithNLP.Application.Common.Utility;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config) : IAuthService
    {
        // inject Identity Managers
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IConfiguration _config = config;
        private const int _expirationTokenHours = 12;

        public async Task<ResponseDTO<string>> GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
        {
            try
            {
                // create token handler instance
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"]);
                var issuer = _config["JwtSettings:Issuer"];
                var audience = _config["JwtSettings:Audience"];

                var claimList = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id),
                    new(JwtRegisteredClaimNames.Email, user.Email),
                    new(JwtRegisteredClaimNames.Name, user.FullName)
                };

                // adding roles to claim List
                claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                // signing credentials
                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    IssuedAt = DateTime.UtcNow,
                    Issuer = issuer,
                    Audience = audience,
                    Expires = DateTime.UtcNow.AddHours(_expirationTokenHours),
                    Subject = new ClaimsIdentity(claimList),
                    SigningCredentials = signingCredentials
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new ResponseDTO<string>(tokenString, "Token generated successfully!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<string>(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email,
                    loginModel.Password, false, false);

                if (!result.Succeeded)
                    return new ResponseDTO<string>("Email/Password is incorrect!");

                // getting this user and user roles
                var userFromDb = await _userManager.FindByEmailAsync(loginModel.Email)
                    ?? throw new Exception("User not found in our system!");

                var userRoles = await _userManager.GetRolesAsync(userFromDb);

                // create and return token
                var generatedToken = await GenerateJwtToken(userFromDb, userRoles);

                return new ResponseDTO<string>(generatedToken.Data, "Login successfully!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<string>(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                // create new ApplicationUser instance
                ApplicationUser user = new()
                {
                    Email = registerModel.Email,
                    FullName = registerModel.FullName,
                    UserName = registerModel.Email
                };

                // create and added to db user
                await _userManager.CreateAsync(user, registerModel.Password);

                // assign role
                await _userManager.AddToRoleAsync(user, SD.Role_User);

                return new ResponseDTO<string>(null, "Registration successful!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<string>(ex.Message);
            }
        }
    }
}
