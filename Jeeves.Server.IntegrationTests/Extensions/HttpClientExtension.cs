using System.Net.Http;
using System.Threading.Tasks;

namespace Jeeves.Server.IntegrationTests.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<HttpResponseMessage> GetSuccessAsync(this HttpClient client, string requestUri)
        {
            HttpResponseMessage response = null;
            
            try
            {
                response = await client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var tmp = response;
                response = null;
                return tmp;
            }
            finally
            {
                response?.Dispose();
            }
        }
    }
}