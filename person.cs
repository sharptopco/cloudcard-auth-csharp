using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace WebAPIClient
{
    [DataContract(Name="person")]
    public class Person
    {
        [DataMember(Name="email")]
        public string Email { get; set; }

        [DataMember(Name="identifier")]
        public string Identifier { get; set; }

        [DataMember(Name="links")]
        public Links Links { get; set; }

        [DataMember(Name="currentPhoto")]
        public Photo Photo { get; set; }

        public Boolean NeedsToSubmitPhoto() {
            if (Photo != null && (Photo.Status.Equals("PENDING") || Photo.Status.Equals("APPROVED") || Photo.Status.Equals("READY_FOR_DOWNLOAD") || Photo.Status.Equals("DOWNLOADED") || Photo.Status.Equals("DONE"))) {
                return false;
            }
            return true;
        }

        public string getLoginLink() {
            return Links.Login;
        }
    }
}