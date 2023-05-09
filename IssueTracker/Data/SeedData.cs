using IssueTracker.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data {
    public class SeedData {

        public static async Task Initialize(IServiceProvider serviceProvider, string password = "$Demo123") {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>())) {
                var adminUid = await EnsureUser(serviceProvider, "admin@test.de", password);

                await EnsureRole(serviceProvider, adminUid, AuthorizationConstants.AdminRole);
                await EnsureRole(serviceProvider, null, AuthorizationConstants.SupportRole);
                await EnsureRole(serviceProvider, null, AuthorizationConstants.DeveloperRole);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string userName, string initPW) {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(userName);

            if (user == null) {
                user = new IdentityUser {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, initPW);
            }

            if (user == null) {
                throw new Exception("User did not get created.");
            }
            return user.Id;
        }

        private static async Task EnsureRole(IServiceProvider serviceProvider, string uid, string role) {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            IdentityResult ir;
            if (await roleManager.RoleExistsAsync(role) == false) {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (uid != null) {
                var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
                var user = await userManager.FindByIdAsync(uid);

                if (user == null) {
                    throw new Exception("User not existing");
                }

                ir = await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
