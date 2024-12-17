using JobSearch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.Controllers;

public class ValidationController : Controller
{
    private readonly UserManager<MyUser> _userManager;

    public ValidationController(UserManager<MyUser> userManager)
    {
        _userManager = userManager;
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserEmail(string email, int id)
    {
        MyUser myUser = await _userManager.FindByEmailAsync(email);

        if (myUser != null && myUser.Id != id)
        {
            return Json(false);
        }
        return Json(true);
    }
    
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserName(string username, int id)
    {
        MyUser myUser = await _userManager.FindByNameAsync(username);

        if (myUser != null && myUser.Id != id)
        {
            return Json(false);
        }

        return Json(true);
    }
}