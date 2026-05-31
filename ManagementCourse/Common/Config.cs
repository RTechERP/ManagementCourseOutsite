using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Common
{
    public static class Config
    {
        /// <summary>
        /// Môi trường chạy
        /// 1: Môi trường Publish lên server
        /// 0: Môi trường Test trên local
        /// </summary>
        public static IConfiguration? Configuration { get; set; }

        public static int _environment => Configuration?.GetValue<int>("Environment", 0) ?? 0;

        public static string Connection()
        {
            if (_environment == 0)
            {
                return Configuration["ConnectionString"] ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
