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

            Console.WriteLine($"----------------------------------------------------------------------------------------------------");
            Console.WriteLine($"USE CASE 1: Create or update a user and then send them to CloudCard if they need to submit a photo");
            Person Person = Service.CreateOrUpdatePerson(json);
            Console.WriteLine($"Person '{Person.Identifier}' created or updated.");
            if (Person.NeedsToSubmitPhoto()){
                Console.WriteLine($"Person '{Person.Identifier}' needs to upload a photo.");
                Console.WriteLine($"Show the user this link: \n{Person.getLoginLink()}\n * Note: Use case 2 will deprovision this link.");
            } else {
                Console.WriteLine($"Person '{Person.Identifier}' has already uploaded their photo.");
            }

            Console.WriteLine($"----------------------------------------------------------------------------------------------------");
            Console.WriteLine($"USE CASE 2: get login link without updating anything other than the login link");
            string link = Service.CreateLoginLink(Settings.IdNumber);
            Console.WriteLine($"The login link for '{Settings.IdNumber}' follows:\n{link}\n * Note: this link deprovisioned all previous links.");

            Console.WriteLine($"----------------------------------------------------------------------------------------------------");
            Console.WriteLine($"USE CASE 3: Look up a person by their email address");
            Person = Service.FindPerson(Settings.Email);
            string Status = (Person.Photo == null) ? "null" : Person.Photo.Status;
            Console.WriteLine($"Found person '{Person.Identifier}' with a photo status of '{Status}'");
        }

    }
}
