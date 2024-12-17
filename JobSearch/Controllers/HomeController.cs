using JobSearch.Models;
using JobSearch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly JobSearchDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    public HomeController(JobSearchDbContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        MyUser user = await _userManager.GetUserAsync(User);
        
        ViewBag.JobCategories = await _context.JobCategories.ToListAsync();

        IQueryable<Cv> cvQuery = _context.Cvs.AsQueryable()
            .Where(cv => cv.IsPublic);
        
        IQueryable<Vacancy> vacancyQuery = _context.Vacancies.AsQueryable()
            .Where(v => v.IsPublic);


        cvQuery = cvQuery.OrderByDescending(cv => cv.UpdateDate);

        vacancyQuery = vacancyQuery.OrderByDescending(cv => cv.UpdateDate);
        
        int pageSize = 20;
        int page = 1;
        
        int cvsCount = await cvQuery.CountAsync();
        List<Cv> cvs = await cvQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(cv => cv.User)
            .Include(cv => cv.JobCategory)
            .ToListAsync();
        
        int vacanciesCount = await vacancyQuery.CountAsync();
        List<Vacancy> vacancies = await vacancyQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(v => v.User)
            .Include(v => v.JobCategory)
            .ToListAsync();
        
        
        IndexViewModel vm = new IndexViewModel
        {
            CurrentUser = user,
            AllCvs = await _context.Cvs.Where(v => v.UserId == user.Id).ToListAsync(),
            PublicCvs = cvs,
            Vacancies = vacancies,
            PageViewModelForCvs = new PageViewModel(cvsCount, page, pageSize),
            PageViewModelForVacancies = new PageViewModel(vacanciesCount, page, pageSize)
        };
        
        return View(vm);
    }

    public async Task<IActionResult> IndexWithParams(string search = "", int filter = -1, string sortOrder = "dateDesc", int page = 1)
    {
        MyUser user = await _userManager.GetUserAsync(User);
        
        ViewBag.JobCategories = await _context.JobCategories.ToListAsync();

        IQueryable<Cv> cvQuery = _context.Cvs.AsQueryable()
            .Where(cv => cv.IsPublic && (string.IsNullOrEmpty(search) || cv.Title.ToLower().Contains(search.ToLower())));
        
        IQueryable<Vacancy> vacancyQuery = _context.Vacancies.AsQueryable()
            .Where(v => v.IsPublic && (string.IsNullOrEmpty(search) || v.Title.ToLower().Contains(search.ToLower())));

        if (filter != -1)
        {
            cvQuery = cvQuery.Where(cv => cv.JobCategoryId == filter);
            vacancyQuery = vacancyQuery.Where(v => v.JobCategoryId == filter);
        }

        cvQuery = sortOrder switch
        {
            "dateAsc" => cvQuery.OrderBy(cv => cv.UpdateDate),
            "salaryAsc" => cvQuery.OrderBy(cv => cv.ExpectedSalaryLevel),
            "salaryDesc" => cvQuery.OrderByDescending(cv => cv.ExpectedSalaryLevel),
            _ => cvQuery.OrderByDescending(cv => cv.UpdateDate)
        };
        
        vacancyQuery = sortOrder switch
        {
            "dateAsc" => vacancyQuery.OrderBy(cv => cv.UpdateDate),
            "salaryAsc" => vacancyQuery.OrderBy(cv => cv.Salary),
            "salaryDesc" => vacancyQuery.OrderByDescending(cv => cv.Salary),
            _ => vacancyQuery.OrderByDescending(cv => cv.UpdateDate)
        };
        
        int pageSize = 20;
        
        int cvsCount = await cvQuery.CountAsync();
        List<Cv> cvs = await cvQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(cv => cv.User)
            .Include(cv => cv.JobCategory)
            .ToListAsync();
        
        int vacanciesCount = await vacancyQuery.CountAsync();
        List<Vacancy> vacancies = await vacancyQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(v => v.User)
            .Include(v => v.JobCategory)
            .ToListAsync();
        
        
        IndexViewModel vm = new IndexViewModel
        {
            CurrentUser = user,
            AllCvs = await _context.Cvs.Where(v => v.UserId == user.Id).ToListAsync(),
            PublicCvs = cvs,
            Vacancies = vacancies,
            PageViewModelForCvs = new PageViewModel(cvsCount, page, pageSize),
            PageViewModelForVacancies = new PageViewModel(vacanciesCount, page, pageSize)
        };

        return PartialView("_IndexPartialview", vm);
    }
}