using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Transform
{
    public static class IdentityTranform
    {
        public static string UserAlreadyExists(string userName)
        {
            return $"Tên người dùng ${userName} đã tồn tại!";
        }

        public static string UserNotExists(string userName) 
        {
            return $"Không tìm thấy người dùng ${userName}!";
        }

        public static string InvalidCredentials(string userName) 
        {
            return $"Thông tin xác thực của người dùng ${userName} không hợp lệ!";
        }
    }
}
