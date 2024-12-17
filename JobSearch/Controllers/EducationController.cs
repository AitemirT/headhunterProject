using JobSearch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Controllers;

public class EducationController : Controller
{
    private readonly JobSearchDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    public EducationController(JobSearchDbContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        Education? education = await _context.Educations.FirstOrDefaultAsync(w => w.Id == id);

        if (education == null)
        {
            return NotFound();
        }

        _context.Educations.Remove(education);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Cv", new {id = education.CvId});
    }
}