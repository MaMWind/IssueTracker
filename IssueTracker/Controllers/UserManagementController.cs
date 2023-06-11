using IssueTracker.Authorization;
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
            var isAdmin = User.IsInRole(AuthorizationConstants.AdminRole);
            if (isAdmin) {
                UserManagementViewModel vm = new UserManagementViewModel();
                vm.Users = _context.Users.ToList();
                vm.Roles = _context.Roles.ToList();
                vm.UserRoles = _context.UserRoles.ToList();
                return View(vm);
            }
            return Forbid();
        }

        public IActionResult Details(string id) {
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.id = id;
            userViewModel.name = _context.Users.First(x => x.Id == id).UserName;
            userViewModel.roles = _context.UserRoles.Where(x => x.UserId == id).ToList();
            return View(userViewModel);
        }

        public async Task<IActionResult> AssignRoles(UserViewModel vm) {
            var user = _context.Users.FirstOrDefault(x => x.Id == vm.id);
            if (vm.developer) {
                await _userManager.AddToRoleAsync(user, AuthorizationConstants.DeveloperRole);
            }
            if (vm.admin) {
                await _userManager.AddToRoleAsync(user, AuthorizationConstants.AdminRole);
            }
            if (vm.support) {
                await _userManager.AddToRoleAsync(user, AuthorizationConstants.SupportRole);
            }

            return RedirectToAction("Index");
        }
    }
}
