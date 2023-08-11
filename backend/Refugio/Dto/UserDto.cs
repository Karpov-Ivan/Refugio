using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Refugio.Dto
{
    [JsonObject]
    public class UserDto
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("activity")]
        public string? Activity { get; set; }

        [JsonPropertyName("university")]
        public string? University { get; set; }

        [JsonPropertyName("facultyName")]
        public string? FacultyName { get; set; }

        [JsonPropertyName("vkIdString")]
        public string? VkIdString { get; set; }
    }
}