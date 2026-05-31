using System;
using System.Collections.Generic;
using ManagementCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementCourse.Models.Context;

public partial class RTCCourseContext : DbContext
{
    public RTCCourseContext(DbContextOptions<RTCCourseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseAnswer> CourseAnswers { get; set; }

    public virtual DbSet<CourseCatalog> CourseCatalogs { get; set; }

    public virtual DbSet<CourseCatalogType> CourseCatalogTypes { get; set; }

    public virtual DbSet<CourseExam> CourseExams { get; set; }

    public virtual DbSet<CourseExamEvaluate> CourseExamEvaluates { get; set; }

    public virtual DbSet<CourseExamPractice> CourseExamPractices { get; set; }

    public virtual DbSet<CourseExamResult> CourseExamResults { get; set; }

    public virtual DbSet<CourseExamResultDetail> CourseExamResultDetails { get; set; }

    public virtual DbSet<CourseFile> CourseFiles { get; set; }

    public virtual DbSet<CourseLesson> CourseLessons { get; set; }

    public virtual DbSet<CourseLessonHistory> CourseLessonHistories { get; set; }

    public virtual DbSet<CourseQuestion> CourseQuestions { get; set; }

    public virtual DbSet<CourseRightAnswer> CourseRightAnswers { get; set; }

    public virtual DbSet<CourseType> CourseTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigSystem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ConfigSy__3214EC279D66AA65");

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
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC271B172364");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseAn__3214EC270E29A3BC");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseCatalog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseCa__3214EC27DF7AA4BC");

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

        modelBuilder.Entity<CourseCatalogType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseCa__3214EC27CA2644FB");

            entity.ToTable("CourseCatalogType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseCatalogTypeCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CourseCatalogTypeName).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEx__3214EC27E2DC27FA");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseEx__3214EC271239B65A");

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

        modelBuilder.Entity<CourseExamPractice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEx__3214EC27074C3220");

