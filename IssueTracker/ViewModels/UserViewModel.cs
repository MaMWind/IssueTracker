using Microsoft.AspNetCore.Identity;

namespace IssueTracker.ViewModels {
    public class UserViewModel {
        public string id { get; set; }
        public string name { get; set; }
        public List<IdentityUserRole<string>> roles { get; set; }

        public bool support { get; set; }
        public bool developer { get; set; }
        public bool admin { get; set; }
    }
}
