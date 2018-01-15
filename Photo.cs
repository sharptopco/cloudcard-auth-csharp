using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace WebAPIClient
{
    [DataContract(Name="currentPhoto")]
    public class Photo
    {
        [DataMember(Name="status")]
        public string Status { get; set; }
    }
}