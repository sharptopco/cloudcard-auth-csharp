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

            Person person = service.FindPerson(settings.email);
            Console.WriteLine($"person.Identifier == {person.Identifier}");
        }

    }
}
