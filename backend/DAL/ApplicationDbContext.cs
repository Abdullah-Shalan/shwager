using Microsoft.EntityFrameworkCore;
using Final_Project.Entities;

namespace Final_Project.DAL
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Hr> Hrs { get; set; }
        public DbSet<CandidateTask> CandidateTasks { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobTask> JobTasks { get; set; }

    }
}