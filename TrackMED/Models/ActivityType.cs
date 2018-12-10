using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class ActivityType: IEntity
    {
        [Required(ErrorMessage = "Activity Type is required")]
        [Display(Name ="Activity")]
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
    }
}