using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexiconLMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LexiconLMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<ApplicationUser> AppUser { get; set; }
        public DbSet<Module> Module { get; set; }

        public DbSet<ModuleActivity> ModuleActivity { get; set; }

        public DbSet<Document> Documents { get; set; }



        // ---------------------------------------------------- To Solve this problem ----------------------------------------------------------------------------
        // ALTER TABLE[Documents] ADD CONSTRAINT[FK_Documents_Courses_CourseId] FOREIGN KEY([CourseId]) REFERENCES[Courses] ([CourseId]) ON DELETE CASCADE;      |
        // Failed executing DbCommand(10ms) [Parameters=[], CommandType='Text', CommandTimeout='30']                                                             |
        // ALTER TABLE[Documents] ADD CONSTRAINT[FK_Documents_Courses_CourseId] FOREIGN KEY([CourseId]) REFERENCES[Courses] ([CourseId]) ON DELETE CASCADE;      |
        // ----------------------------------------------VVV--- Write this code ---VVV----------------------------------------------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>()
                .HasOne(b => b.Course)
                .WithMany(a => a.Documents)
                .HasForeignKey(b => b.CourseId)  // .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasOne(b => b.Module)
                .WithMany(a => a.Documents)
                .HasForeignKey(b => b.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(b => b.ModuleActivity)
                .WithMany(a => a.Documents)
                .HasForeignKey(b => b.ModuleActivityId)
                .OnDelete(DeleteBehavior.Restrict);

        }
        // ---------------------------------------------------------------------------------------------------


    }
}
