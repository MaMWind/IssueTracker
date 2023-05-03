using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IssueTracker.ViewModels;
using IssueTracker.Data;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace IssueTracker.Controllers {
    public class IssueController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public IssueController(ILogger<HomeController> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
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
            issueModel.CreatorID = "Admin";
            _context.Issues.Add(issueModel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), new {projectID = issueModel.ProjectID}); 
        }
    }
}
