using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace ManagementCourse.Common
{
    public class EmailHelper
    {
        private readonly SmtpSettings _smtp;

        public EmailHelper(IOptions<SmtpSettings> smtp)
        {
            _smtp = smtp.Value;
        }
        public async Task SendAsync( string FullName, string Email)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtp.DisplayName, _smtp.Mail));
                email.To.Add(MailboxAddress.Parse(_smtp.MailTo));
                //if (!string.IsNullOrWhiteSpace(cc))
                //{
                //    foreach (var mailcc in cc.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                //    {
                //        if (MailboxAddress.TryParse(mailcc.Trim(), out var addr))
                //        {
                //            email.Cc.Add(addr);
                //        }
                //    }

                //}
                email.Subject = "Thông báo tài khoản mới chờ phê duyệt";
                string ApproveUrl = "https://erp.rtc.edu.vn/rerpweb/usersweb/get-all?status=-1";
                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                
<!DOCTYPE html>

<html lang=""vi"">

<head>

  <meta charset=""UTF-8"" />

  <title>Thông báo tài khoản mới chờ phê duyệt</title>

</head>

<body style=""margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;"">

  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f6f8; padding:24px 0;"">

    <tr>

      <td align=""center"">

        <table width=""650"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #e5e7eb;"">

          

          <!-- Header -->

          <tr>

            <td style=""background-color:#1677ff; padding:18px 24px; color:#ffffff;"">

              <h2 style=""margin:0; font-size:20px; font-weight:600;"">

                Thông báo tài khoản mới chờ phê duyệt

              </h2>

            </td>

          </tr>



          <!-- Content -->

          <tr>

            <td style=""padding:24px; color:#333333; font-size:14px; line-height:1.6;"">

              <p style=""margin-top:0;"">Kính gửi Quản trị viên,</p>



              <p>

                Hệ thống vừa ghi nhận một người dùng mới đã đăng ký tài khoản trên website

                và đang chờ phê duyệt.

              </p>



              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin:20px 0; border-collapse:collapse;"">

                <tr>

                  <td style=""padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600; width:180px;"">

                    Họ và tên

                  </td>

                  <td style=""padding:10px 12px; border:1px solid #e5e7eb;"">

                    {FullName}

                  </td>

                </tr>

                <tr>

                  <td style=""padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600;"">

                    Email

                  </td>

                  <td style=""padding:10px 12px; border:1px solid #e5e7eb;"">

                    {Email}

                  </td>

                </tr>
              </table>



              <p>

                Vui lòng truy cập hệ thống quản trị để kiểm tra thông tin và thực hiện

                phê duyệt hoặc từ chối tài khoản.

              </p>



              <div style=""margin-top:24px; text-align:center;"">

                <a href=""{ApproveUrl}""

                   style=""display:inline-block; background-color:#1677ff; color:#ffffff; text-decoration:none; padding:10px 20px; border-radius:6px; font-weight:600;"">

                  Xem và phê duyệt tài khoản

                </a>

              </div>



              <p style=""margin-top:24px;"">

                Trân trọng,<br />

                <strong>Hệ thống quản lý website</strong>

              </p>

            </td>

          </tr>



          <!-- Footer -->

          <tr>

            <td style=""background-color:#f9fafb; padding:14px 24px; color:#6b7280; font-size:12px; text-align:center; border-top:1px solid #e5e7eb;"">

              Email này được gửi tự động từ hệ thống. Vui lòng không trả lời email này.

            </td>

          </tr>



        </table>

      </td>

    </tr>

  </table>

</body>

</html>";


                email.Body = builder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_smtp.Mail, _smtp.Password);
                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gửi mã OTP đặt lại mật khẩu tới người dùng
        /// </summary>
        public async Task SendOtpAsync(string toEmail, string fullName, string otp)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtp.DisplayName, _smtp.Mail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "[Lưu ý] Mã xác thực đặt lại mật khẩu";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f0f2f5; padding: 40px 20px;'>
    <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.08);'>
        <div style='background: linear-gradient(135deg, #1677ff 0%, #0958d9 100%); padding: 30px; text-align: center;'>
            <h2 style='color: #ffffff; margin: 0; font-size: 22px; font-weight: 700;'>Đặt lại mật khẩu</h2>
            <p style='color: rgba(255,255,255,0.85); margin: 6px 0 0; font-size: 14px;'>RTC – Hệ thống đào tạo trực tuyến</p>
        </div>
        <div style='padding: 40px 30px; color: #333333;'>
            <p style='margin-top: 0; font-size: 16px;'>Chào <b>{fullName}</b>,</p>
            <p style='font-size: 15px; line-height: 1.6; color: #555555;'>
                Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.<br/>
                Vui lòng sử dụng mã xác thực dưới đây để tiếp tục:
            </p>

            <div style='background: #f8f9fa; border: 2px dashed #1677ff; border-radius: 12px; margin: 30px 0; padding: 30px; text-align: center;'>
                <span style='font-size: 40px; font-weight: 800; color: #1677ff; letter-spacing: 10px; display: block;'>{otp}</span>
                <p style='margin-top: 10px; color: #888888; font-size: 13px;'>⏱ Mã OTP có hiệu lực trong <b>10 phút</b></p>
            </div>

            <div style='background-color: #fff7e6; border-left: 4px solid #fa8c16; border-radius: 6px; padding: 12px 16px; margin-bottom: 20px;'>
                <p style='margin: 0; font-size: 13px; color: #d46b08;'>
                    ⚠️ Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này. Mật khẩu của bạn sẽ không bị thay đổi.
                </p>
            </div>
        </div>
        <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;'>
            <p style='margin: 0; font-size: 12px; color: #999999;'>© {DateTime.Now.Year} RTC. Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</div>";

                email.Body = builder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_smtp.Mail, _smtp.Password);
                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
