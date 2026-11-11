using Microsoft.EntityFrameworkCore;
using CRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CRM.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobApplication> Applications => Set<JobApplication>();


}