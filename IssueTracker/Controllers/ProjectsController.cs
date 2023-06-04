using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
              return _context.Project != null ? 
                          View(await _context.Project.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Project'  is null.");
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ProjectID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectID,Name")] Project project)
        {
            if (User.IsInRole(AuthorizationConstants.AdminRole)) {
                project.CreatorID = _userManager.GetUserId(User);
                if (ModelState.IsValid)
                {
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(project);
            }
            return Forbid();
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.IsInRole(AuthorizationConstants.AdminRole)) {
                if (id == null || _context.Project == null) {
                    return NotFound();
                }

                var project = await _context.Project.FindAsync(id);
                if (project == null) {
                    return NotFound();
                }
                return View(project);
            }
            return Forbid();
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectID,Name,CreatorID")] Project project)
        {
            if (User.IsInRole(AuthorizationConstants.AdminRole)) {
                if (id != project.ProjectID) {
                    return NotFound();
                }

                if (ModelState.IsValid) {
                    try {
                        _context.Update(project);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException) {
                        if (!ProjectExists(project.ProjectID)) {
                            return NotFound();
                        }
                        else {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(project);
            }
            return Forbid();
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ProjectID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.IsInRole(AuthorizationConstants.AdminRole)) {
                if (_context.Project == null) {
                    return Problem("Entity set 'ApplicationDbContext.Project'  is null.");
                }
                var project = await _context.Project.FindAsync(id);
                if (project != null) {
                    _context.Project.Remove(project);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return Forbid();
        }

        private bool ProjectExists(int id)
        {
          return (_context.Project?.Any(e => e.ProjectID == id)).GetValueOrDefault();
        }
    }
}
