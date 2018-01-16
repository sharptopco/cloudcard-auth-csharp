using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace WebAPIClient
{
    [DataContract(Name="person_response")]
    public class PersonResponse
    {
        [DataMember(Name="access_link")]
        public string Link { get; set; }

        [DataMember(Name="user")]
        public Person Person { get; set; }
    }
}