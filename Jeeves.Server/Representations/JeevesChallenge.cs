using System;
using System.Text.Json.Serialization;

namespace Jeeves.Server.Representations
{
    public class JeevesChallenge
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}