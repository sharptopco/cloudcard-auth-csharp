using System;
using System.Collections.Generic;

namespace WebAPIClient
{
    public class Settings
    {
        public string BaseURL { get; set; }

        public string AuthToken { get; set; }

        public string Email { get; set; }

        public string IdNumber { get; set; }

        public Boolean IsValid { get; }

        public string Name {get; set; }

        public Settings (string[] args) {
            Dictionary<string,string> Dictionary = ParseArgs(args);
            BaseURL = GetSetting("baseURL", Dictionary);
            AuthToken = GetSetting("authToken", Dictionary);
            Email = GetSetting("email", Dictionary);
            IdNumber = GetSetting("idNumber", Dictionary);
            Name = GetSetting("name", Dictionary);
            IsValid = BaseURL == null || AuthToken == null || Email == null || IdNumber == null;
        }

        private static Dictionary<string,string> ParseArgs(string[] args)
        {
            char[] DelimiterChars = { '=' };
            Dictionary<string,string> Settings = new Dictionary<string,string>();

            foreach (var Arg in args) {
                string[] Entry = Arg.Split(DelimiterChars);
                Settings[Entry[0]] = Entry[1];
            }
            return Settings;
        }

        private static string GetSetting(string key, Dictionary<string, string> settings)
        {
            string Value;
            if(settings.TryGetValue(key, out Value)) {
                Console.WriteLine($"{key} is {Value}.");
                return Value;
            } else {
                Console.WriteLine($"{key} is not specified.") ;
                return null;
            }
        }

    }    
}