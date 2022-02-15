namespace FormTask.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("formData")]
    public partial class formData
    {
        [DisplayName("#")]
        public int id { get; set; }
        [Required(ErrorMessage ="Please enter a valid Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter a valid Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please choose an image")]
        public string Image { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}
