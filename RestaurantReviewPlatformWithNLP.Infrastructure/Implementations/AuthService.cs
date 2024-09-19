using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantReviewPlatformWithNLP.Application.Common.Models;
using RestaurantReviewPlatformWithNLP.Application.Common.Utility;
using RestaurantReviewPlatformWithNLP.Application.Services;
using RestaurantReviewPlatformWithNLP.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class AuthService : IAuthService
    {
        // inject Identity Managers
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private const int _expirationTokenHours = 12;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
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
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name, user.FullName)
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
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email,
                    loginModel.Password, false, false);

                if (!result.Succeeded)
                    throw new Exception("Email/Password is incorrect!");

                // getting this user and user roles
                var userFromDb = await _userManager.FindByEmailAsync(loginModel.Email)
                    ?? throw new Exception("User not found in our system!");

                var userRoles = await _userManager.GetRolesAsync(userFromDb);

                // create token
                return await GenerateJwtToken(userFromDb, userRoles);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task RegisterAsync(RegisterModel registerModel)
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
