using RLA.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RLA.Infrastructure.Data
{
    public class ElearningPlatformDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ElearningPlatformDbContext(DbContextOptions<ElearningPlatformDbContext> options) : base(options) { }

        public DbSet<Term> Terms { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizSubmission> QuizSubmissions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<ExpiredToken> ExpiredTokens { get; set; }
        public DbSet<CourseDiscussion> CourseDiscussions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Course relationships
            builder.Entity<Course>()
                .HasOne(c => c.Level)
                .WithMany()
                .HasForeignKey(c => c.LevelId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a level if courses reference it

            builder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.SetNull);

            // SetNull allow deleting a professor if courses reference it, and does not delete courses
            // Cascade allows deleting a professor if courses reference it, and deletes these courses
            // Resetrict Prevent deleting a professor if courses reference it


            builder.Entity<Course>()
                .HasOne(c => c.Term)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TermId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a term if courses reference it

            builder.Entity<Course>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Course>()
                .Property(c => c.Code)
                .HasMaxLength(20);

            builder.Entity<Course>()
                .Property(c => c.Overview)
                .HasMaxLength(500);

            builder.Entity<Course>()
                .Property(c => c.Location)
                .HasMaxLength(200)
                .IsRequired();

            builder.Entity<Course>()
                .Property(c => c.DayOfWeek)
                .IsRequired();

            builder.Entity<Course>()
                .Property(c => c.Time)
                .IsRequired();

            builder.Entity<Course>()
                .Property(c => c.CreditHours)
                .IsRequired();

            // CourseDiscussion relationships
            builder.Entity<CourseDiscussion>()
                .HasOne(cd => cd.Course)
                .WithMany(c => c.Discussions)
                .HasForeignKey(cd => cd.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Delete discussions if the course is deleted

            builder.Entity<CourseDiscussion>()
                .HasOne(cd => cd.Professor)
                .WithMany()
                .HasForeignKey(cd => cd.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict Prevent deleting a professor if discussions exist

            builder.Entity<CourseDiscussion>()
                .Property(cd => cd.Message)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Entity<CourseDiscussion>()
                .Property(cd => cd.PostedAt)
                .HasColumnType("datetime")
                .IsRequired();

            // Term relationships
            builder.Entity<Term>()
                .HasMany(t => t.Courses)
                .WithOne(c => c.Term)
                .HasForeignKey(c => c.TermId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Term>()
                .HasMany(t => t.Quizzes)
                .WithOne(q => q.Term)
                .HasForeignKey(q => q.TermId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Term>()
                .HasMany(t => t.StudentCourses)
                .WithOne(sc => sc.Term)
                .HasForeignKey(sc => sc.TermId)
                .OnDelete(DeleteBehavior.Restrict);

            // Section relationships
            builder.Entity<Section>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Section>()
                .HasOne(s => s.Assistant)
                .WithMany(a => a.Sections)
                .HasForeignKey(s => s.AssistantId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentCourse relationships
            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Section)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quiz relationships
            builder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuizSubmission relationships
            builder.Entity<QuizSubmission>()
                .HasOne(qs => qs.Student)
                .WithMany(s => s.QuizSubmissions)
                .HasForeignKey(qs => qs.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuizSubmission>()
                .HasOne(qs => qs.Quiz)
                .WithMany(q => q.QuizSubmissions)
                .HasForeignKey(qs => qs.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question relationships
            builder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Answer relationships
            builder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuizAnswer relationships
            builder.Entity<QuizAnswer>()
                .HasOne(qa => qa.QuizSubmission)
                .WithMany(qs => qs.QuizAnswers)
                .HasForeignKey(qa => qa.QuizSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<QuizAnswer>()
                .HasOne(qa => qa.Question)
                .WithMany(q => q.QuizAnswers)
                .HasForeignKey(qa => qa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuizAnswer>()
                .HasOne(qa => qa.SelectedAnswer)
                .WithMany(a => a.QuizAnswers)
                .HasForeignKey(qa => qa.SelectedAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Material relationships
            builder.Entity<Material>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Materials)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}