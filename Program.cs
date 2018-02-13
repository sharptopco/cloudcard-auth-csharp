using System;

namespace WebAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings Settings = new Settings(args);

            if (Settings.IsValid)
            {
                Console.WriteLine("Exiting because of missing arguments");
                return;
            }

            CloudCardService Service = new CloudCardService(Settings.BaseURL, Settings.AuthToken);
            
            if (!Service.IsCloudCardUp) 
            {
                Console.WriteLine("***\n*** Cannot Connect to CloudCard.\n***");
                return;
            }            

            //Create or update a person
            String json = 
$@"{{
    ""email"":""{Settings.Email}"",
    ""identifier"":""{Settings.IdNumber}""
}}";
            Person Person = Service.CreateOrUpdatePerson(Settings.IdNumber, json);
            Console.WriteLine($"Person '{Person.Identifier}' created or updated.");

            //get login link without updating
            string link = Service.CreateLoginLink(Settings.IdNumber);
            Console.WriteLine($"The login link for '{Settings.IdNumber}' follows:\n{link}");

            //Find a person
            Person = Service.FindPerson(Settings.Email);
            string Status = (Person.Photo == null) ? "null" : Person.Photo.Status;
            Console.WriteLine($"Found person '{Person.Identifier}' with a photo status of '{Status}'");

            string Needs = Person.NeedsToUploadPhoto() ? "needs" : "does not need";
            Console.WriteLine($"Person '{Person.Identifier}' {Needs} to upload a photo.");
        }

    }
}
