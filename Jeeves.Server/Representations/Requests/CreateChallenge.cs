using System.Text.Json.Serialization;

namespace Jeeves.Server.Representations.Requests
{
    public class CreateChallenge
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}