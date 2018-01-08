using System;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace WebAPIClient
{
    public class CloudCardService    
    {
        private static HttpClient Client = new HttpClient();
        private const string Protocol = "https://";

        public string baseURL { get; set; }

        public string authToken { get; set; }

        public Boolean isCloudCardUp { get; set; }

        public CloudCardService
        (string baseURL, string authToken) {
            this.baseURL = baseURL;
            this.authToken = authToken;

            Client.BaseAddress = new Uri($"{Protocol}{baseURL}/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("X-Auth-Token", authToken);

            this.isCloudCardUp = CheckCloudCardStatus().Result;
        }

        private static async Task<Boolean> CheckCloudCardStatus()
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

        public Person FindPerson(string email) {
            return GetPersonFromCloudCard(email).Result;
        }

        public async Task<Person> GetPersonFromCloudCard(string email) {
            var serializer = new DataContractJsonSerializer(typeof(Person));

            var personTask = Client.GetStringAsync($"api/people/{email}");
            string personString = await personTask;
            Console.WriteLine(personString);

            var streamTask = Client.GetStreamAsync($"api/people/{email}");
            return serializer.ReadObject(await streamTask) as Person;
        }
    }
}