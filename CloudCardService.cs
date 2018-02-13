using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIClient
{
    public class CloudCardService    
    {
        private static HttpClient Client = new HttpClient();
        private const string Protocol = "https://";

        public string BaseURL { get; set; }

        public string AuthToken { get; set; }

        public Boolean IsCloudCardUp { get; set; }

        public CloudCardService
        (string BaseURL, string AuthToken) {
            this.BaseURL = BaseURL;
            this.AuthToken = AuthToken;

            Client.BaseAddress = new Uri($"{Protocol}{BaseURL}/");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

            this.IsCloudCardUp = CheckCloudCardStatus().Result;
        }

        private static async Task<Boolean> CheckCloudCardStatus()
        {
            try {
                string statusString = await Client.GetStringAsync($"metadata");;
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
            var streamTask = Client.GetStreamAsync($"api/people/{email}");
            return serializer.ReadObject(await streamTask) as Person;
        }

        public Person CreateOrUpdatePerson(string json) {
            return PostPersonToCloudCard(json).Result;
        }

        public string CreateLoginLink(string identifier) {
            return PostPersonToCloudCard($@"{{ ""identifier"": ""{identifier}"" }}").Result.Links.Login;
        }

        public async Task<Person> PostPersonToCloudCard(string json) {
            var serializer = new DataContractJsonSerializer(typeof(Person));
            
            Console.WriteLine($"Posting the following JSON to /api/people: \n{json}\n");

            StringContent content = new System.Net.Http.StringContent(json.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync($"api/people?sendInvitation=false&allowUpdate=true&getLoginLink=true", content);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            Stream stream = GenerateStreamFromString(responseString);
            Person person =  serializer.ReadObject(stream) as Person;

            return person;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}