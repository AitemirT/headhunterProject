namespace JobSearch.Models;

public class Chat
{
    public int Id { get; set; }
    public int CvId { get; set; }
    public Cv? Cv { get; set; }
    public int JobSeekerID { get; set; }
    public MyUser? JobSeeker { get; set; }
    public int EmployerID { get; set; }
    public MyUser? Employer { get; set; }
    
    public int RespondId { get; set; }
    public Respond? Respond { get; set; }
    public List<Message> Messages { get; set; }
}