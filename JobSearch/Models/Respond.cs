namespace JobSearch.Models;

public class Respond
{
    public int Id { get; set; }
    public int VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }
    public int CvId { get; set; }
    public Cv? Cv { get; set; }
    public int JobSeekerId { get; set; }
    public MyUser? JobSeeker { get; set; }
    public int EmployerId { get; set; }
    public MyUser? Employer { get; set; }

    public int? ChatId { get; set; }
    public Chat? Chat { get; set; }
    public DateTime CreationDate { get; set; }
    
}