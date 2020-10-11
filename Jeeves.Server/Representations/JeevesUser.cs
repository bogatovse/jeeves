using System;
using System.Text.Json.Serialization;

namespace Jeeves.Server.Representations
{
    public class JeevesUser
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}