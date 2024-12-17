using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobSearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace JobSearch.Controllers
{
    [Authorize]
    public class CvController : Controller
    {
        private readonly JobSearchDbContext _context;
        private readonly UserManager<MyUser> _userManager;

        public CvController(JobSearchDbContext context, UserManager<MyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cv = await _context.Cvs
                .Include(c => c.JobCategory)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Educations)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cv == null)
            {
                return NotFound();
            }

            return View(cv);
        }
        
        public async Task<IActionResult> Create()
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            MyUser user = await _userManager.GetUserAsync(User);
            
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cv cv)
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            cv.UpdateDate = DateTime.UtcNow;
            cv.UserId = (await _userManager.GetUserAsync(User)).Id;
            cv.IsPublic = false;
            
            if (ModelState.IsValid)
            {
                _context.Add(cv);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name", cv.JobCategoryId);
            return View(cv);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var cv = await _context.Cvs
                .Include(cv => cv.WorkExperiences)
                .Include(cv => cv.Educations)
                .FirstOrDefaultAsync(cv => cv.Id == id);
            
            if (cv == null)
            {
                return NotFound();
            }
            
            MyUser user = await _userManager.GetUserAsync(User);

            if (user.Id != cv.UserId)
            {
                return BadRequest();
            }
            
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name", cv.JobCategoryId);
            return View(cv);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cv cv)
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            MyUser user = await _userManager.GetUserAsync(User);

            if (user.Id != cv.UserId)
            {
                return BadRequest();
            }
            
            cv.UpdateDate = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CvExists(cv.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "User", new {id = user.Id});
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name", cv.JobCategoryId);
            return View(cv);
        }

        [HttpPost]
        public async Task<IActionResult> PublishHide(int id)
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            Cv? cv = await _context.Cvs.FirstOrDefaultAsync(cv => cv.Id == id);

            if (cv == null)
            {
                return NotFound();
            }

            MyUser user = await _userManager.GetUserAsync(User);

            if (user.Id != cv.UserId)
            {
                return BadRequest();
            }

            cv.IsPublic = !cv.IsPublic;
            _context.Update(cv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "User", new {id = user.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id)
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            Cv? cv = await _context.Cvs.FirstOrDefaultAsync(cv => cv.Id == id);

            if (cv == null)
            {
                return NotFound();
            }

            MyUser user = await _userManager.GetUserAsync(User);

            if (user.Id != cv.UserId)
            {
                return BadRequest();
            }
            
            cv.UpdateDate = DateTime.UtcNow;
            _context.Update(cv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("jobSeeker"))
            {
                return BadRequest();
            }
            
            var cv = await _context.Cvs.FindAsync(id);
            
            if (cv == null)
            {
                return NotFound();
            }

            MyUser user = await _userManager.GetUserAsync(User);

            if (user.Id != cv.UserId)
            {
                return BadRequest();
            }
            
            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "User", new {id = user.Id});
        }

        private bool CvExists(int id)
        {
            return _context.Cvs.Any(e => e.Id == id);
        }
    }
}
