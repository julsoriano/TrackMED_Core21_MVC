using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class SystemsDescription: IEntity
    {
        [Required(ErrorMessage = "A description of the system is required")]
        [Display(Name ="Systems Description")]
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        //[Timestamp]
        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }

        /*
        [Timestamp]
        public byte[] RowVersion { get; set; }
        */
    }
}
