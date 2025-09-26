using System.Text.Json.Serialization;

namespace JsonTypedSelector.Model
{
    public record Person
    {
        [JsonPropertyName("card_number")]
        public int CardNumber { get; init; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; init; }

        [JsonPropertyName("addresses")]
        public List<Address> Addresses { get; init; }
    }
}
