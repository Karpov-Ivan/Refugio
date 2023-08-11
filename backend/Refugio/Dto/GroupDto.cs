using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Refugio.Dto
{
    [JsonObject]
    public class GroupDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("activity")]
        public string? Activity { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("member-count")]
        public string? MembersCount { get; set; }

        [JsonPropertyName("place")]
        public string? Place { get; set; }

        [JsonPropertyName("is-closed")]
        public bool? IsClosed { get; set; }
    }
}