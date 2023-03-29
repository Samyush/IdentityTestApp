using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityTestApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IdentityTestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager; 

        private readonly SignInManager<IdentityUser> _signInManager;

        //private readonly IMessageService _messageService;
        
        private readonly IEmailSender _emailSender;
        
        private readonly IConfiguration _configuration;
        
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager,
            //IMessageService messageService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            //_messageService = messageService;
            _emailSender = emailSender;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("create_role")]
        public async Task<IActionResult> CreateRole(string userRole)
        {
            var role = new IdentityRole { Name = userRole };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                // role created successfully
                return Ok();
            }
            else
            {
                // role creation failed
                return BadRequest(result.Errors);
            }
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new IdentityUser {UserName = model.Username, Email = model.Email};
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new {message = "Registration successful"});
        }

        #region register_admin

        [HttpPost("register_admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterDto model)
        {
            var user = new IdentityUser {UserName = model.Username, Email = model.Email};
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the user to the "Administrator" role
                await _userManager.AddToRoleAsync(user, "Admin");

                // ...
            }
            else if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var token = GenerateJwtToken(user);

            return Ok(new {token});
        }
        
        #endregion

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result =
                await _signInManager.PasswordSignInAsync(model.Username, model.Password, false,
                    lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return BadRequest(new {message = "Invalid username or password"});
            }

            return Ok("Login successful");
        }

        #region LoginWithJwt

        [HttpPost("login_with_jwt")]
        public async Task<IActionResult> LoginJwt(LoginDto model)
        {
            var result =
                await _signInManager.PasswordSignInAsync(model.Username, model.Password, false,
                    lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return BadRequest(new {message = "Invalid username or password"});
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            var token = GenerateJwtToken(user);

            return Ok(new {token});

        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:ValidIssuer"],
                _configuration["Jwt:ValidIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        
        
        // implementation of email service
         
        #region ValidateEmail

        [HttpPost("validate_email")]
        [Authorize]
        public async Task<IActionResult> ValidateEmail()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && !currentUser.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
                var confirmationLink = Url.Action("ConfirmEmail", "Auth",
                    new {token, email = currentUser.Email}, Request.Scheme);
               // var message = new Message(new string[] {currentUser.Email}, "Confirm your email",
                 //   confirmationLink, null);
                await _emailSender.SendEmailAsync(currentUser.Email, "Confirm your email", confirmationLink);
                 //_userManager.ConfirmEmailAsync(confirmationLink, token);
                //await _messageService.SendEmailAsync(message);
                
                return Ok($"see email for confirmation link (mail sent to {currentUser.Email})");
            }

            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            // Other code...

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{email}':");
            }

            // Set EmailConfirmed to true and save changes
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Other code...
            return Ok($"Email confirmed successfully Threat has been detected, please hide your email address password from github /n {user.Email}");
        }


        #endregion
        

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful");
        }
    }

}