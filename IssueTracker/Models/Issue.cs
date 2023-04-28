using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Models {
    public class Issue {

        public int IssueID { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Type{ get; set; }

        // TODO implement Fileupload and storage
        public string Files { get; set; }

        public string Status { get; set; }

        public string CreatorID { get; set; }


        public string AssigneeID { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
