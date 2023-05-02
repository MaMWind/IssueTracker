using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Models {
    public class Issue {

        public int IssueID { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public Type Type{ get; set; }

        // Store the uploaded files directly onto the server and only store the path in the database.
        public string Filepath { get; set; }

        public Status Status { get; set; }

        public string CreatorID { get; set; }

        public string AssigneeID { get; set; }

        public ICollection<Message> Messages { get; set; }
    }

    public enum Status {
        Open,
        Closed,
        Rejected
    }

    public enum Type {
        Defect,
        Enhancement,
        Task
    }
}
