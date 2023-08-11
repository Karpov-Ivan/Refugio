using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Refugio.Dto
{
    [JsonObject]
    public class PointDto
    {
        [JsonPropertyName("pointX")]
        public double? pointX { get; set; }

        [JsonPropertyName("pointY")]
        public double? pointY { get; set; }

        [JsonPropertyName("color")]
        public string? color { get; set; }
    }
}