using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Inventory.Uwp.Helpers
{
    public static class Json
    {
        public static T ToObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static async Task<T> ToObjectAsync<T>(string value)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            });
        }

        public static string Stringify(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static async Task<string> StringifyAsync(object value)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.SerializeObject(value);
            });
        }
    }
}
