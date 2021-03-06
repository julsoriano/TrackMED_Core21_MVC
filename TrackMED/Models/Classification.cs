﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Classification: IEntity
    {
        [Required(ErrorMessage = "A classification name is required")]
        [Display(Name ="Classification")]
        public string Desc { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedAtUtc { get; set; }

    }
}