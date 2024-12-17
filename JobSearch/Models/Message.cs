namespace JobSearch.Models;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime DateOfSend { get; set; }
    
    public int SenderId { get; set; }
    public MyUser? Sender { get; set; }
    public int ChatId { get; set; }
    public Chat Chat { get; set; }
}