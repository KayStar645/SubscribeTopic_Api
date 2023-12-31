﻿
namespace Core.Application.Transform
{
    public class CommonTranform
    {
        // Giới tính
        public static string male = "Nam";
        public static string female = "Nữ";
        public static string other = "Khác";

        public static string[] GetGender()
        {
            return new string[]
            {
                male,
                female,
                other
            };
        }

        // Học hàm
        public static string bachelor = "Cử nhân";
        public static string engineer = "Kỹ sư";
        public static string postgraduate = "Cao học";
        public static string master = "Thạc sĩ";
        public static string doctorate = "Tiến sĩ";
        public static string[] GetListAcademicTitle()
        {
            return new string[]
            {
                bachelor,
                engineer,
                postgraduate,
                master,
                doctorate
            };
        }

        public static string associateProfessor = "Phó giáo sư";
        public static string professor = "Giáo sư";
        public static string[] GetListDegree()
        {
            return new string[]
            {
                associateProfessor,
                professor
            };
        }

        public static string semester1 = "Học kỳ 1";
        public static string semester2 = "Học kỳ 2";
        public static string semester3 = "Học kỳ 3";
        public static string[] GetListSemester()
        {
            return new string[]
            {
                semester1,
                semester2,
                semester3
            };
        }

        public static string[] GetListSchoolYear()
        {
            int currentYear = DateTime.Now.Year;
            string[] result = new string[6];
            int index = 0;
            for(int year = currentYear - 3; year < currentYear + 3; year++, index++)
            {
                result[index] = (year) + "-" + (year + 1);
            }

            return result;
        }
    }
}
