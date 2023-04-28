using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Models {
    public class Project {
        public int ProjectID { get; set; }

        public string Name { get; set; }

        // The Creator of the Project. The ID in IdentityUser is a string.
        public string CreatorID { get; set; } 
    }
}
