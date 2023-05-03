using IssueTracker.Models;

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
            return new IssueView { Title = issue.Title, Type = issue.Type, Status = issue.Status };
        }
    }

    public class IssueView {
        public string Title { get; set; }
        public Models.Type Type { get; set; }
        public Status Status { get; set; }
    }
}
