using IssueTracker.Authorization;
using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data {
    public class SeedData {

        public static async Task Initialize(IServiceProvider serviceProvider, string password = "$Demo123") {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>())) {
                context.Database.Migrate();
                var adminUid = await EnsureUser(serviceProvider, "admin@test.de", password);

                await EnsureRole(serviceProvider, adminUid, AuthorizationConstants.AdminRole);
                await EnsureRole(serviceProvider, null, AuthorizationConstants.SupportRole);
                await EnsureRole(serviceProvider, null, AuthorizationConstants.DeveloperRole);
                InitializeTestData(context, adminUid);
            }
        }

        private static void InitializeTestData(ApplicationDbContext context, string adminUid) {
            Project project = new Project() { CreatorID = adminUid, Name = "TestProjekt" };
            Project project2 = new Project() { CreatorID = adminUid, Name = "IssueTracker" };
            context.Project.Add(project);
            context.Project.Add(project2);
            context.SaveChanges();
            var projectID1 = context.Project.First(x => x.Name == "TestProjekt").ProjectID;
            var projectID2 = context.Project.First(x => x.Name == "IssueTracker").ProjectID;
            Issue issue1 = new Issue() { CreatorID = adminUid, ProjectID = projectID1, Description = "This is a test issue with mockup data", Title = "Test Issue 1", Status = Status.Open, Type = Models.Type.Enhancement };
            Issue issue2 = new Issue() { CreatorID = adminUid, ProjectID = projectID1, Description = "This is a test issue with mockup data", Title = "Test Issue 2", Status = Status.Closed, Type = Models.Type.Enhancement };
            Issue issue3 = new Issue() { CreatorID = adminUid, ProjectID = projectID1, Description = "This is a test issue with mockup data", Title = "Test Issue 3", Status = Status.Rejected, Type = Models.Type.Enhancement };
            Issue issue4 = new Issue() { CreatorID = adminUid, ProjectID = projectID1, Description = "This is a test issue with mockup data", Title = "Test Issue 4", Status = Status.Open, Type = Models.Type.Defect };
            Issue issue5 = new Issue() { CreatorID = adminUid, ProjectID = projectID1, Description = "This is a test issue with mockup data", Title = "Test Issue 5", Status = Status.Open, Type = Models.Type.Task };
            Issue issue6 = new Issue() { CreatorID = adminUid, ProjectID = projectID2, Description = "This is a test issue with mockup data", Title = "Test Issue 6", Status = Status.Open, Type = Models.Type.Enhancement };
            Issue issue7 = new Issue() { CreatorID = adminUid, ProjectID = projectID2, Description = "This is a test issue with mockup data", Title = "Test Issue 7", Status = Status.Open, Type = Models.Type.Enhancement };
            Issue issue8 = new Issue() { CreatorID = adminUid, ProjectID = projectID2, Description = "This is a test issue with mockup data", Title = "Test Issue 8", Status = Status.Closed, Type = Models.Type.Task };
            Issue issue9 = new Issue() { CreatorID = adminUid, ProjectID = projectID2, Description = "This is a test issue with mockup data", Title = "Test Issue 9", Status = Status.Open, Type = Models.Type.Enhancement };
            Issue issue10 = new Issue() { CreatorID = adminUid, ProjectID = projectID2, Description = "This is a test issue with mockup data", Title = "Test Issue 10", Status = Status.Rejected, Type = Models.Type.Defect};
            Issue issue11 = new Issue() { CreatorID = adminUid, ProjectID = projectID2, Description = "This is a test issue with mockup data", Title = "Test Issue 11", Status = Status.Open, Type = Models.Type.Enhancement };
            context.Issues.Add(issue1);
            context.Issues.Add(issue2);
            context.Issues.Add(issue3);
            context.Issues.Add(issue4);
            context.Issues.Add(issue5);
            context.Issues.Add(issue6);
            context.Issues.Add(issue7);
            context.Issues.Add(issue8);
            context.Issues.Add(issue9);
            context.Issues.Add(issue11);
            context.Issues.Add(issue10);
            context.SaveChanges();

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
