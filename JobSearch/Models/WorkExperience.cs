using System.ComponentModel.DataAnnotations;

namespace JobSearch.Models;

public class WorkExperience
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Год начала работы не может быть пустым")]
    public int WorkStartYear { get; set; }

    [Required(ErrorMessage = "Год окончания работы не может быть пустым")]
    public int? WorkEndYear { get; set; }
    
    [Required(ErrorMessage = "Название места работы не может быть пустым")]
    public string CompanyName { get; set; }

    [Required(ErrorMessage = "Должность не может быть пустой")]
    public string Position { get; set; }

    [Required(ErrorMessage = "Должностные обязанности не могут быть пустыми")]
    public string Responsibilities { get; set; }
    
    public int CvId { get; set; }

    public Cv? Cv { get; set; }
}