using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Config
{
    public static class RoleConfig
    {
        public static string Admin()
        {
            return "Admin";
        }
        public static string Ministry()
        {
            return "Ministry";
        }
        public static string Teacher()
        {
            return "Teacher";
        }
    }
}
