using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared.DTOs;
using LibraryEcommerceWebApi.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDataProtector _dataProtector;
        private readonly GenerateToken _generateToken;
        private readonly IEmailSender _emailSender;

        public AccountController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IDataProtectionProvider dataProtectionProvider,
            GenerateToken generateToken,
            IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _dataProtector = dataProtectionProvider.CreateProtector("EmailConfirmationToken");
            _generateToken = generateToken;
            _emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(registerDto);

                if (await _unitOfWork.Users.GetByEmailAsync(registerDto.Email) != null)
                    return BadRequest($"This email is already registered: {registerDto.Email}");

                string imageName = $"{Guid.NewGuid()}{Path.GetExtension(registerDto.FormFile.FileName)}";
                string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");

                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);

                string fullPath = Path.Combine(imageDirectory, imageName);
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                    await registerDto.FormFile.CopyToAsync(stream);

                user.ImagePath = Path.Combine("assets", "images", imageName);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate Email Confirmation Token
                var token = _dataProtector.Protect(user.Id);
                var confirmationLink = Url.Action(
                    nameof(ConfirmEmail), "Account",
                    new { token },
                    protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(registerDto.Email, "Confirm your email",
                    $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");

                return Ok(user);
            }
            return BadRequest();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var userId = _dataProtector.Unprotect(token);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return BadRequest(new { message = "Invalid user" });

            if (user.IsConfirmed == true)
                return BadRequest(new { message = "Email is already confirmed" });

            user.IsConfirmed = true;
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "Email confirmed successfully. You can now log in." });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input data" });

            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
            if (user == null || user.IsDeleted == true)
                return Unauthorized(new { message = "User not found or deleted" });

            if (user.IsConfirmed == false)
                return BadRequest(new { message = "User is not confirmed" });

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Incorrect password" });

            var response = _generateToken.GenerateJWTToken(user);
            return Ok(response);
        }

        [HttpPut]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            if (forgetPasswordDto.Email == null)
            {
                ModelState.AddModelError("MissingValue", "Enter your email address");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input data" });

            var user = await _unitOfWork.Users.GetByEmailAsync(forgetPasswordDto.Email);
            if (user == null || user.IsDeleted == true || user.IsConfirmed == false)
                return BadRequest(new { message = "invalid email address" });

            // send confirmation email
            var token = _dataProtector.Protect(user.Id);
            var confirmationLink = Url.Action(
                nameof(ChangePassword), "Account", new { token, forgetPasswordDto.NewPassword },
                protocol: HttpContext.Request.Scheme
            );

            await _emailSender.SendEmailAsync(user.Email, "Forget Password",
                $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>"
            );
            return Ok();
        }

        [HttpGet("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string token, string NewPassword)
        {
            var userId = _dataProtector!.Unprotect(token);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("Error", "Invalid user");
                return BadRequest();
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "Password changes successfully. You can now log in." });
        }


    }
}
