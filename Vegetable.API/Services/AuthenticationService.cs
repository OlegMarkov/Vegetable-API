using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vegetable.API.Controllers;
using Vegetable.Core.Database;
using Vegetable.Entities;

namespace Vegetable.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IOwnerRepo _ownerRepo;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IOwnerRepo ownerRepo, IConfiguration configuration)
        {
            _ownerRepo = ownerRepo;
            _configuration = configuration;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = await _ownerRepo.GetUserByPhoneNumber(model.User.PhoneNumber);
            
            if (user == null)
            {
                var owner = new Owner
                {
                    TimeZone = model.TimeZone,
                    UserId = model.User.PhoneNumber,
                    Country = model.Country                   
                };

                // TODO: Logic to add 1 month free subscription.
                // Do we need to move the logic to the mobile app?
                owner.SubscriptionStartDate = DateTime.UtcNow;
                owner.SubscriptionEndDate =  DateTime.UtcNow.AddMonths(1);
                owner.SubscriptionTypeId = 1;
                owner.AllowSite = true;

                var newOwner = await _ownerRepo.CreateOwner(owner);
                model.User.OwnerId = newOwner.Id;
            }
            else
            {
                user.AllowNotifications = true;
                model.User.OwnerId = user.OwnerId;
            }
            user = await _ownerRepo.AddUser(model.User.OwnerId, model.User);
            var token = GenerateJwtToken(user);
            return new AuthenticateResponse(user, token);
        }


        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 10 years
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.OwnerId.ToString()), new Claim("userId", user.PhoneNumber) }),
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public class AuthenticateResponse
        {
            public User User { get; set; }
            public string Token { get; set; }

            public AuthenticateResponse(User user, string token)
            {
                User = user;
                Token = token;
            }
        }
    }
}
