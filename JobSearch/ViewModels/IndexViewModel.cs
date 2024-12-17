using JobSearch.Models;

namespace JobSearch.ViewModels;

public class IndexViewModel
{
    public MyUser CurrentUser { get; set; }
    
    public List<Cv> PublicCvs { get; set; }
    public List<Cv> AllCvs { get; set; }
    
    public List<Vacancy> Vacancies { get; set; }

    public PageViewModel PageViewModelForCvs { get; set; }
    
    public PageViewModel PageViewModelForVacancies { get; set; }
}