using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Deployment: IEntity
    {
        [JsonProperty(PropertyName = "deploymentID")]
        public string DeploymentID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        //[DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:ddMMMyy hh:mm}")]
        [Display(Name = "Deployment Date")]
        [JsonProperty(PropertyName = "deploymentDate")]
        public DateTime DeploymentDate { get; set; }

        [JsonProperty(PropertyName = "systemsTabID")]
        public string SystemTabID { get; set; }

        [JsonProperty(PropertyName = "locationID")]
        public string LocationID { get; set; }

        [JsonProperty(PropertyName = "referenceNo")]
        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }

        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        [JsonProperty(PropertyName = "systemsTab")]
        public virtual SystemTab SystemTab { get; set; }

        [JsonProperty(PropertyName = "location")]
        public virtual Location Location { get; set; }
    }
}