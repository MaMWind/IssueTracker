using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.ViewModels {
    public class IssueViewModel {

        public  List<IssueView> _issues { get; set; }

        public int ProjectID { get; set; }

        // Create the ViewModel, that should be used in the view from the database model.
        public  void ListFromIssueList(List<Issue> issues) {
            _issues = new List<IssueView>();
            foreach (var issue in issues) {
                _issues.Add(CreateFromIssue(issue));
            }
        }

        public static IssueView CreateFromIssue(Issue issue) {
            return new IssueView { Title = issue.Title, Type = issue.Type, Status = issue.Status, Description = issue.Description, FilePath = issue.Filepath, Id = issue.IssueID };
        }
    }

    public class IssueView {
        public int Id { get; set; }
        public string Title { get; set; }
        public Models.Type Type { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public List<MessageView> Messages { get; set; }
        public string? FilePath { get; set; }

        public IdentityUser Assignee { get; set; }

        public IdentityUser Creator { get; set; }
    }

    public class MessageView {
        public int MessageId { get; set; }

        public string Text { get; set; }

        public IdentityUser Creator { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
