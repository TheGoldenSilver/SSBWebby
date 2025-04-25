using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GenderApplication.Models;
using GenderApplication.Services;
using GenderApplication.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        // Show welcome message if present
        if (TempData["WelcomeMessage"] != null)
        {
            ViewData["WelcomeMessage"] = TempData["WelcomeMessage"];
        }

        // Check if user is authenticated
        if (User.Identity?.IsAuthenticated != true)
        {
            TempData["ErrorMessage"] = "Please log in to access the assessment.";
            return RedirectToAction(nameof(Login));
        }

        // Get the user's email from claims
        var userEmail = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

        if (user != null && !user.IsEmailVerified)
        {
            ViewData["VerificationMessage"] = "Please verify your email to unlock all features.";
        }

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

        // Set up authentication cookie/session
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("FullName", user.FullName)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4)
        };
        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties).Wait();
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
    public async Task<IActionResult> SignUp(string fullName, string email, string phone, string province, string city, string education, string workExperience, string careerOpportunities, string trainingCourse, string password, string confirmPassword, bool privacyAct, bool termsOfService)
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
            // Immediately verify and activate the user (no email verification)
            var user = new User
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Province = province,
                City = city,
                Education = education,

                WorkExperience = workExperience,
                CareerOpportunities = careerOpportunities,
                TrainingCourse = trainingCourse,
                Password = _passwordHasher.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailVerified = true,
                EmailVerificationToken = null,
                EmailVerificationTokenExpiry = null
            };

            // Save user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Auto-login after sign-up
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", user.FullName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["WelcomeMessage"] = $"Welcome, {user.FullName}! Your account has been created.";
            return RedirectToAction(nameof(Assessment));
        }
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAssessment([FromBody] AssessmentDto dto)
    {
        if (dto?.SoftSkills == null || dto?.TechSkills == null || dto.SoftSkills.Length != 5 || dto.TechSkills.Length != 5)
            return BadRequest("Invalid data");
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized();
        var response = new Models.AssessmentResponse
        {
            UserEmail = userEmail,
            SoftSkills = System.Text.Json.JsonSerializer.Serialize(dto.SoftSkills),
            TechSkills = System.Text.Json.JsonSerializer.Serialize(dto.TechSkills),
            SubmittedAt = DateTime.UtcNow
        };
        _context.AssessmentResponses.Add(response);
        await _context.SaveChangesAsync();
        return Ok();
    }

    public class AssessmentDto
    {
        public int[] SoftSkills { get; set; }
        public int[] TechSkills { get; set; }
    }
}
