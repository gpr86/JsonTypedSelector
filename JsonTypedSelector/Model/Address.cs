using System.Text.Json.Serialization;

namespace JsonTypedSelector.Model
{
    public record Address
    {
        [JsonPropertyName("city")]
        public string City { get; init; }

        [JsonPropertyName("country_code")]
        public CountryCode CountryCode { get; init; }
    }
}
