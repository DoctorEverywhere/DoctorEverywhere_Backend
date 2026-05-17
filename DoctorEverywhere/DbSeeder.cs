using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using DoctorEverywhere.Domain;

namespace DoctorEverywhere
{
    public static class DbSeeder
    {
        public static async Task SeedManagerAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await roleManager.RoleExistsAsync("Manager"))
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            var managerUsername = "manager";
            var existingUser = await userManager.FindByNameAsync(managerUsername);

            if (existingUser == null)
            {
                var newManager = new ApplicationUser
                {
                    UserName = managerUsername
                };

                var result = await userManager.CreateAsync(newManager, "Manager1!!!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newManager, "Manager");

                    var managerProfile = new Manager
                    {
                        ApplicationUserId = newManager.Id,
                        FirstName = "Manager",
                        LastName = "Admin",
                    };
                    context.Managers.Add(managerProfile);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
