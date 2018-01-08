using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace WebAPIClient
{
    class Program
    {
        private static HttpClient Client = new HttpClient();
        private const string PROTOCOL = "https://";
        static void Main(string[] args)
        {
            Settings settings = new Settings(args);

            if (settings.isValid)
            {
                Console.WriteLine("Exiting because of missing arguments");
                return;
            }

            Client.BaseAddress = new Uri($"{PROTOCOL}{settings.baseURL}/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("X-Auth-Token", settings.authToken);
            
            if (!IsCloudCardUp().Result) 
            {
                Console.WriteLine("***\n*** Cannot Connect to CloudCard.\n***");
                return;
            }            

            Person person = GetPerson(settings.email).Result;
            Console.WriteLine($"person.Identifier == {person.Identifier}");
        }

        private static async Task<Boolean> IsCloudCardUp()
        {
            try {
                string statusString = await Client.GetStringAsync($"metadata");;
                Console.WriteLine(statusString);
                return statusString.Contains("\"name\":\"cloud-card\"");
            } catch (Exception e) {
                Console.WriteLine($"Exception thrown checking CloudCard status.\n{e}");
                return false;
            }
        }

        private static async Task<Person> GetPerson(string email) {
            var serializer = new DataContractJsonSerializer(typeof(Person));

            var personTask = Client.GetStringAsync($"api/people/{email}");
            string personString = await personTask;
            Console.WriteLine(personString);

            var streamTask = Client.GetStreamAsync($"api/people/{email}");
            return serializer.ReadObject(await streamTask) as Person;
        }
    }
}
