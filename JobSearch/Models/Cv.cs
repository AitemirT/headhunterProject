using System.ComponentModel.DataAnnotations;

namespace JobSearch.Models;

public class Cv
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название резюме не может быть пустым")]
    public string Title { get; set; }

    public DateTime UpdateDate { get; set; }
    
    [Required(ErrorMessage = "Ожидаемый уровень заработной платы не может быть пустым")]
    public decimal ExpectedSalaryLevel { get; set; }

    [Required(ErrorMessage = "Телеграм контакт не может быть пустым")]
    public string TelegramContact { get; set; }

    [Required(ErrorMessage = "Почта для связи не может быть пустым")]
    [EmailAddress(ErrorMessage = "Почта в некорректном формате")]
    public string EmailContact { get; set; }

    [Required(ErrorMessage = "Номер телефона не может быть пустым")]
    public string PhoneNumberContact { get; set; }

    public string? FacebookContact { get; set; }

    public string? LinkedInContact { get; set; }
    
    public bool IsPublic { get; set; }

    public List<WorkExperience> WorkExperiences { get; set; }
    
    public List<Education> Educations { get; set; }
    
    [Required(ErrorMessage = "Категория вакансии не может быть пустой")]
    public int JobCategoryId { get; set; }
    
    public JobCategory? JobCategory { get; set; }

    public int UserId { get; set; }
    
    public MyUser? User { get; set; }

    public Cv()
    {
        WorkExperiences = new List<WorkExperience>();
        Educations = new List<Education>();
    }
}