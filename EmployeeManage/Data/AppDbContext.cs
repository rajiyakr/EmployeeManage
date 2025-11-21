using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeeManage.Models;
namespace EmployeeManage.Data
{
    public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Models.Task>().ToTable("Tasks");
            modelBuilder.Entity<Message>().ToTable("Messages");
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
