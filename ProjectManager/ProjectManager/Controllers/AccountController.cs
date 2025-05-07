using Microsoft.AspNetCore.Mvc;
using ProjectManager.Models.Services;
using ProjectManager.Models;
using System.Security.Cryptography;
using ProjectManager.Models.ViewModels;
using ProjectManager.Encryption;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ProjectManager.Controllers
{
    
    public class AccountController : Controller
    {

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Title"] = "Login";
                return View();
            }
                
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await MongoManipulator.GetObjectByField<User>("Username", model.Name.ToLower().Trim());
                if (user != null)
                {
                    if (Argon2Helper.VerifyPassword(model.Password, user.Password, user.Salt))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, "User"),
                            new Claim(ClaimTypes.NameIdentifier, user._id.ToString())
                        };
                        var claimsIndetity = new ClaimsIdentity
                        (claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                        );

                        //Authentication settings
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,  // Refresh cookie if about to expire
                            IsPersistent = true // Cookies persist on browser reset
                        };
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIndetity),
                            authProperties
                        );
                        Console.WriteLine("Login succes");
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("Password", "Username or password incorrect");
                    return View(model);
                }
                ModelState.AddModelError("Password", "Username or password incorrect");
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Title"] = "Register";
                return View();
            }
        }

        //Register User action
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userList = await MongoManipulator.GetAllObjects<User>();

                bool usernameTaken = userList.Any(u => u.Username.ToLower() == model.Name.ToLower());
                bool emailTaken = userList.Any(u => u.Email == model.Email.ToLower());
                bool passwordsMatch = model.Password ==  model.ConfirmPassword;

                if (usernameTaken || emailTaken || !passwordsMatch)
                {
                    if (usernameTaken)
                        ModelState.AddModelError("Name", "This username is already taken");
                    if (emailTaken)
                        ModelState.AddModelError("Email", "This email is already registered");
                    if (!passwordsMatch)
                        ModelState.AddModelError("ConfirmPassword", "Passwords didnt match ");

                    return View(model);
                }
                else
                {
                    var generatedSalt = new byte[16];  // create empty bytes for the salt
                    RandomNumberGenerator.Fill(generatedSalt);  // Fill the salt with secure random generator

                    var newUser = new User()
                    {
                        Username = model.Name.Trim().ToLower(),
                        FirstName = model.FirstName.Trim().ToLower(),
                        LastName = model.LastName.Trim().ToLower(),
                        Password = Argon2Helper.HashPassword(model.Password.Trim(), generatedSalt),
                        Email = model.Email.Trim().ToLower(),
                        IsActive = true,
                        Salt = generatedSalt

                    };

                    await MongoManipulator.Save(newUser); // Save to DB

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}
