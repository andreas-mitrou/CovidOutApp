using System;
using System.Threading.Tasks;
using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CovidOutApp.Web.Helpers {

    public static class IdentityRoleHelpers {

        public static void InitializeIdentityData(IServiceProvider serviceProvider){
              using (var scope = serviceProvider.CreateScope())
                {
                    var provider = scope.ServiceProvider;
                    var context = provider.GetRequiredService<ApplicationDbContext>();
                    var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
                    CreateRoles(roleManager);
                    CreateSuperAdmin(userManager, false);
                }
        }   

        public static async void CreateSuperAdmin(UserManager<ApplicationUser> userManager, bool deleteExisting=false){
          var user = await userManager.FindByNameAsync("SuperAdmin@yahoo.gr");
          
          if (user != null && deleteExisting){
              await userManager.DeleteAsync(user);
              user = null;
          }    
          
          if (user == null){
                var result = await userManager.CreateAsync(new ApplicationUser{ UserName = "SuperAdmin@yahoo.gr", Email= "SuperAdmin@yahoo.gr", EmailConfirmed=true},"Super$123Admin");          
                if (result.Succeeded){
                    user = await userManager.FindByNameAsync("SuperAdmin@yahoo.gr");
                    if (user != null){
                        await userManager.AddToRoleAsync(user,"Admin");
                    }   
                }
          }         
        }

        public static async void CreateRoles (RoleManager<IdentityRole> roleManager){
            string[] roles = new string[] {"Admin", "Visitor", "VenueOwner"};
            foreach (var role in roles){
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        private static async Task EnsureRoleCreated(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public static async Task EnsureRolesCreated(this RoleManager<IdentityRole> roleManager)
        {
        // add all roles, that should be in database, here
            await EnsureRoleCreated(roleManager, "VenueOwner");
            await EnsureRoleCreated(roleManager, "Administrator");
            await EnsureRoleCreated(roleManager, "Visitor");
        }
    }
}