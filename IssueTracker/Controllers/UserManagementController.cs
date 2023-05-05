using IssueTracker.Data;
using IssueTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Controllers {
    public class UserManagementController : Controller {

        ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagementController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() {
            UserManagementViewModel vm = new UserManagementViewModel();
            vm.Users = _context.Users.ToList();
            vm.Roles = _context.Roles.ToList();
            vm.UserRoles = _context.UserRoles.ToList();
            return View(vm);
        }
    }
}
