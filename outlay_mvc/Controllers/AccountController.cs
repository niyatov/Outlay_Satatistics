using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using outlay_mvc.Dtoes;
using outlay_mvc.Entities;

namespace outlay_mvc.Controllers;
[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    public IActionResult SignIn() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(SignInDto signIn, string? ReturnUrl)
    {
        if (!ModelState.IsValid) return View();

        User user;
        if (signIn.UsernameOrEmail!.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(signIn.UsernameOrEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(signIn.UsernameOrEmail);
        }
        if (user == null)
        {
            ModelState.AddModelError("", "Login or parol incorrect");
            return View(signIn);
        }
        var result = await _signInManager.PasswordSignInAsync(user, signIn.Password, true, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Login or parol incorrect");
            return View(signIn);
        }
        if (ReturnUrl != null) return LocalRedirect(ReturnUrl);
        return Redirect("/");
    }


    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto register)
    {

        if (!ModelState.IsValid) return View();

        var userExists = await _userManager.FindByNameAsync(register.Username) != null;
        var emailExists = await _userManager.FindByEmailAsync(register.Email) != null;

        if (userExists)
        {
            ModelState.AddModelError("", "Username already exists.");
            return View(register);
        }

        if (emailExists)
        {
            ModelState.AddModelError("", "Email already exists.");
            return View(register);
        }


        User user = new User
        {
            Email = register.Email,
            UserName = register.Username
        };

        IdentityResult result = await _userManager.CreateAsync(user, register.Password);

        if (!result.Succeeded)
        {
            return View();
        }

        //await _signInManager.SignInAsync(user, true);

        return RedirectToAction("SignIn");
    }

    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(SignIn));
    }
}
