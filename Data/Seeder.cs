using Microsoft.AspNetCore.Identity;
using Quiz_App.Models;
using System;
using System.Threading.Tasks;

public class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string roleName = "Admin";
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = "admin1@gmail.com"; // Replace with your desired email
        var adminPassword = "Admin@123"; // Replace with your desired password

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
            var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
            if (createAdminUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, roleName);
            }
        }
    }
}
