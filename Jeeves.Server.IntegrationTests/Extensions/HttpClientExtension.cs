using System.Net.Http;
using System.Text;
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
        
        public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, string body)
        {
            HttpResponseMessage response = null;
            
            try
            {
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                response = await client.PostAsync(requestUri, content);
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