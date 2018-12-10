using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Location: IEntity
    {
        [Required(ErrorMessage = "A location name is required")]
        [Display(Name ="Location")]
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
    }
}