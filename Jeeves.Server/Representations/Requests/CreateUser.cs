using System.Text.Json.Serialization;

namespace Jeeves.Server.Representations.Requests
{
    public class CreateUser
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}