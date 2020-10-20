using System;
using System.Text.Json.Serialization;

namespace Jeeves.Server.Representations
{
    public class JeevesAttempt
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("status")]
        public JeevesAttemptStatus Status { get; set; }
        
        [JsonPropertyName("solution")]
        public byte[] Solution { get; set; }
    }
}