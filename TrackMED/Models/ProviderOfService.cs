using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class ProviderOfService: IEntity
    {
        [Required(ErrorMessage = "Service Provider name is required")]
        [Display(Name ="Service Provider")]
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
    }
}