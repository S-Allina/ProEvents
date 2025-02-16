using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
namespace ProEvent.Services.Core.Interfaces
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventStatus
    {
        Relevant,
        NoPlaces,
        Passed
    }
}
