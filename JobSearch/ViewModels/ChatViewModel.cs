using JobSearch.Models;

namespace JobSearch.ViewModels;

public class ChatViewModel
{
    public MyUser CurrentUser { get; set; }
    public Chat Chat { get; set; }
    public List<Message> Messages { get; set; }
}