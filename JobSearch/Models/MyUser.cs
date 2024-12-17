using Microsoft.AspNetCore.Identity;

namespace JobSearch.Models;

public class MyUser : IdentityUser<int>
{
    public string PathToAvatarPhoto { get; set; } = string.Empty;
    public List<Cv> Cvs { get; set; }

    public MyUser()
    {
        Cvs = new List<Cv>();
    }
}