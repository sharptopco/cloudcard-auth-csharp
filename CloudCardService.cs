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

        public PersonResponse CreateOrUpdatePerson(string identifier, string json) {
            return PutPersonToCloudCard(identifier, json).Result;
        }

        public string GetLink(string identifier) {
            return PutPersonToCloudCard(identifier, "").Result.link;
        }

        public async Task<PersonResponse> PutPersonToCloudCard(string identifier, string json) {
            var serializer = new DataContractJsonSerializer(typeof(PersonResponse));
            
            if(json != null && json != "") {
                Console.WriteLine($"Updating user '{identifier}' with the following values: \n{json}\n");
            } else {
                Console.WriteLine($"Getting link for user '{identifier}'");
            }

            StringContent content = new System.Net.Http.StringContent(json.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PutAsync($"api/people/{identifier}?allowCreate=true&getLoginLink=true", content);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            Stream stream = GenerateStreamFromString(responseString);
            PersonResponse personResponse =  serializer.ReadObject(stream) as PersonResponse;

            return personResponse;
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