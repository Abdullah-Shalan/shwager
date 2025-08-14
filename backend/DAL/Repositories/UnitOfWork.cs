using Final_Project.Entities;

namespace Final_Project.DAL.Repositories
{
    public class UnitOfWork : IDisposable
    {
        public readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private GenericRepository<Hr> HrRepository = null!;
        private GenericRepository<Candidate> CandidateRepository = null!;
        private GenericRepository<CandidateTask> CandidateTaskRepository = null!;
        private GenericRepository<Job> JobRepository = null!;
        private GenericRepository<JobTask> JobTaskRepository = null!;
        
        public GenericRepository<Hr> Hrs
        {
            get
            {
                if (this.HrRepository == null)
                {
                    HrRepository = new GenericRepository<Hr>(_context);
                }
                return HrRepository;
            }
        }

        public GenericRepository<Candidate> Candidates
        {
            get
            {
                if (this.CandidateRepository == null)
                {
                    CandidateRepository = new GenericRepository<Candidate>(_context);
                }
                return CandidateRepository;
            }
        }
        public GenericRepository<CandidateTask> CandidateTasks
        {
            get
            {
                if (this.CandidateTaskRepository == null)
                {
                    CandidateTaskRepository = new GenericRepository<CandidateTask>(_context);
                }
                return CandidateTaskRepository;
            }
        }
        public GenericRepository<Job> Jobs
        {
            get
            {
                if (this.JobRepository == null)
                {
                    JobRepository = new GenericRepository<Job>(_context);
                }
                return JobRepository;
            }
        }
        public GenericRepository<JobTask> JobTasks
        {
            get
            {
                if (this.JobTaskRepository == null)
                {
                    JobTaskRepository = new GenericRepository<JobTask>(_context);
                }
                return JobTaskRepository;
            }
        }
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}