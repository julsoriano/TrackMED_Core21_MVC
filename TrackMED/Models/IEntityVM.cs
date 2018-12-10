using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class IEntityVM: IEntity
    {
        [Required(ErrorMessage = "A description of the equipment is required")]
        [Display(Name = "Description")]
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        //[Timestamp]
        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
    }
}
