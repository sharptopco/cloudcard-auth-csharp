using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace WebAPIClient
{
    [DataContract(Name="links")]
    public class Links
    {
        [DataMember(Name="login")]
        public string Login { get; set; }
    }
}