using ManagementCourse.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Reposiory
{
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>
    {
        /// <summary>
        /// Lấy token hợp lệ: chưa dùng và chưa hết hạn
        /// </summary>
        public PasswordResetToken GetValidToken(string hashedToken)
        {
            return GetAll(x =>
                x.Token == hashedToken &&
                x.IsUsed == false &&
                x.ExpiredAt > DateTime.Now
            ).FirstOrDefault();
        }

        /// <summary>
        /// Vô hiệu hóa tất cả token cũ của user (chưa dùng)
        /// </summary>
        public async Task InvalidateOldTokens(int userId)
        {
            var oldTokens = GetAll(x => x.UserId == userId && x.IsUsed == false).ToList();
            foreach (var token in oldTokens)
            {
                token.IsUsed = true;
                token.UpdatedDate = DateTime.Now;
                await UpdateAsync(token);
            }
        }
    }
}
