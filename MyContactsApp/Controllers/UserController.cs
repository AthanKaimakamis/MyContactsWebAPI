using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyContactsApp.Models;
using MyContactsApp.Services;

namespace MyContactsApp.Controllers;

public class UserController : Controller
{
    
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    // GET-Login
    [HttpGet("login")]
    public IActionResult Login(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    // GET-login_external
    [HttpGet("login/{provider}")]
    public IActionResult LoginExternal([FromRoute] string provider, [FromQuery] string returnUrl)
    {
        if (User != null && User.Identities.Any(identity => identity.IsAuthenticated))
        {
            return RedirectToAction("Index", "Contact");
        }

        returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
        var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };

        return new ChallengeResult(provider, authenticationProperties);
    }
    
    // POST-Login
    [Route("validate"),HttpPost("login")]
    public async Task<IActionResult> Validate(string email, string password, string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (_userService.TryValidateUser(email, password, out List<Claim> claims))
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var items = new Dictionary<string, string>();
            items.Add(".AuthScheme", CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties(items);
            await HttpContext.SignInAsync(claimsPrincipal, properties);
            
            return (returnUrl is null)
                ?RedirectToAction("Index","Contact")
                :Redirect(returnUrl);
        }
        

        TempData["Error"] = "Error. Email or Password is invalid!";
        return View("Login");
    }

    // GET - Register
    [HttpGet("register")]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    
    // POST - Register
    [Route("register"), HttpPost("register")]
    [ValidateAntiForgeryToken]
    public IActionResult Register(LocalUser userData, string password, string confirmPassword)
    {
        if(!_userService.LocalUserExists(userData.Email).Result){
            if (password != confirmPassword)
            {
                TempData["VerifyPassword"] = "Verify password does not match!";
                return View("Register");
            }

            _userService.RegisterLocalUser(userData, password);
            return RedirectToAction("Validate", (userData, password, "/"));
        }
        TempData["UserExist"] = $"User with email {userData.Email} already exists.";
        return View("Register");
}
    
    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyEmail(string email)
    {
        if (!_userService.LocalUserExists(email).Result)
        {
            return Json($"Email {email} is already in use.");
        }

        return Json(true);
    }
    
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var scheme = User.Claims.FirstOrDefault(c => c.Type is ".AuthScheme").Value;
        if (scheme is "google")
        {
            await HttpContext.SignOutAsync();
            return Redirect($"https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=https://localhost:7040/");
        }
        if (scheme is "Cookies")
        { 
            await HttpContext.SignOutAsync(); 
            return Redirect("/");
        }
        return new SignOutResult(new[] {CookieAuthenticationDefaults.AuthenticationScheme, scheme});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}