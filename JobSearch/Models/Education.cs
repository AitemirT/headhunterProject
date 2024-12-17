using System.ComponentModel.DataAnnotations;

namespace JobSearch.Models;

public class Education
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Год начала обучения не может быть пустым")]
    public int StartYear { get; set; }

    public int? EndYear { get; set; }

    [Required(ErrorMessage = "Название учебного заведения не может быть пустой")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Направление обучения не может быть пустым")]
    public string Direction { get; set; }

    public int CvId { get; set; }

    public Cv? Cv { get; set; }
}