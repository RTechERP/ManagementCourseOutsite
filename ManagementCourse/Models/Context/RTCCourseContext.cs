using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ManagementCourse.Models;

#nullable disable

namespace ManagementCourse.Models.Context
{
    public partial class RTCCourseContext : DbContext
    {
        public RTCCourseContext()
        {
        }

        public RTCCourseContext(DbContextOptions<RTCCourseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseAnswer> CourseAnswers { get; set; }
        public virtual DbSet<CourseCatalog> CourseCatalogs { get; set; }
        public virtual DbSet<CourseExam> CourseExams { get; set; }
        public virtual DbSet<CourseExamEvaluate> CourseExamEvaluates { get; set; }
        public virtual DbSet<CourseExamResult> CourseExamResults { get; set; }
        public virtual DbSet<CourseExamResultDetail> CourseExamResultDetails { get; set; }
        public virtual DbSet<CourseFile> CourseFiles { get; set; }
        public virtual DbSet<CourseLesson> CourseLessons { get; set; }
        public virtual DbSet<CourseLessonHistory> CourseLessonHistories { get; set; }
        public virtual DbSet<CourseQuestion> CourseQuestions { get; set; }
        public virtual DbSet<CourseRightAnswer> CourseRightAnswers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server= 192.168.1.3,9000;database=RTCCourse;User Id = sa; Password=1;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ConfigSystem>(entity =>
            {
                entity.ToTable("ConfigSystem");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.KeyName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.KeyValue1).HasMaxLength(200);

                entity.Property(e => e.KeyValue10).HasMaxLength(200);

                entity.Property(e => e.KeyValue2).HasMaxLength(200);

                entity.Property(e => e.KeyValue3).HasMaxLength(200);

                entity.Property(e => e.KeyValue4).HasMaxLength(200);

                entity.Property(e => e.KeyValue5).HasMaxLength(200);

                entity.Property(e => e.KeyValue6).HasMaxLength(200);

                entity.Property(e => e.KeyValue7).HasMaxLength(200);

                entity.Property(e => e.KeyValue8).HasMaxLength(200);

                entity.Property(e => e.KeyValue9).HasMaxLength(200);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CourseCatalogId).HasColumnName("CourseCatalogID");

                entity.Property(e => e.CourseCopyId).HasColumnName("CourseCopyID");

                entity.Property(e => e.CourseTypeId).HasColumnName("CourseTypeID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.FileCourseId).HasColumnName("FileCourseID");

                entity.Property(e => e.Instructor).HasMaxLength(200);

                entity.Property(e => e.LeadTime).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NameCourse).HasMaxLength(300);

                entity.Property(e => e.QuestionCount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.QuestionDuration).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Stt).HasColumnName("STT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseAnswer>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseCatalog>(entity =>
            {
                entity.ToTable("CourseCatalog");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Stt).HasColumnName("STT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseExam>(entity =>
            {
                entity.ToTable("CourseExam");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CodeExam)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Goal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LessonId).HasColumnName("LessonID");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseExamEvaluate>(entity =>
            {
                entity.ToTable("CourseExamEvaluate");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CourseExamResultId).HasColumnName("CourseExamResultID");

                entity.Property(e => e.CourseQuestionId).HasColumnName("CourseQuestionID");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateCompleted).HasColumnType("datetime");

                entity.Property(e => e.DateEvaluate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Point).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseExamResult>(entity =>
            {
                entity.ToTable("CourseExamResult");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LessonId).HasColumnName("LessonID");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PercentageCorrect).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PracticePoints).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseExamResultDetail>(entity =>
            {
                entity.ToTable("CourseExamResultDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseFile>(entity =>
            {
                entity.ToTable("CourseFile");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LessonId).HasColumnName("LessonID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseLesson>(entity =>
            {
                entity.ToTable("CourseLesson");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.FileCourseId).HasColumnName("FileCourseID");

                entity.Property(e => e.LessonCopyId).HasColumnName("LessonCopyID");

                entity.Property(e => e.LessonTitle).HasMaxLength(400);

                entity.Property(e => e.RequiredWatchedPercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Stt).HasColumnName("STT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UrlPdf).HasColumnName("UrlPDF");

                entity.Property(e => e.VideoUrl).HasColumnName("VideoURL");
            });

            modelBuilder.Entity<CourseLessonHistory>(entity =>
            {
                entity.ToTable("CourseLessonHistory");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ViewDate).HasColumnType("datetime");

                entity.Property(e => e.WatchedPercent).HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<CourseQuestion>(entity =>
            {
                entity.ToTable("CourseQuestion");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Stt).HasColumnName("STT");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseRightAnswer>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CourseAnswerId).HasColumnName("CourseAnswerID");

                entity.Property(e => e.CourseQuestionId).HasColumnName("CourseQuestionID");

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BankAccount).HasMaxLength(250);

                entity.Property(e => e.Bhxh)
                    .HasMaxLength(250)
                    .HasColumnName("BHXH");

                entity.Property(e => e.Bhyt)
                    .HasMaxLength(250)
                    .HasColumnName("BHYT");

                entity.Property(e => e.BirthOfDate).HasColumnType("datetime");

                entity.Property(e => e.Cmtnd)
                    .HasMaxLength(250)
                    .HasColumnName("CMTND");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Communication).HasMaxLength(100);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.EmailCom).HasMaxLength(250);

                entity.Property(e => e.FullName).HasMaxLength(250);

                entity.Property(e => e.HandPhone).HasMaxLength(100);

                entity.Property(e => e.HomeAddress).HasMaxLength(100);

                entity.Property(e => e.ImagePath).HasColumnType("ntext");

                entity.Property(e => e.IsAdminSale).HasColumnName("isAdminSale");

                entity.Property(e => e.JobDescription).HasMaxLength(200);

                entity.Property(e => e.LoginName).HasMaxLength(50);

                entity.Property(e => e.MainViewId).HasColumnName("MainViewID");

                entity.Property(e => e.Mst)
                    .HasMaxLength(250)
                    .HasColumnName("MST");

                entity.Property(e => e.Organization)
                    .HasMaxLength(550)
                    .HasComment("Đơn vị công tác");

                entity.Property(e => e.PassExpireDate).HasColumnType("datetime");

                entity.Property(e => e.PasswordHash).HasMaxLength(250);

                entity.Property(e => e.PinPassword).HasMaxLength(255);

                entity.Property(e => e.Position)
                    .HasMaxLength(550)
                    .HasComment("Vị trí");

                entity.Property(e => e.PostalCode).HasMaxLength(50);

                entity.Property(e => e.Qualifications).HasMaxLength(250);

                entity.Property(e => e.ReferralSource)
                    .HasMaxLength(550)
                    .HasComment("Nguồn giới thiệu");

                entity.Property(e => e.Resident).HasMaxLength(100);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.StartWorking).HasColumnType("datetime");

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.Telephone).HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserGroupId).HasColumnName("UserGroupID");

                entity.Property(e => e.UserGroupSxid).HasColumnName("UserGroupSXID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
