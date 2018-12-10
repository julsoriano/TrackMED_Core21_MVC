using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class Component: Equipment
    {
        [Display(Name = "Asset #")]
        [JsonProperty(PropertyName = "assetnumber")]
        public string assetnumber { get; set; }

        [Required(ErrorMessage = "An equipment description is required")]
        [DisplayName("Component Id")]
        [JsonProperty(PropertyName = "descriptionID")]
        public string DescriptionID { get; set; }

        [JsonProperty(PropertyName = "model_ManufacturerID")]
        public string Model_ManufacturerID { get; set; }

        [JsonProperty(PropertyName = "providerOfServiceID")]
        public string ProviderOfServiceID { get; set; }

        [JsonProperty(PropertyName = "description")]
        public virtual Description Description { get; set; }

        [JsonProperty(PropertyName = "model_Manufacturer")]
        public virtual Model_Manufacturer Model_Manufacturer { get; set; }

        [JsonProperty(PropertyName = "providerOfService")]
        public virtual ProviderOfService ProviderOfService { get; set; }
        /*
         * You can use the DisplayFormat attribute by itself, but it's generally a good idea to use the DataType attribute also. 
         * The DataType attribute conveys the semantics of the data as opposed to how to render it on a screen, and provides the following benefits that you don't get with DisplayFormat:
           1. The browser can enable HTML5 features (for example to show a calendar control, the locale-appropriate currency symbol, email links, some client-side input validation, etc.).
           2. By default, the browser will render data using the correct format based on your locale.
           3. The DataType attribute can enable MVC to choose the right field template to render the data (the DisplayFormat uses the string template). 
         * For more information, see Brad Wilson's ASP.NET MVC 2 Templates. (Though written for MVC 2, this article still applies to the current version of ASP.NET MVC.)
         * 
         * Also:
         * If you use the DataType attribute with a date field, you have to specify the DisplayFormat attribute also in order to ensure that the field renders correctly in Chrome browsers. 
         * For more information, see this StackOverflow thread. (http://stackoverflow.com/questions/12633471/mvc4-datatype-date-editorfor-wont-display-date-value-in-chrome-fine-in-interne)
         */
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "Calibration Due Date")]
        [JsonProperty(PropertyName = "calibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        [Display(Name = "Calibration Interval (Days)")]
        [JsonProperty(PropertyName = "calibrationInterval")]
        public int? CalibrationInterval { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "Maintenance Due Date")]
        [JsonProperty(PropertyName = "maintenanceDate")]
        public DateTime? MaintenanceDate { get; set; }

        [Display(Name = "Maintenance Interval (Days)")]
        [JsonProperty(PropertyName = "maintenanceInterval")]
        public int? MaintenanceInterval { get; set; }

        [Display(Name = "Module")]
        [JsonProperty(PropertyName = "imteModule")]
        public string imteModule { get; set; }

        //[Display(Name = "Desc+Tag")]
        /*
        public string DescTag
        {
            get
            {
                if (Description.Desc != null)
                    return Description.Desc + (!String.IsNullOrEmpty(Description.Tag) ? " Tag: " + Description.Tag : null);
                else return null;
                //return !String.IsNullOrEmpty(Description.Desc) ? Description.Desc + (!String.IsNullOrEmpty(Description.Tag) ? " Tag: " + Description.Tag: null) : null;
            }
        }
        */
        /*
        [Display(Name = "IMTE+")]
        public string IMTECalDate
        {
            get
            {
                return imte + " " + (CalibrationDateTime != null? Convert.ToString(CalibrationDateTime) : null);
            }
        }
        */
    }

    public enum orientModule : byte
    {
        [Display(Name = "Left")]
        Left,
        [Display(Name = "Right")]
        Right
    }

    /*  T E S T   D A T A
        var sernum = ["ser01", "ser02", "ser03"];
        var imte = ["imte01", "imte02", "imte03"];
        for( i=0; i < 3; i++) {
            db.Component.insert([
                  { imte: imte[i]+"1L", SerialNumber: sernum[i]+"1L", CreatedAtUtc: ISODate() },
                  { imte: imte[i]+"2L", SerialNumber: sernum[i]+"2L", CreatedAtUtc: ISODate() },
                  { imte: imte[i]+"3L", SerialNumber: sernum[i]+"3L", CreatedAtUtc: ISODate() },

          
                  { imte: imte[i]+"1R", SerialNumber: sernum[i]+"1R", CreatedAtUtc: ISODate() },
                  { imte: imte[i]+"2R", SerialNumber: sernum[i]+"2R", CreatedAtUtc: ISODate() },
                  { imte: imte[i]+"3R", SerialNumber: sernum[i]+"3R", CreatedAtUtc: ISODate() }
           ])
         }

    
        MongoDB Enterprise > use trackmed
        switched to db trackmed
        MongoDB Enterprise > var sernum = ["ser01", "ser02", "ser03"];
        MongoDB Enterprise > var imte = ["imte01", "imte02", "imte03"];
        MongoDB Enterprise > for( i=0; i < 3; i++) {
        ...     db.Component.insert([
        ...           { imte: imte[i]+"1L", SerialNumber: sernum[i]+"1L", CreatedAtUtc: ISODate() },
        ...           { imte: imte[i]+"2L", SerialNumber: sernum[i]+"2L", CreatedAtUtc: ISODate() },
        ...           { imte: imte[i]+"3L", SerialNumber: sernum[i]+"3L", CreatedAtUtc: ISODate() },
        ...
        ...
        ...           { imte: imte[i]+"1R", SerialNumber: sernum[i]+"1R", CreatedAtUtc: ISODate() },
        ...           { imte: imte[i]+"2R", SerialNumber: sernum[i]+"2R", CreatedAtUtc: ISODate() },
        ...           { imte: imte[i]+"3R", SerialNumber: sernum[i]+"3R", CreatedAtUtc: ISODate() }
        ...    ])
        ...  }
        BulkWriteResult({
                "writeErrors" : [ ],
                "writeConcernErrors" : [ ],
                "nInserted" : 6,
                "nUpserted" : 0,
                "nMatched" : 0,
                "nModified" : 0,
                "nRemoved" : 0,
                "upserted" : [ ]
        })
        MongoDB Enterprise > db.Component.find().pretty()
        {
                "_id" : ObjectId("5771f3de03508a29611ed042"),
                "imte" : "imte011L",
                "SerialNumber" : "ser011L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.230Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed043"),
                "imte" : "imte012L",
                "SerialNumber" : "ser012L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.230Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed044"),
                "imte" : "imte013L",
                "SerialNumber" : "ser013L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.230Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed045"),
                "imte" : "imte011R",
                "SerialNumber" : "ser011R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.230Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed046"),
                "imte" : "imte012R",
                "SerialNumber" : "ser012R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.230Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed047"),
                "imte" : "imte013R",
                "SerialNumber" : "ser013R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.230Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed048"),
                "imte" : "imte021L",
                "SerialNumber" : "ser021L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.826Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed049"),
                "imte" : "imte022L",
                "SerialNumber" : "ser022L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.827Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed04a"),
                "imte" : "imte023L",
                "SerialNumber" : "ser023L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.827Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed04b"),
                "imte" : "imte021R",
                "SerialNumber" : "ser021R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.827Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed04c"),
                "imte" : "imte022R",
                "SerialNumber" : "ser022R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.827Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed04d"),
                "imte" : "imte023R",
                "SerialNumber" : "ser023R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.827Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed04e"),
                "imte" : "imte031L",
                "SerialNumber" : "ser031L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.829Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed04f"),
                "imte" : "imte032L",
                "SerialNumber" : "ser032L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.829Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed050"),
                "imte" : "imte033L",
                "SerialNumber" : "ser033L",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.829Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed051"),
                "imte" : "imte031R",
                "SerialNumber" : "ser031R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.829Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed052"),
                "imte" : "imte032R",
                "SerialNumber" : "ser032R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.829Z")
        }
        {
                "_id" : ObjectId("5771f3de03508a29611ed053"),
                "imte" : "imte033R",
                "SerialNumber" : "ser033R",
                "CreatedAtUtc" : ISODate("2016-06-28T03:49:50.829Z")
        }
        MongoDB Enterprise >
     
     */
}