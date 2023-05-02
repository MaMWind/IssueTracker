using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Models {
    public class Message {

        public int MessageId { get; set; }

        public string Text { get; set; }

        public int IssueID { get; set; }

        public string CreatorID { get; set; }

        public DateTime CreateDate { get; set;}
    }
}
