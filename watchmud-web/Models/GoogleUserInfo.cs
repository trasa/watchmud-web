using Newtonsoft.Json;

namespace Watchmud.Web.Models
{
    /*
    * res: {
"id": "102171105725265301028",
"email": "trasa@meancat.com",
"verified_email": true,
"name": "Tony Rasa",
"given_name": "Tony",
"family_name": "Rasa",
"link": "https://plus.google.com/102171105725265301028",
"picture": "https://lh3.googleusercontent.com/a-/AOh14GhjQ9PK4zNgPtf8jPFLdPKIXKksetebiqAYVnnTzg=s96-c",
"gender": "male",
"locale": "en",
"hd": "meancat.com"
}
    */
    public class GoogleUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set;}
        
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
        
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Link { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
        public string Locale { get; set; }
        public string HD { get; set; }
    }

}