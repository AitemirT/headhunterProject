namespace JobSearch.Models;

public class Vacancy
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Salary { get; set; }
    public string Description { get; set; }
    public int? MaxExperience { get; set; }
    public int? MinExperience { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool IsPublic { get; set; }
    public int JobCategoryId { get; set; }
    public JobCategory? JobCategory { get; set; }
    public int UserId { get; set; }
    public MyUser? User { get; set; }
}