namespace JobSearch.Models;

public class JobCategory
{
    public int Id { get; set; }

    public string Name { get; set; }
    public List<Cv> Cvs { get; set; }

    public JobCategory()
    {
        Cvs = new List<Cv>();
    }
}