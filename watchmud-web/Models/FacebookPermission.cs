using System;
using Newtonsoft.Json;

namespace Watchmud.Web.Models
{
    
    //{"data":[{"permission":"public_profile","status":"granted"},{"permission":"email","status":"declined"}]}
    public class FacebookPermission
    {
        public string Permission { get; set; }
        public string Status { get; set; }

        [JsonIgnore] public bool Granted => "granted".Equals(Status, StringComparison.InvariantCultureIgnoreCase);
    }
}