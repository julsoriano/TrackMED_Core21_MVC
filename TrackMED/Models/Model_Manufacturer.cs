using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Model_Manufacturer: IEntity
    {
        [Required(ErrorMessage = "A model/manufacturer description is required")]
        [Display(Name ="Model/Manufacturer")]
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
    }
}