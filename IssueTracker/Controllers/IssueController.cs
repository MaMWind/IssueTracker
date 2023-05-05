using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IssueTracker.ViewModels;
using IssueTracker.Data;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Controllers {
    public class IssueController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IssueController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int projectID) {
            List<Issue> issues = await _context.Issues.Where(x=>x.ProjectID == projectID).ToListAsync();
            IssueViewModel viewModel = new IssueViewModel();
            viewModel.ListFromIssueList(issues);
            viewModel.ProjectID = projectID;
            return View(viewModel);
        }

        // GET: Issue/Create
        public IActionResult Create(int projectID) {
            var issueModel = new Issue();
            issueModel.ProjectID = projectID;
            return View(issueModel);
        }

        public IActionResult CreateIssue(Issue issueModel) {
            issueModel.Status = Status.Open;
            string id = _userManager.GetUserId(User);
            issueModel.CreatorID = id;
            _context.Issues.Add(issueModel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), new {projectID = issueModel.ProjectID}); 
        }

        public IActionResult Details(int id) {
            Issue issue = _context.Issues.FirstOrDefault(x => x.IssueID == id);
            IssueView issueView = IssueViewModel.CreateFromIssue(issue);
            issueView.Messages = MessageViewFromMessage(_context.Messages.Where(x => x.IssueID == id).ToList());
            issueView.Creator = _context.Users.FirstOrDefault(x => x.Id == issue.CreatorID);
            issueView.Assignee = _context.Users.FirstOrDefault(x => x.Id == issue.AssigneeID);
            if (issueView == null) {
                return View("Error");
            }
            return View(issueView);
        }

        public IActionResult Delete(int id) {
            Issue issue = _context.Issues.First(x => x.IssueID == id);
            int projectId = issue.ProjectID;
            _context.Issues.Remove(issue);
            _context.Messages.RemoveRange(_context.Messages.Where(x => x.IssueID == id));
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), new { projectID = projectId });
        }

        public IActionResult CreateMessage(Message message) {
            Issue issue = _context.Issues.FirstOrDefault(x=> x.IssueID == message.IssueID);
            issue.Messages.Add(message);
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new {id = message.IssueID});
        }


        private List<MessageView> MessageViewFromMessage(List<Message> messages) {
            List<MessageView> messageViews = new List<MessageView>();
            foreach (Message message in messages) {
                MessageView messageView = new MessageView { CreateDate = message.CreateDate, 
                    Creator = _context.Users.FirstOrDefault(x=> x.Id == message.CreatorID), 
                    MessageId = message.MessageId, Text = message.Text };
               messageViews.Add(messageView);
            }
            return messageViews;
        }

        public IActionResult Assign(int id) {
            _context.Issues.First(x => x.IssueID == id).AssigneeID = _userManager.GetUserId(User);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new {id = id });
        }
    }
}
