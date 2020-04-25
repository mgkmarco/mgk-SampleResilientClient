using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SampleClient
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var fooHttpClient = ClientFactory.FooHttpClient;
            var httpResponseMessage = await fooHttpClient.GetAsync("api/values/5");
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(response);
            
            httpResponseMessage = await fooHttpClient.GetAsync("api/values");
            response = await httpResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(response);
            
            await fooHttpClient.PostAsync("api/values", new StringContent("Hello"));
            
            var barHttpClient = ClientFactory.BarHttpClient;
            var httpResponseMessage2 = await barHttpClient.GetAsync("api/values/5");
            var response2 = await httpResponseMessage2.Content.ReadAsStringAsync();
            
            Console.WriteLine(response2);
        }
    }
}