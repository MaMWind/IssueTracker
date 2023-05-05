using Microsoft.AspNetCore.Identity;

namespace IssueTracker.ViewModels {
    public class UserManagementViewModel {

        public List<IdentityUser> Users { get; set; }

        public List<IdentityRole> Roles { get; set; }

        public List<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
