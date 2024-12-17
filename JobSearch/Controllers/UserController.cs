using JobSearch.Models;
using JobSearch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly JobSearchDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public UserController(UserManager<MyUser> userManager, JobSearchDbContext context, IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Profile(int? id)
    {
        MyUser? currentUser = await _userManager.GetUserAsync(User);

        if (id != null)
        {
            MyUser? user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }
            
            return View(new ProfileViewModel
            {
                CurrentUser = currentUser,
                User = user,
                Cvs = user.Id == currentUser.Id ? await _context.Cvs.Where(c => c.UserId == user.Id).ToListAsync() 
                    : await _context.Cvs.Where(c => c.UserId == user.Id && c.IsPublic).ToListAsync(),
                Vacancies = user.Id == currentUser.Id ? await _context.Vacancies.Where(v => v.UserId == user.Id).ToListAsync()
                    : await _context.Vacancies.Where(v => v.UserId == user.Id && v.IsPublic).ToListAsync(),
                IsTargetUserJobSeeker = await _userManager.IsInRoleAsync(user, "jobSeeker"),
                IsTargetUserEmployer = await _userManager.IsInRoleAsync(user, "employer")
            });
        }

        return View(new ProfileViewModel
        {
            CurrentUser = currentUser,
            User = currentUser,
            Cvs = await _context.Cvs.Where(c => c.UserId == currentUser.Id).ToListAsync(),
            Vacancies = await _context.Vacancies.Where(v => v.UserId == currentUser.Id).ToListAsync(),
            IsTargetUserJobSeeker = await _userManager.IsInRoleAsync(currentUser, "jobSeeker"),
            IsTargetUserEmployer = await _userManager.IsInRoleAsync(currentUser, "employer")
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            MyUser user = await _userManager.FindByIdAsync(model.Id.ToString());
            MyUser currentUser = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Нет такого пользователя");
                return StatusCode(400, PartialView("_EditFormPartialView", model));
            }

            if (user.Id != currentUser.Id)
            {
                return Forbid();
            }
            
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;

            if (model.Avatar != null)
            {
                if (model.Avatar.Length > 0 && model.Avatar.ContentType.StartsWith("image/"))
                {
                    if (!string.IsNullOrEmpty(user.PathToAvatarPhoto))
                    {
                        string filePathToDelete = Path.Combine(_environment.WebRootPath, user.PathToAvatarPhoto.TrimStart('/'));
                        if (System.IO.File.Exists(filePathToDelete))
                        {
                            System.IO.File.Delete(filePathToDelete);
                        }
                    }
                    
                    string newAvatarFileName = $"avatar_{user.Email}{Guid.NewGuid()}{Path.GetExtension(model.Avatar.FileName)}";
                    string filePath = Path.Combine(_environment.WebRootPath, "avatars", newAvatarFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Avatar.CopyToAsync(stream);
                    }

                    user.PathToAvatarPhoto = $"/avatars/{newAvatarFileName}";
                }
                else
                {
                    ModelState.AddModelError("Avatar", "Аватаром может быть только картинка");
                }
            }

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Profile", "User", new {id = user.Id});
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            ModelState.AddModelError(string.Empty, "Данные в некорректном формате");
        }

        return StatusCode(400, PartialView("_EditFormPartialView", model));
    }
}