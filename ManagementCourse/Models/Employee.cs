using System;
using System.Collections.Generic;

namespace ManagementCourse.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Code { get; set; }

    public string FullName { get; set; }

    public DateTime? BirthOfDate { get; set; }

    public int? Sex { get; set; }

    public string Qualifications { get; set; }

    public string BankAccount { get; set; }

    public string Bhyt { get; set; }

    public string Mst { get; set; }

    public string Bhxh { get; set; }

    public string Cmtnd { get; set; }

    public string JobDescription { get; set; }

    public bool? NhanVienTuVan { get; set; }

    public string Telephone { get; set; }

    public string HandPhone { get; set; }

    public string HomeAddress { get; set; }

    public string Resident { get; set; }

    public string PostalCode { get; set; }

    public int? DepartmentId { get; set; }

    public int? Status { get; set; }

    public string Communication { get; set; }

    public DateTime? PassExpireDate { get; set; }

    public bool? IsCashier { get; set; }

    public int? CashierNo { get; set; }

    public string EmailCom { get; set; }

    public string Email { get; set; }

    public DateTime? StartWorking { get; set; }

    public int? UserGroupId { get; set; }

    public int? UserGroupSxid { get; set; }

    public int? MainViewId { get; set; }

    public string Position { get; set; }

    public bool? IsSetupFunction { get; set; }

    public string ImagePath { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? IsAdminSale { get; set; }

    public int? RoleId { get; set; }

    public int? TeamId { get; set; }

    public int? Leader { get; set; }

    public string IdchamCongCu { get; set; }

    public string IdchamCongMoi { get; set; }

    public string AnhCbnv { get; set; }

    public int? ChucVuHdid { get; set; }

    public int? ChuVuId { get; set; }

    public string DvBhxh { get; set; }

    public string DiaDiemLamViec { get; set; }

    public string NoiSinh { get; set; }

    public int? GioiTinh { get; set; }

    public string DanToc { get; set; }

    public string TonGiao { get; set; }

    public string QuocTich { get; set; }

    public int? TinhTrangHonNhanId { get; set; }

    public string SoCmtnd { get; set; }

    public DateTime? NgayCap { get; set; }

    public string NoiCap { get; set; }

    public string DcThuongTru { get; set; }

    public string DcTamTru { get; set; }

    public string SdtcaNhan { get; set; }

    public string EmailCaNhan { get; set; }

    public string SdtcongTy { get; set; }

    public string EmailCongTy { get; set; }

    public string NguoiLienHeKhiCan { get; set; }

    public string MoiQuanHe { get; set; }

    public string SdtnguoiThan { get; set; }

    public int? LoaiHdldid { get; set; }

    public string TinhTrangKyHd { get; set; }

    public DateTime? NgayBatDauHd { get; set; }

    public DateTime? NgayKetThucHd { get; set; }

    public string SoHd { get; set; }

    public string SoSoBhxh { get; set; }

    public int? NguoiGiuSoBhxh { get; set; }

    public DateTime? NgayBatDauBhxhcty { get; set; }

    public decimal? MucDongBhxhhienTai { get; set; }

    public decimal? LuongThuViec { get; set; }

    public decimal? LuongCoBan { get; set; }

    public decimal? AnCa { get; set; }

    public decimal? XangXe { get; set; }

    public decimal? DienThoai { get; set; }

    public decimal? NhaO { get; set; }

    public decimal? TrangPhuc { get; set; }

    public decimal? ChuyenCan { get; set; }

    public decimal? Khac { get; set; }

    public decimal? TongPhuCap { get; set; }

    public decimal? TongLuong { get; set; }

    public decimal? GiamTruBanThan { get; set; }

    public int? SoNguoiPt { get; set; }

    public decimal? TongTien { get; set; }

    public string StkchuyenLuong { get; set; }

    public bool? Syll { get; set; }

    public bool? GiayKs { get; set; }

    public bool? CmndorCccd { get; set; }

    public bool? SoHk { get; set; }

    public bool? Xnns { get; set; }

    public bool? BangCap { get; set; }

    public bool? Cv { get; set; }

    public bool? Dxv { get; set; }

    public bool? CamKetTs { get; set; }

    public bool? ToTrinhTd { get; set; }

    public bool? ThuMoiNhanViec { get; set; }

    public bool? Qdtd { get; set; }

    public bool? Hdtv { get; set; }

    public bool? Dgtv { get; set; }

    public bool? Hdldxdth { get; set; }

    public bool? DgchuyenHd { get; set; }

    public bool? Hdldkxdth { get; set; }

    public bool? TinhTrangCapDongPhuc { get; set; }

    public bool? GiayKsk { get; set; }

    public DateTime? NgayBatDauThuViec { get; set; }

    public DateTime? NgayKetThucThuViec { get; set; }

    public string SoHdtv { get; set; }

    public DateTime? NgayBatDauHdxdth { get; set; }

    public DateTime? NgayKetThucHdxdth { get; set; }

    public string SoHdxdth { get; set; }

    public DateTime? NgayHieuLucHdkxdth { get; set; }

    public string SoHdkxdth { get; set; }

    public DateTime? NgayBatDauBhxh { get; set; }

    public DateTime? NgayKetThucBhxh { get; set; }

    public string SoNhaDcThuongTru { get; set; }

    public string DuongDcThuongTru { get; set; }

    public string PhuongDcThuongTru { get; set; }

    public string QuanDcThuongTru { get; set; }

    public string TinhDcThuongTru { get; set; }

    public string SoNhaDcTamTru { get; set; }

    public string DuongDcTamTru { get; set; }

    public string PhuongDcTamTru { get; set; }

    public string QuanDcTamTru { get; set; }

    public string TinhDcTamTru { get; set; }

    public bool? Hdldxdthyear { get; set; }

    public bool? DgchuyenHdyear { get; set; }

    public DateTime? EndWorking { get; set; }

    public string ReasonDeleted { get; set; }

    public string CodeOld { get; set; }

    public int? ProjectTypeId { get; set; }

    public int? EmployeeTeamId { get; set; }

    public bool? GiayXacNhanCuTru { get; set; }

    public string UserZaloId { get; set; }

    public int? TaxCompanyId { get; set; }

    public string NguoiLienHeKhiCan2 { get; set; }

    public string MoiQuanHe2 { get; set; }

    public string SdtnguoiThan2 { get; set; }

    public bool? IsExcludedFromSalary { get; set; }
}
