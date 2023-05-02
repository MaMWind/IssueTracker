using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;

namespace IssueTracker.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }
        public DbSet<IssueTracker.Models.Project>? Project { get; set; }
        public DbSet<IssueTracker.Models.Issue>? Issues { get; set; }
        public DbSet<IssueTracker.Models.Message>? Messages { get; set; }
    }
}