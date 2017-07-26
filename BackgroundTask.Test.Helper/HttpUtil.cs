using System.Net.Http;
using System.Threading.Tasks;

namespace BackgroundTask.Test.Helper
{
    public class HttpUtil
    {
        private static HttpClient _client;

        static HttpUtil()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        public static async Task<string> GetContentAsync(string url)
        {
            return await _client.GetStringAsync(url);
        }

        public static async Task<byte[]> DownloadFile(string url)
        {
            return await _client.GetByteArrayAsync(url);
        }
    }
}
