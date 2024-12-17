using JobSearch.Models;
using JobSearch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace JobSearch.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly SignInManager<MyUser> _signInManager;
    private readonly IWebHostEnvironment _environment;
    private readonly JobSearchDbContext _context;

    public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IWebHostEnvironment environment, JobSearchDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "")
    {
        return View(new LoginViewModel {ReturnUrl = returnUrl});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            MyUser? user = await _userManager.FindByEmailAsync(model.UserNameOrEmail) ?? await _userManager.FindByNameAsync(model.UserNameOrEmail);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
                return View(model);
            }
            
            Microsoft.AspNetCore.Identity.SignInResult result =
                await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            
             if (result.Succeeded)
             {
                 if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                 {
                     return Redirect(model.ReturnUrl);
                 }

                 return RedirectToAction("Index", "Home");
             }
             
             ModelState.AddModelError(string.Empty, "Неправильные логин или пароль");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Roles = new List<string> { "Соискатель", "Работодатель" };
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            string fileName = $"avatar_{model.Email}{Path.GetExtension(model.Avatar.FileName)}";
            
            if (model.Avatar != null && model.Avatar.Length > 0 && model.Avatar.ContentType.StartsWith("image/"))
            {
                string filePath = Path.Combine(_environment.WebRootPath, "avatars", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(stream);
                }
            }
            else
            {
                ModelState.AddModelError("Avatar", "Аватар может быть только картинкой");
                return View(model);
            }
            
            MyUser myUser = new MyUser
            {
                UserName = model.UserName.ToLower(),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PathToAvatarPhoto = $"/avatars/{fileName}"
            };

            IdentityResult result = await _userManager.CreateAsync(myUser, model.Password);

            if (result.Succeeded)
            {
                if (model.Role == "Соискатель")
                {
                    await _userManager.AddToRoleAsync(myUser, "jobSeeker");
                }
                else if (model.Role == "Работодатель")
                {
                    await _userManager.AddToRoleAsync(myUser, "employer");
                }
                await _signInManager.SignInAsync(myUser, false);
                await _userManager.AddToRoleAsync(myUser, "user");
                await _signInManager.RefreshSignInAsync(myUser);
                return RedirectToAction("Index", "Home");
            }

            foreach (IdentityError error in result.Errors) 
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        ViewBag.Roles = new List<string> { "Соискатель", "Работодатель" };
        return View(model);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}