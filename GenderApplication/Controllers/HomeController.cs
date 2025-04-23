using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GenderApplication.Models;
using GenderApplication.Services;
using GenderApplication.Data;

namespace GenderApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public HomeController(
        ILogger<HomeController> logger,
        IPasswordHasher passwordHasher,
        ApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Location()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Assessment()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid verification link.";
            return RedirectToAction(nameof(Login));
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Login));
        }

        if (user.IsEmailVerified)
        {
            TempData["SuccessMessage"] = "Email already verified. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        if (user.EmailVerificationToken != token)
        {
            TempData["ErrorMessage"] = "Invalid verification token.";
            return RedirectToAction(nameof(Login));
        }

        if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
        {
            TempData["ErrorMessage"] = "Verification link has expired. Please request a new one.";
            return RedirectToAction(nameof(Login));
        }

        user.IsEmailVerified = true;
        user.IsActive = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Email verified successfully! You can now log in.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult SignUp()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password, bool rememberMe)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewData["ErrorMessage"] = "Email and password are required.";
            return View();
        }

        // Find user by email
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            ViewData["ErrorMessage"] = "Invalid email or password.";
            return View();
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.Password))
        {
            ViewData["ErrorMessage"] = "Invalid email or password.";
            return View();
        }

        // Check if email is verified
        if (!user.IsEmailVerified)
        {
            ViewData["ErrorMessage"] = "Please verify your email address before logging in.";
            return View();
        }

        // Check if user is active
        if (!user.IsActive)
        {
            ViewData["ErrorMessage"] = "This account has been deactivated.";
            return View();
        }

        // TODO: Set up authentication cookie/session
        TempData["SuccessMessage"] = "You have successfully logged in!";
        return RedirectToAction(nameof(Assessment));
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewData["ErrorMessage"] = "Email address is required.";
            return View();
        }
        
        // Here you would typically:
        // 1. Check if the email exists in your database
        // 2. Generate a password reset token
        // 3. Send an email with the reset link
        // 4. Store the token in your database with an expiration time
        
        // For now, we'll just show a success message
        ViewData["SuccessMessage"] = "If your email is registered with us, you will receive password reset instructions shortly.";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(string fullName, string email, string phone, string province, string education, string department, string workExperience, string password, string confirmPassword, bool privacyAct, bool termsOfService)
    {
        if (!privacyAct || !termsOfService)
        {
            ModelState.AddModelError("", "You must accept both the Data Privacy Act and Terms of Service to proceed.");
            return View();
        }

        if (string.IsNullOrEmpty(password) || password != confirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match or are empty.");
            return View();
        }

        // Check if email is already registered
        if (_context.Users.Any(u => u.Email == email))
        {
            ModelState.AddModelError("", "This email is already registered.");
            return View();
        }

        if (ModelState.IsValid)
        {
            // Generate verification token
            string verificationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            // Create new user with hashed password
            var user = new User
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Province = province,
                Education = education,
                Department = department,
                WorkExperience = workExperience,
                Password = _passwordHasher.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            // Save user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate verification link
            var verificationLink = Url.Action(
                "VerifyEmail",
                "Home",
                new { email = user.Email, token = verificationToken },
                protocol: HttpContext.Request.Scheme);

            // Send verification email
            await _emailService.SendVerificationEmailAsync(user.Email, verificationLink);
            
            TempData["SuccessMessage"] = "Account created successfully! Please check your email to verify your account.";
            return RedirectToAction(nameof(Assessment));
        }
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
