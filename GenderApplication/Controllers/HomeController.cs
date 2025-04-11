using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GenderApplication.Models;

namespace GenderApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
        // Here you would typically:
        // 1. Validate the credentials against your database
        // 2. Set up authentication cookies or tokens
        // 3. Redirect to the appropriate page based on user role
        
        // For now, we'll just redirect to the home page
        // In a real application, you would check credentials and handle errors
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewData["ErrorMessage"] = "Email and password are required.";
            return View();
        }
        
        // Simulate successful login
        TempData["SuccessMessage"] = "You have successfully logged in!";
        return RedirectToAction(nameof(Index));
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
    public IActionResult SignUp(string fullName, string email, string phone, string province, string city, string education, string workExperience, string careerOpportunities, string trainingCourse, string password, string confirmPassword, bool privacyAct, bool termsOfService)
    {
        if (!privacyAct || !termsOfService)
        {
            ModelState.AddModelError("", "You must accept both the Data Privacy Act and Terms of Service to proceed.");
            return View();
        }

        if (ModelState.IsValid)
        {
            // Here you would typically:
            // 1. Validate the input
            // 2. Check if the email is already registered
            // 3. Hash the password
            // 4. Save the user to the database
            // 5. Send a confirmation email
            
            // For now, we'll just redirect to the home page
            TempData["SuccessMessage"] = "Account created successfully! Please log in.";
            return RedirectToAction(nameof(Index));
        }
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
