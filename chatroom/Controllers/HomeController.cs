using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using chatroom.DbOperations;
using chatroom.FluentValidations;
using chatroom.Models;

namespace chatroom.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public UserRepository UserRepository = null;

        public HomeController()
        {
            UserRepository = new UserRepository();
        }
        public ActionResult GetAllUsers()
        {
            var result = UserRepository.GetAllUsers();
            return View(result);
        }
        public ActionResult Profile_Details(int id)
        {
            var result = UserRepository.Profile_Details(id);
            return View(result);
        }
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendFriendRequest(int receiverId)
        {
            int senderId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');

            if (parts.Length == 2 && int.TryParse(parts[1], out senderId))
            {
                var senderUser = UserRepository.GetUserById(senderId);

                var receiverUser = UserRepository.GetUserById(receiverId);

                if (senderUser == null)
                {
                    return HttpNotFound("Sender user not found");
                }

                if (receiverUser == null)
                {
                    return HttpNotFound("Receiver user not found");
                }
               
                if (senderId == receiverId)
                {
                    TempData["Message"] = "You cannot send a friend request to yourself.";
                    return RedirectToAction("GetAllUsers", "Home");
                }
                if (UserRepository.AreFriends(senderId, receiverId))
                {
                    TempData["Message"] = "You are already friends with this user.";
                    return RedirectToAction("GetAllUsers", "Home");
                }
                if (UserRepository.HasPendingFriendRequest(senderId, receiverId))
                {
                    TempData["Message"] = "A friend request has already been sent to this user.";
                    return RedirectToAction("GetAllUsers", "Home");
                }


                UserRepository.SendFriendRequest(senderUser, receiverUser);

                ViewBag.Message = "Friend request sent successfully";
            }
            else
            {
                ViewBag.Message = "Invalid user format.";
                return View();
            }

            return RedirectToAction("GetAllUsers", "Home");
        }
        
        public ActionResult PendingFriendRequests()
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');

            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                var pendingRequests = UserRepository.GetPendingFriendRequests(userId);

                return View(pendingRequests);
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid user format.";
                return View();
            }
        }


        [HttpPost]
        public ActionResult AcceptRequest(int senderId)
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                bool accepted = UserRepository.AcceptFriendRequest(senderId, userId);
                if (accepted)
                {
                    TempData["Message"] = "Friend request accepted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to accept friend request.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid user format.";
            }

            return RedirectToAction("PendingFriendRequests","Home");
        }
        [HttpPost]
        public ActionResult RejectRequest(int senderId)
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if(parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                bool rejected = UserRepository.RejectFriendRequest(senderId, userId);
                if (rejected)
                {
                    TempData["Message"] = "Friend Request Rejected";

                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reject friend request.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid User Format";
            }
            return RedirectToAction("PendingFriendRequest","Home");
        }
        public ActionResult Friends_List()
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                var result = UserRepository.FriendsList(userId);
                return View(result);
            }
            return View();
        }
        public ActionResult My_Profile()
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                var result = UserRepository.Profile_Details(userId);
                return View(result);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Unfriend(int SenderId)
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');

            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                UserRepository userRepository = new UserRepository();
                var unfriended = userRepository.Unfriend(userId, SenderId);

                if (unfriended)
                {
                    return RedirectToAction("Friends_List", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to unfriend the user.";
                    return RedirectToAction("Friends_List", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid user format.";
                return RedirectToAction("Friends_List", "Home");
            }
        }

        public ActionResult EditProfile()
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
              
                var profiled = UserRepository.GetUserById(userId);
                return View(profiled);
            }
            return View();           
        }

        [HttpPost]
        public ActionResult EditProfile(UserData userData, UsersModel user)
        {
            UsersModeValidator validator = new UsersModeValidator();
            var result = validator.Validate(userData);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View();
            }

            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                using (var context = new chatroomEntities())
                {
                    var existingUserData = context.UserData.Find(userId);

                    if (existingUserData != null)
                    {
                       
                        existingUserData.FirstName = userData.FirstName;
                        existingUserData.LastName = userData.LastName;
                        existingUserData.Email = userData.Email;
                        if (user.ProfileP != null )
                        {
                            string extension = Path.GetExtension(user.ProfileP.FileName);
                            string fileName = Guid.NewGuid().ToString() + extension;
                            string path = Server.MapPath("~/ProfileP/");
                            string fullPath = Path.Combine(path, fileName);
                            user.ProfileP.SaveAs(fullPath);
                            existingUserData.ProfilePicture = fileName;
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User data not found.";
                        return RedirectToAction("My_Profile");
                    }
                }
            }           
            return RedirectToAction("My_Profile");
        }   
        public ActionResult Chats()
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                ViewBag.UserId = userId;
                var chats = UserRepository.Chats(userId);
                var filteredChats =chats.Where(c => c.SenderId != userId && c.RecieverId != userId).ToList();
               
                return View(filteredChats);
            }
           
            else
            {
                ViewBag.ErrorMessage = "Invalid user format.";
                return View();
            }
        }
        [HttpPost]
        public ActionResult SendMessage(int RecieverId, string Content)
        {
            int userId = 0;
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                var senderId = userId;

                if (senderId == RecieverId)
                {
                    TempData["ErrorMessage"] = "You cannot send a message to yourself.";
                    return RedirectToAction("Chats", "Home");
                }
               

                var message = new Messages
                {
                    SenderId = senderId,
                    RecieverId = RecieverId,
                    Content = Content,
                    TimeStamp = DateTime.Now,
                    
                };

                bool messageSent = UserRepository.SendMessage(message);

                if (messageSent)
                {
                    TempData["Message"] = "Message sent successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send message.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid user format.";
            }

            return RedirectToAction("Chats", "Home");
        }
       
        public ActionResult ChatDetails(int chatId)
        {
            int userId = 0; 
            string person = User.Identity.Name;
            string[] parts = person.Split('^');
            if (parts.Length == 2 && int.TryParse(parts[1], out userId))
            {
                var chats = UserRepository.UserChat(userId, chatId);
                ViewBag.ChatId = chatId;

                ViewBag.RecieverName = UserRepository.GetUserById(chatId).FirstName.ToUpper() + " "+ UserRepository.GetUserById(chatId).LastName.ToUpper();
                return View(chats);
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid user format.";
                return View();
            }
            
        }

    }
}


