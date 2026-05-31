using ManagementCourse.Common;
using Microsoft.EntityFrameworkCore;

namespace ManagementCourse.Models.Context
{
    public partial class RTCCourseContext
    {
        public RTCCourseContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.Connection());
            }
        }
    }
}
