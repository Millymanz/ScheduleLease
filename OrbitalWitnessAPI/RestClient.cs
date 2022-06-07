using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using OrbitalWitnessAPI.Interfaces;

namespace OrbitalWitnessAPI
{
    public class RestClient : IRestClient
    {
        private IConfigurationRoot _configuration;

        public RestClient(IConfiguration configuration)
        {
            _configuration = (IConfigurationRoot)configuration;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
           
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetFromRestService(string url)
        {
            var auth = _configuration.GetSection("APIUserName").Value + ":" +_configuration.GetSection("APIPassword").Value;            

            var pwd = Base64Encode(auth);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + pwd);

            var stringTask = client.GetStringAsync(url);

            var msg = await stringTask;

            return msg;
        }
    }
}
