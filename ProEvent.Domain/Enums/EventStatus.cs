using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
namespace ProEvent.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventStatus
    {
        Relevant,
        NoPlaces,
        Passed
    }
}
