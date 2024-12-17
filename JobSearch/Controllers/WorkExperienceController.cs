using JobSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Controllers;

[Authorize]
public class WorkExperienceController : Controller
{
    private readonly JobSearchDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    public WorkExperienceController(JobSearchDbContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        WorkExperience? workExperience = await _context.WorkExperiences.FirstOrDefaultAsync(w => w.Id == id);

        if (workExperience == null)
        {
            return NotFound();
        }

        _context.WorkExperiences.Remove(workExperience);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Cv", new {id = workExperience.CvId});
    }
}