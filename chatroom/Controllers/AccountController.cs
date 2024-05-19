using chatroom.FluentValidations;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;




namespace chatroom.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Models.Membership model)
        {
            // valuidate data using fluent validation
            MembershipValidator validator = new MembershipValidator();
            var result = validator.Validate(model);

            if(!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            using (var context = new chatroomEntities())
            {
                var sender = context.UserData.FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);
                if (sender != null)
                {
                    FormsAuthentication.SetAuthCookie($"{sender.UserName}^{sender.id}", false);

                    System.Diagnostics.Debug.WriteLine($"Logged in user: {model.UserName}");


                    return RedirectToAction("GetAllUsers", "Home");
                }
                ModelState.AddModelError("", "Invalid Username and password");
            }
            return View();


        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserData model, UsersModel user)
        {
            UsersModelValidator validator = new UsersModelValidator();
            var result = validator.Validate(model);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View();
            }
            using (var context = new chatroomEntities())
            {
                string extension = Path.GetExtension(user.ProfileP.FileName); 
                string fileName = Guid.NewGuid().ToString() + extension; 
                string path = Server.MapPath("~/ProfileP/");
                string fullpath = Path.Combine(path, fileName);
                user.ProfileP.SaveAs(fullpath);

                model.ProfilePicture = fileName;

                context.UserData.Add(model);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        //public Pagination<UserData> Test()
        //{
        //    using (var context = new chatroomEntities())
        //    {
        //        var userData = context.UserData.ToList();
                
        //        var result = new Pagination<UserData>();
                
        //        result.Records = userData;
                
        //        return result;
        //    }
        //}

        //public Pagination<FriendRequests> Test2()
        //{
        //    using (var context = new chatroomEntities())
        //    {
        //        var friendRequests = context.FriendRequests.ToList();

        //        var result = new Pagination<FriendRequests>();

        //        result.Records = friendRequests;

        //        return result;
        //    }
        //}
    }

}