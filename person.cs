using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace WebAPIClient
{
    [DataContract(Name="person")]
    public class Person
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="email")]
        public string Email { get; set; }

        [DataMember(Name="identifier")]
        public string Identifier { get; set; }
    }
}