using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Core.Application.Services
{
    public static class CustomJsonSerializer
    {
        public static string Serialize(object value, JsonSerializerOptions options = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                options = new JsonSerializerOptions();
            }

            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

            return JsonSerializer.Serialize(value, options);
        }
    }

}
