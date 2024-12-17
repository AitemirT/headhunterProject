using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Models;

public class JobSearchDbContext : IdentityDbContext<MyUser, IdentityRole<int>, int>
{
    public DbSet<MyUser> Users { get; set; }

    public DbSet<Cv> Cvs { get; set; }

    public DbSet<WorkExperience> WorkExperiences { get; set; }
    
    public DbSet<Education> Educations { get; set; }

    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Respond> Responds { get; set; }
    public JobSearchDbContext(DbContextOptions<JobSearchDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Respond>()
            .HasOne(r => r.Chat)
            .WithOne(c => c.Respond)
            .HasForeignKey<Respond>(r => r.ChatId);
    }
}