            entity.ToTable("CourseExamPractice");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.PracticePoints).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseExamResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEx__3214EC2779401D97");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseEx__3214EC2798935EC2");

            entity.ToTable("CourseExamResultDetail");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseFi__3214EC270F8A4850");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseLe__3214EC27B3E35AEA");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseLe__3214EC27BB11B0AB");

            entity.ToTable("CourseLessonHistory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ViewDate).HasColumnType("datetime");
            entity.Property(e => e.WatchedPercent).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<CourseQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseQu__3214EC27FE04910F");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseRi__3214EC27098CCF56");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseAnswerId).HasColumnName("CourseAnswerID");
            entity.Property(e => e.CourseQuestionId).HasColumnName("CourseQuestionID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseTy__3214EC2729925C9C");

            entity.ToTable("CourseType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseTypeCode)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CourseTypeName).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC270CFD6ECD");

            entity.ToTable("Employee");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AnCa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AnhCbnv)
                .HasColumnType("ntext")
                .HasColumnName("AnhCBNV");
            entity.Property(e => e.BankAccount).HasMaxLength(550);
            entity.Property(e => e.Bhxh)
                .HasMaxLength(550)
                .HasColumnName("BHXH");
            entity.Property(e => e.Bhyt)
                .HasMaxLength(550)
                .HasColumnName("BHYT");
            entity.Property(e => e.BirthOfDate).HasColumnType("datetime");
            entity.Property(e => e.ChuVuId).HasColumnName("ChuVuID");
            entity.Property(e => e.ChucVuHdid).HasColumnName("ChucVuHDID");
            entity.Property(e => e.ChuyenCan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CmndorCccd).HasColumnName("CMNDorCCCD");
            entity.Property(e => e.Cmtnd)
                .HasMaxLength(550)
                .HasColumnName("CMTND");
            entity.Property(e => e.Code).HasMaxLength(550);
            entity.Property(e => e.CodeOld).HasMaxLength(550);
            entity.Property(e => e.Communication).HasMaxLength(550);
            entity.Property(e => e.CreatedBy).HasMaxLength(550);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Cv).HasColumnName("CV");
            entity.Property(e => e.DanToc).HasMaxLength(550);
            entity.Property(e => e.DcTamTru).HasMaxLength(550);
            entity.Property(e => e.DcThuongTru).HasMaxLength(550);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DgchuyenHd).HasColumnName("DGChuyenHD");
            entity.Property(e => e.DgchuyenHdyear).HasColumnName("DGChuyenHDYear");
            entity.Property(e => e.Dgtv).HasColumnName("DGTV");
            entity.Property(e => e.DiaDiemLamViec).HasMaxLength(550);
            entity.Property(e => e.DienThoai).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DuongDcTamTru).HasMaxLength(150);
            entity.Property(e => e.DuongDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.DvBhxh)
                .HasMaxLength(550)
                .HasColumnName("DvBHXH");
            entity.Property(e => e.Dxv).HasColumnName("DXV");
            entity.Property(e => e.Email).HasMaxLength(550);
            entity.Property(e => e.EmailCaNhan).HasMaxLength(550);
            entity.Property(e => e.EmailCom).HasMaxLength(550);
            entity.Property(e => e.EmailCongTy).HasMaxLength(550);
            entity.Property(e => e.EmployeeTeamId).HasColumnName("EmployeeTeamID");
            entity.Property(e => e.EndWorking).HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(550);
            entity.Property(e => e.GiamTruBanThan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiayKs).HasColumnName("GiayKS");
            entity.Property(e => e.GiayKsk).HasColumnName("GiayKSK");
            entity.Property(e => e.HandPhone).HasMaxLength(550);
            entity.Property(e => e.Hdldkxdth).HasColumnName("HDLDKXDTH");
            entity.Property(e => e.Hdldxdth).HasColumnName("HDLDXDTH");
            entity.Property(e => e.Hdldxdthyear).HasColumnName("HDLDXDTHYear");
            entity.Property(e => e.Hdtv).HasColumnName("HDTV");
            entity.Property(e => e.HomeAddress).HasMaxLength(550);
            entity.Property(e => e.IdchamCongCu)
                .HasMaxLength(550)
                .HasColumnName("IDChamCongCu");
            entity.Property(e => e.IdchamCongMoi)
                .HasMaxLength(550)
                .HasColumnName("IDChamCongMoi");
            entity.Property(e => e.ImagePath).HasMaxLength(550);
            entity.Property(e => e.IsAdminSale).HasColumnName("isAdminSale");
            entity.Property(e => e.JobDescription).HasMaxLength(550);
            entity.Property(e => e.Khac).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiHdldid).HasColumnName("LoaiHDLDID");
            entity.Property(e => e.LuongCoBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LuongThuViec).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MainViewId).HasColumnName("MainViewID");
            entity.Property(e => e.MoiQuanHe).HasMaxLength(150);
            entity.Property(e => e.MoiQuanHe2).HasMaxLength(250);
            entity.Property(e => e.Mst)
                .HasMaxLength(550)
                .HasColumnName("MST");
            entity.Property(e => e.MucDongBhxhhienTai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("MucDongBHXHHienTai");
            entity.Property(e => e.NgayBatDauBhxh)
                .HasColumnType("datetime")
                .HasColumnName("NgayBatDauBHXH");
            entity.Property(e => e.NgayBatDauBhxhcty)
                .HasColumnType("datetime")
                .HasColumnName("NgayBatDauBHXHCty");
            entity.Property(e => e.NgayBatDauHd)
                .HasColumnType("datetime")
                .HasColumnName("NgayBatDauHD");
            entity.Property(e => e.NgayBatDauHdxdth)
                .HasColumnType("datetime")
                .HasColumnName("NgayBatDauHDXDTH");
            entity.Property(e => e.NgayBatDauThuViec).HasColumnType("datetime");
            entity.Property(e => e.NgayCap).HasColumnType("datetime");
            entity.Property(e => e.NgayHieuLucHdkxdth)
                .HasColumnType("datetime")
                .HasColumnName("NgayHieuLucHDKXDTH");
            entity.Property(e => e.NgayKetThucBhxh)
                .HasColumnType("datetime")
                .HasColumnName("NgayKetThucBHXH");
            entity.Property(e => e.NgayKetThucHd)
                .HasColumnType("datetime")
                .HasColumnName("NgayKetThucHD");
            entity.Property(e => e.NgayKetThucHdxdth)
                .HasColumnType("datetime")
                .HasColumnName("NgayKetThucHDXDTH");
            entity.Property(e => e.NgayKetThucThuViec).HasColumnType("datetime");
            entity.Property(e => e.NguoiGiuSoBhxh).HasColumnName("NguoiGiuSoBHXH");
            entity.Property(e => e.NguoiLienHeKhiCan).HasMaxLength(150);
            entity.Property(e => e.NguoiLienHeKhiCan2).HasMaxLength(250);
            entity.Property(e => e.NhaO).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NoiCap).HasMaxLength(150);
            entity.Property(e => e.NoiSinh).HasMaxLength(550);
            entity.Property(e => e.PassExpireDate).HasColumnType("datetime");
            entity.Property(e => e.PhuongDcTamTru).HasMaxLength(150);
            entity.Property(e => e.PhuongDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.Position).HasMaxLength(550);
            entity.Property(e => e.PostalCode).HasMaxLength(550);
            entity.Property(e => e.ProjectTypeId).HasColumnName("ProjectTypeID");
            entity.Property(e => e.Qdtd).HasColumnName("QDTD");
            entity.Property(e => e.Qualifications).HasMaxLength(550);
            entity.Property(e => e.QuanDcTamTru).HasMaxLength(150);
            entity.Property(e => e.QuanDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.QuocTich).HasMaxLength(550);
            entity.Property(e => e.Resident).HasMaxLength(550);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.SdtcaNhan)
                .HasMaxLength(50)
                .HasColumnName("SDTCaNhan");
            entity.Property(e => e.SdtcongTy)
                .HasMaxLength(50)
                .HasColumnName("SDTCongTy");
            entity.Property(e => e.SdtnguoiThan)
                .HasMaxLength(50)
                .HasColumnName("SDTNguoiThan");
            entity.Property(e => e.SdtnguoiThan2)
                .HasMaxLength(150)
                .HasColumnName("SDTNguoiThan2");
            entity.Property(e => e.SoCmtnd)
                .HasMaxLength(250)
                .HasColumnName("SoCMTND");
            entity.Property(e => e.SoHd)
                .HasMaxLength(150)
                .HasColumnName("SoHD");
            entity.Property(e => e.SoHdkxdth)
                .HasMaxLength(100)
                .HasColumnName("SoHDKXDTH");
            entity.Property(e => e.SoHdtv)
                .HasMaxLength(100)
                .HasColumnName("SoHDTV");
            entity.Property(e => e.SoHdxdth)
                .HasMaxLength(100)
                .HasColumnName("SoHDXDTH");
            entity.Property(e => e.SoHk).HasColumnName("SoHK");
            entity.Property(e => e.SoNguoiPt).HasColumnName("SoNguoiPT");
            entity.Property(e => e.SoNhaDcTamTru).HasMaxLength(150);
            entity.Property(e => e.SoNhaDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.SoSoBhxh)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("SoSoBHXH");
            entity.Property(e => e.StartWorking).HasColumnType("datetime");
            entity.Property(e => e.StkchuyenLuong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STKChuyenLuong");
            entity.Property(e => e.Syll).HasColumnName("SYLL");
            entity.Property(e => e.TaxCompanyId).HasColumnName("TaxCompanyID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.Telephone).HasMaxLength(550);
            entity.Property(e => e.TinhDcTamTru).HasMaxLength(150);
            entity.Property(e => e.TinhDcThuongTru).HasMaxLength(150);
            entity.Property(e => e.TinhTrangHonNhanId).HasColumnName("TinhTrangHonNhanID");
            entity.Property(e => e.TinhTrangKyHd)
                .HasMaxLength(150)
                .HasColumnName("TinhTrangKyHD");
            entity.Property(e => e.ToTrinhTd).HasColumnName("ToTrinhTD");
            entity.Property(e => e.TonGiao).HasMaxLength(550);
            entity.Property(e => e.TongLuong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongPhuCap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangPhuc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(550);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserGroupId).HasColumnName("UserGroupID");
            entity.Property(e => e.UserGroupSxid).HasColumnName("UserGroupSXID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserZaloId)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("UserZaloID");
            entity.Property(e => e.XangXe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Xnns).HasColumnName("XNNS");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27AAD814E3");

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
