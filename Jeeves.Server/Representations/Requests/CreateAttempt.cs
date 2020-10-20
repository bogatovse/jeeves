using System.Text.Json.Serialization;

namespace Jeeves.Server.Representations.Requests
{
    public class CreateAttempt
    {
        [JsonPropertyName("solution")]
        public byte[] Solution { get; set; }
    }
}