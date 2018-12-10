using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public abstract class Equipment: IEntity
    {
        [StringLength(20)]
        [Required(ErrorMessage = "An equipment code is required")]
        [Display(Name="IMTE")]
        [JsonProperty(PropertyName = "imte")]
        public string imte { get; set; }

        [Display(Name ="Serial #")]
        [JsonProperty(PropertyName = "serialnumber")]
        public string serialnumber { get; set; }

        [StringLength(50)]
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        /*
        [Timestamp]
        public byte[] RowVersion { get; set; }
        */

        [Display(Name = "Created On")]
        [JsonProperty(PropertyName = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
        
        [DisplayName("Owner")]
        [JsonProperty(PropertyName = "ownerID")]
        public string OwnerID { get; set; }

        [DisplayName("Status")]
        [JsonProperty(PropertyName = "statusID")]
        public string StatusID { get; set; }

        [DisplayName("Activity")]
        [JsonProperty(PropertyName = "activityTypeID")]
        public string ActivityTypeID { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public virtual Owner Owner { get; set; }

        [JsonProperty(PropertyName = "status")]
        public virtual Status Status { get; set; }

        [JsonProperty(PropertyName = "activityType")]
        public virtual ActivityType ActivityType { get; set; }
        //public virtual ICollection<Event> Events { get; set; }

    }
}