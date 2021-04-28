using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourEaseWebApi.Models
{
    public class clsUser
    {
        public int UserId { get; set; }
        public string Full_Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Contact_Number { get; set; }
        public string Location_City { get; set; }
        public string Location_Area { get; set; }
        public int? User_Type { get; set; }
        public string UserTypeName { get; set; }
        public int? Fake_Reported_Count { get; set; }
        public bool? Is_Verified { get; set; }
        public double? RatingValue { get; set; }
    }
}