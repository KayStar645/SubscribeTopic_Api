using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Services
{
    public static class ResultConverter
    {
        public static string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] charArray = input.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                if (char.IsLetter(charArray[i]) && i == 0)
                {
                    charArray[i] = char.ToLowerInvariant(charArray[i]);
                }
                else if (charArray[i] == '_' && i < charArray.Length - 1 && char.IsLetter(charArray[i + 1]))
                {
                    charArray[i + 1] = char.ToLowerInvariant(charArray[i + 1]);
                    i++;
                }
            }

            return new string(charArray);
        }
    }
}
