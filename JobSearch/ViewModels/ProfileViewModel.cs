using JobSearch.Models;

namespace JobSearch.ViewModels;

public class ProfileViewModel
{
    public MyUser CurrentUser { get; set; }
    public MyUser User { get; set; }
    public List<Cv> Cvs { get; set; }
    public List<Vacancy> Vacancies { get; set; }
    public bool IsTargetUserJobSeeker { get; set; }
    public bool IsTargetUserEmployer { get; set; }
}