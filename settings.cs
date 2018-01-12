using System;
using System.Collections.Generic;

namespace WebAPIClient
{
    public class Settings
    {
        public string baseURL { get; set; }

        public string authToken { get; set; }

        public string email { get; set; }

        public string idNumber { get; set; }

        public Boolean isValid { get; }

        public string name {get; set; }

        public Settings (string[] args) {
            Dictionary<string,string> dictionary = ParseArgs(args);
            baseURL = GetSetting("baseURL", dictionary);
            authToken = GetSetting("authToken", dictionary);
            email = GetSetting("email", dictionary);
            idNumber = GetSetting("idNumber", dictionary);
            name = GetSetting("name", dictionary);
            isValid = baseURL == null || authToken == null || email == null || idNumber == null;
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

    }    
}