using JobSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Controllers;

[Authorize]
public class RespondController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly JobSearchDbContext _context;

    public RespondController(UserManager<MyUser> userManager, JobSearchDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        MyUser user = await _userManager.GetUserAsync(User);
        List<Respond> responds = await _context.Responds
            .Where(r => r.JobSeekerId == user.Id)
            .Include(r => r.Vacancy)
            .Include(r => r.Employer)
            .Include(r => r.Cv)
            .ToListAsync();

        if (User.IsInRole("employer"))
        {
            responds = await _context.Responds
                .Where(r => r.EmployerId == user.Id)
                .Include(r => r.Vacancy)
                .Include(r => r.Employer)
                .Include(r => r.Cv)
                .ToListAsync();
        }

        return View(responds);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRespond(int? vacancyId, int? cvId)
    {
        MyUser jobSeeker = await _userManager.GetUserAsync(User);
        
        if (jobSeeker == null || !await _userManager.IsInRoleAsync(jobSeeker, "jobSeeker"))
        {
            return BadRequest();
        }

        Vacancy? vacancy = await _context.Vacancies.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
        {
            return NotFound();
        }
        Cv? cv = await _context.Cvs.FindAsync(cvId);
        if (cv == null)
        {
            return NotFound();
        }

        if (vacancy.UserId == jobSeeker.Id)
        {
            return BadRequest("Нельзя откликаться на собственную вакансию");
        }

        Respond? respond = await _context.Responds.FirstOrDefaultAsync(r => r.VacancyId == vacancyId && r.CvId == cvId);
        
        if (respond != null)
        {
            return RedirectToAction("Index", "Chat", new { chatId = respond.ChatId});
        }

        respond = new Respond()
        {
            VacancyId = vacancy.Id,
            CvId = cv.Id,
            CreationDate = DateTime.UtcNow,
            JobSeekerId = jobSeeker.Id,
            EmployerId = vacancy.User.Id,
        };
        _context.Responds.Add(respond);
        await _context.SaveChangesAsync();
        
        Chat newChat = new Chat
        {
            RespondId = respond.Id,
            CvId = cv.Id,
            JobSeekerID = jobSeeker.Id,
            EmployerID = vacancy.User.Id,
        };

        _context.Chats.Add(newChat);
        await _context.SaveChangesAsync();
        
        respond.ChatId = newChat.Id;

        _context.Responds.Update(respond);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index", "Chat", new { chatId = newChat.Id });
    }
}