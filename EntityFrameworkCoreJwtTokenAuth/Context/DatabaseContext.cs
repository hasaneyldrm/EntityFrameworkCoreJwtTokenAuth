using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreJwtTokenAuth.Models.Database;

namespace EntityFrameworkCoreJwtTokenAuth.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options: options)
        {

        }

        public DbSet<Educator> Educator { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Company> Company { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Company>(buildAction: builder =>
            {

                builder.ToTable(name: "companies");
                builder.Property(propertyExpression: p => p.Id)
                    .ValueGeneratedOnAdd();
                builder.Property(propertyExpression: p => p.Id).HasColumnName("id");

                builder.OwnsOne(navigationExpression: c => c.Timestamps,
                    buildAction: a =>
                    {

                        a.Property(propertyExpression: p => p.CreatedAt).HasColumnName(name: "createdAt");
                        a.Property(propertyExpression: p => p.UpdatedAt).HasColumnName(name: "updatedAt");
                    });
            });


            model.Entity<Educator>(buildAction: builder =>
            {

                builder.ToTable(name: "educators");
                builder.Property(propertyExpression: p => p.Id)
                    .ValueGeneratedOnAdd();
                builder.Property(propertyExpression: p => p.Id).HasColumnName("id");

                builder.OwnsOne(navigationExpression: c => c.Timestamps,
                    buildAction: a =>
                    {
                        a.Property(propertyExpression: p => p.CreatedAt).HasColumnName(name: "createdAt");
                        a.Property(propertyExpression: p => p.UpdatedAt).HasColumnName(name: "updatedAt");
                    });
            });

            model.Entity<Student>(buildAction: builder =>
            {
                builder.ToTable(name: "students");
                builder.Property(propertyExpression: p => p.Id)
                    .ValueGeneratedOnAdd();
                builder.Property(propertyExpression: p => p.Id).HasColumnName("id");

                builder.OwnsOne(navigationExpression: c => c.Class,
                    buildAction: a =>
                    {
                        a.Property(propertyExpression: p => p.Branch).HasColumnName(name: "branch");
                        a.Property(propertyExpression: p => p.Group).HasColumnName(name: "group");
                        a.Property(propertyExpression: p => p.Period).HasColumnName(name: "period");
                    });

                builder.OwnsOne(navigationExpression: c => c.Exam,
                    buildAction: a =>
                    {
                        a.Property(propertyExpression: p => p.Exact).HasColumnName(name: "exactExamDate");
                        a.Property(propertyExpression: p => p.Soonest).HasColumnName(name: "soonestExamDate");
                    });

                builder.Property(propertyExpression: p => p.Debt).HasColumnName(name: "debt");
                builder.OwnsOne(navigationExpression: c => c.Sessions,
                    buildAction: a =>
                    {
                        a.Property(propertyExpression: p => p.Selectable).HasColumnName(name: "remainingSelectableSessions");
                        a.Property(propertyExpression: p => p.Remaining).HasColumnName(name: "remainingTotalSessions");
                        a.Property(propertyExpression: p => p.Past).HasColumnName(name: "pastSessions");
                    });

                builder.OwnsOne(navigationExpression: c => c.Timestamps,
                    buildAction: a =>
                    {
                        a.Property(propertyExpression: p => p.CreatedAt).HasColumnName(name: "createdAt");
                        a.Property(propertyExpression: p => p.UpdatedAt).HasColumnName(name: "updatedAt");
                    });
            });
        }

    }
}
