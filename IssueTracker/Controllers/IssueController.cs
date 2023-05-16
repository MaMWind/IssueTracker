using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IssueTracker.ViewModels;
using IssueTracker.Data;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IssueTracker.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IssueTracker.Controllers {
    public class IssueController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IssueController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, IAuthorizationService authorization) {
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

        public async Task<IActionResult> CreateIssue(Issue issueModel) {

            if (User.IsInRole(AuthorizationConstants.SupportRole) || User.IsInRole(AuthorizationConstants.DeveloperRole) ||
                User.IsInRole(AuthorizationConstants.AdminRole)) {
                issueModel.Status = Status.Open;
                string id = _userManager.GetUserId(User);
                issueModel.CreatorID = id;
                await _context.Issues.AddAsync(issueModel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new {projectID = issueModel.ProjectID}); 
            }
            return Forbid();
        }

        public async Task<IActionResult> Details(int id) {
            if (User.IsInRole(AuthorizationConstants.SupportRole) || User.IsInRole(AuthorizationConstants.DeveloperRole) ||
                User.IsInRole(AuthorizationConstants.AdminRole)) {
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
            return Forbid();
        }

        public IActionResult Delete(int id) {
            if (User.IsInRole(AuthorizationConstants.SupportRole) || User.IsInRole(AuthorizationConstants.DeveloperRole) ||
                User.IsInRole(AuthorizationConstants.AdminRole)) {
                Issue issue = _context.Issues.First(x => x.IssueID == id);
                int projectId = issue.ProjectID;
                _context.Issues.Remove(issue);
                _context.Messages.RemoveRange(_context.Messages.Where(x => x.IssueID == id));
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { projectID = projectId });
            }
            return Forbid();
        }

        public IActionResult CreateMessage(string text, int issueId) {
            if (User.IsInRole(AuthorizationConstants.SupportRole) || User.IsInRole(AuthorizationConstants.DeveloperRole) ||
                User.IsInRole(AuthorizationConstants.AdminRole)) {
                Message message = new Message();
                message.Text = text;
                message.IssueID = issueId;
                message.CreatorID = _userManager.GetUserId(User);
                message.CreateDate = DateTime.UtcNow;



                Issue issue = _context.Issues.FirstOrDefault(x => x.IssueID == issueId);
                if (issue.Messages == null) {
                    issue.Messages = new List<Message>();
                }
                issue.Messages.Add(message);
                _context.Messages.Add(message);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { id = message.IssueID });
            }
            return Forbid();
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
            if (User.IsInRole(AuthorizationConstants.DeveloperRole) ||
                User.IsInRole(AuthorizationConstants.AdminRole)) {
                _context.Issues.First(x => x.IssueID == id).AssigneeID = _userManager.GetUserId(User);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { id = id });
            }
            return Forbid();
        }

        public IActionResult ChangeStatus(IssueView view) {
            if (User.IsInRole(AuthorizationConstants.DeveloperRole) || 
                    User.IsInRole(AuthorizationConstants.AdminRole)) {
                _context.Issues.First(x=> x.IssueID == view.Id).Status = view.Status;
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { id = view.Id });
            }
            return Forbid();
        }

        public async Task<IActionResult> UploadFile(List<IFormFile> files, int id) {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), id.ToString()));
            var size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files) {
                if (formFile.Length > 0) {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), id.ToString(), formFile.FileName);
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return RedirectToAction(nameof(Details), new { id = id});
        }
    }
}
