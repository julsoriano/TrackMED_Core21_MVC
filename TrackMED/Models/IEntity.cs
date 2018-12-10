using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TrackMED.Models
{
    [DataContract] // make this serializable by DataContractSerializer 
    public class IEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }      // must be declared public because default is private altho class is public
    }
}
