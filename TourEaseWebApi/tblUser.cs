//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TourEaseWebApi
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUser
    {
        public int UserId { get; set; }
        public string Full_Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Contact_Number { get; set; }
        public string Location_City { get; set; }
        public string Location_Area { get; set; }
        public Nullable<int> User_Type { get; set; }
        public Nullable<int> Fake_Reported_Count { get; set; }
        public Nullable<bool> Is_Verified { get; set; }
    }
}