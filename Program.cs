using System;

namespace WebAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings(args);

            if (settings.isValid)
            {
                Console.WriteLine("Exiting because of missing arguments");
                return;
            }

            CloudCardService service = new CloudCardService(settings.baseURL, settings.authToken);
            
            if (!service.isCloudCardUp) 
            {
                Console.WriteLine("***\n*** Cannot Connect to CloudCard.\n***");
                return;
            }            

            //Create or update a person
            String json = 
$@"{{
    ""email"":""{settings.email}"",
    ""identifier"":""{settings.idNumber}"",
    ""Name"":""{settings.name}""
}}";
            PersonResponse response = service.CreateOrUpdatePerson(settings.idNumber, json);
            Console.WriteLine($"Person '{response.person.Identifier}' created or updated.");

            //get login link without updating
            string link = service.GetLink(settings.idNumber);
            Console.WriteLine($"login link for '{settings.idNumber}':\n{link}");

            //Find a person
            Person person = service.FindPerson(settings.email);
            Console.WriteLine($"Found person '{person.Identifier}' with a photo status of '{person.Photo.Status}'");

            string needs = person.NeedsToUploadPhoto() ? "needs" : "does not need";
            Console.WriteLine($"Person '{person.Identifier}' {needs} to upload a photo.");
        }

    }
}
