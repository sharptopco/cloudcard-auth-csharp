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
            Dictionary<string,string> settings = ParseArgs(args);

            string baseURL = GetSetting("baseURL", settings);
            string authToken = GetSetting("authToken", settings);
            string email = GetSetting("email", settings);
            string idNumber = GetSetting("idNumber", settings);

            if (baseURL == null || authToken == null || email == null || idNumber == null)
            {
                Console.WriteLine("Exiting because of missing arguments");
                return;
            }

            Client.BaseAddress = new Uri($"{PROTOCOL}{baseURL}/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("X-Auth-Token", authToken);
            
            if (!IsCloudCardUp().Result) 
            {
                Console.WriteLine("***\n*** Cannot Connect to CloudCard.\n***");
                return;
            }            

            Console.WriteLine(GetPerson(email).Result.Identifier);
        }

        private static Dictionary<string,string> ParseArgs(string[] args)
        {
            char[] delimiterChars = { '=' };
            Dictionary<string,string> settings = new Dictionary<string,string>();

            foreach (var arg in args) {
                string[] entry = arg.Split(delimiterChars);
                settings[entry[0]] = entry[1];
            }
            return settings;
        }

        private static string GetSetting(string key, Dictionary<string, string> settings)
        {
            string value;
            if(settings.TryGetValue(key, out value)) {
                Console.WriteLine($"{key} is {value}.");
                return value;
            } else {
                Console.WriteLine($"{key} is not specified.") ;
                return null;
            }
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
