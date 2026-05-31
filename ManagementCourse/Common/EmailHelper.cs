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

    }
}
