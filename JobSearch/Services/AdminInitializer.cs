using JobSearch.Models;
using Microsoft.AspNetCore.Identity;

namespace JobSearch.Services;

public class AdminInitializer
{
    public static async Task SeedRolesAndAdmin(RoleManager<IdentityRole<int>> roleManager, UserManager<MyUser> _userManager)
    {
        string adminEmail = "admin@admin.com";
        string adminPassword = "1qwe@QWE";
        string avatar = $"/adminAvatar/adminAvatar.jpeg";
        string adminUserName = "admin";

        var roles = new[] { "admin", "user", "jobSeeker", "employer" };
        
        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        if (await _userManager.FindByNameAsync(adminEmail) == null)
        {
            MyUser admin = new MyUser { Email = adminEmail, UserName = adminUserName, PathToAvatarPhoto = avatar};
            IdentityResult result = await _userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(admin, "admin");
        }
    }
}