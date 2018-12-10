using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Manufacturer: IEntity
    {
        [Required(ErrorMessage = "Manufacturerer name is required")]
        [Display(Name ="Manufacturer")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }
    }
}