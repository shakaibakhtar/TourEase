using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourEaseWebApi.Models;

namespace TourEaseWebApi.Controllers
{
    public class ApiController : Controller
    {
        TourEaseDb db = new TourEaseDb();
        // GET: Api
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Register(clsUser user)
        {
            bool status = false;
            string message = "";
            int returnId = 0;

            if (user != null)
            {
                try
                {
                    tblUser tmp = new tblUser()
                    {
                        Full_Name = user.Full_Name,
                        Email = user.Email,
                        Password = user.Password,
                        Contact_Number = user.Contact_Number,
                        Location_City = user.Location_City,
                        Location_Area = user.Location_Area,
                        Fake_Reported_Count = user.Fake_Reported_Count,
                        User_Type = user.User_Type
                    };

                    db.tblUsers.Add(tmp);
                    db.SaveChanges();


                    status = true;
                    message = "You have registered successfully";
                    returnId = db.tblUsers.OrderByDescending(x=>x.UserId).Where(x=>x.User_Type == user.User_Type).FirstOrDefault().UserId;
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    returnId = 0;
                }
            }
            else
            {
                status = false;
                message = "Inavlid user sent to api";
                returnId = 0;
            }
            return Json(new { status = status, message = message, returnId = returnId }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Login(clsUser user)
        {
            bool status = false;
            string message = "";

            if (user != null)
            {
                try
                {
                    var tmp = db.tblUsers.Where(x => x.Email == user.Email && x.Password == user.Password).Select(x => new clsUser()
                    {
                        Email = x.Email,
                        Full_Name = x.Full_Name,
                        Contact_Number = x.Contact_Number,
                        Location_City = x.Location_City,
                        Location_Area = x.Location_Area,
                        UserId = x.UserId,
                        Fake_Reported_Count = x.Fake_Reported_Count,
                        User_Type = x.User_Type,
                        UserTypeName = db.tblUserTypes.Where(y=>y.UserTypeId == x.User_Type).Select(y=>y.UserTypeName).FirstOrDefault()
                    }).FirstOrDefault();
                    if (tmp != null)
                    {
                        status = true;
                        message = "You have login successfully.";
                        user = tmp;
                    }
                    else
                    {
                        status = false;
                        message = "Invalid username or password.";
                        user = null;
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    user = null;
                }
            }
            else
            {
                status = false;
                message = "Invalid user sent to api";
                user = null;
            }

            return Json(new { status = status, message = message, user = user }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult GetGuestsOrHosts(int id, int userTypeId) // userType = 1 for Guests, and 2 for Hosts
        {
            bool status = false;
            string message = "";
            List<clsUser> GuestsHostsList = new List<clsUser>();

            if (id != 0 && userTypeId != 0)
            {
                try
                {
                    GuestsHostsList = db.tblUsers.Where(x => x.UserId != id && x.User_Type != userTypeId).Select(x => new clsUser()
                    {
                        UserId = x.UserId,
                        Email = x.Email,
                        Contact_Number = x.Contact_Number,
                        Location_City = x.Location_City,
                        Location_Area = x.Location_Area,
                        Full_Name = x.Full_Name,
                        Fake_Reported_Count = x.Fake_Reported_Count
                    }).ToList<clsUser>();

                    status = true;
                    message = "";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                    GuestsHostsList = null;
                }
            }
            else
            {
                status = false;
                message = "Invalid user or user type sent to api";
                GuestsHostsList = null;
            }
            return Json(new { status = status, message = message, guestsList = GuestsHostsList }, JsonRequestBehavior.AllowGet);
        }
    }
}