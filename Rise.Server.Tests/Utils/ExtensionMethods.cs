using System.Net.Http.Json;

namespace Rise.Server.Tests.Utils
{
    public static class ExtensionMethods
    {
        public static Task<T?> GetAndDeserialize<T>(this HttpClient client, string requestUri)
        {
            return client.GetFromJsonAsync<T>(requestUri);
        }
    }
}
