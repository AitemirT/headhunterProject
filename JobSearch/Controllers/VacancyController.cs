using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobSearch.Models;
using Microsoft.AspNetCore.Identity;

namespace JobSearch.Controllers
{
    public class VacancyController : Controller
    {
        private readonly JobSearchDbContext _context;
        private readonly UserManager<MyUser> _userManager;

        public VacancyController(JobSearchDbContext context, UserManager<MyUser> userManager)
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

            var vacancy = await _context.Vacancies
                .Include(v => v.JobCategory)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }
            if (User.IsInRole("jobSeeker"))
            {
                MyUser user = await _userManager.GetUserAsync(User);
                ViewBag.Cvs = await _context.Cvs.Where(c => c.UserId == user.Id).ToListAsync();
            }


            return View(vacancy);
        }
        
        public IActionResult Create()
        {
            if (!User.IsInRole("employer"))
            {
                return BadRequest();
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vacancy vacancy)
        {
            if (!User.IsInRole("employer"))
            {
                return BadRequest();
            }
            vacancy.UpdateDate = DateTime.UtcNow;
            vacancy.UserId = (await _userManager.GetUserAsync(User)).Id;
            vacancy.IsPublic = false;
            
            if (ModelState.IsValid)
            {
                _context.Add(vacancy);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name", vacancy.JobCategoryId);
            return View(vacancy);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vacancy? vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Name", vacancy.JobCategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", vacancy.UserId);
            return View(vacancy);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Vacancy vacancy)
        {
            if (!User.IsInRole("employer"))
            {
                return BadRequest();
            }
            vacancy.UpdateDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacancy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacancyExists(vacancy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "User");
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "Id", "Id", vacancy.JobCategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", vacancy.UserId);
            return View(vacancy);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id)
        {
            if (!User.IsInRole("employer"))
            {
                return BadRequest();
            }
            Vacancy? vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);
            
            if (vacancy == null)
            {
                return NotFound();
            }
            
            MyUser? user = await _userManager.GetUserAsync(User);
            if (user.Id != vacancy.UserId)
            {
                return BadRequest();
            }
            vacancy.UpdateDate = DateTime.UtcNow;
            _context.Update(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "User", new {id = user.Id});
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("employer"))
            {
                return BadRequest();
            }
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }

            MyUser user = await _userManager.GetUserAsync(User);
            if (user.Id != vacancy.UserId)
            {
                return BadRequest();
            }

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "User", new {id = user.Id});
        }

        private bool VacancyExists(int id)
        {
            return _context.Vacancies.Any(e => e.Id == id);
        }
        
        [HttpPost]
        public async Task<IActionResult> PublishHide(int id)
        {
            if (!User.IsInRole("employer"))
            {
                return BadRequest();
            }
            Vacancy? vacancy = await _context.Vacancies.FirstOrDefaultAsync(cv => cv.Id == id);

            if (vacancy == null)
            {
                return NotFound();
            }

            MyUser user = await _userManager.GetUserAsync(User);

            if (user.Id != vacancy.UserId)
            {
                return BadRequest();
            }

            vacancy.IsPublic = !vacancy.IsPublic;
            _context.Update(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "User", new {id = user.Id});
        }
    }
}
