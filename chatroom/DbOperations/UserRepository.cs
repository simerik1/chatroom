using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace chatroom.DbOperations
{
    public class UserRepository
    {
        private chatroomEntities context;

        public UserRepository()
        {
            context = new chatroomEntities();
        }

        public List<UsersModel> GetAllUsers()
        {
            using (var context = new chatroomEntities())
            {
                var result = context.UserData.Select(x => new UsersModel()
                {
                    Id = x.id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    UserName = x.UserName,
                    ProfilePicture = x.ProfilePicture
                }).ToList();
                return result;
            }
        }

        public UsersModel Profile_Details(int id)
        {
            using (var context = new chatroomEntities())
            {
                var result = context.UserData.Where(x => x.id == id).Select(x => new UsersModel()
                {
                    Id = x.id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    UserName = x.UserName,
                    ProfilePicture = x.ProfilePicture
                }).FirstOrDefault();
                return result;
            }
        }

        public UsersModel GetUserById(int id)
        {
            var userDataEntity = context.UserData.Find(id);
            if (userDataEntity != null)
            {
                return new UsersModel
                {
                    Id = userDataEntity.id,
                    FirstName = userDataEntity.FirstName,
                    LastName = userDataEntity.LastName,
                    Email = userDataEntity.Email,
                    UserName = userDataEntity.UserName,
                    ProfilePicture = userDataEntity.ProfilePicture
                };
            }
            return null;
        }

        public void SendFriendRequest(UsersModel senderUser, UsersModel receiverUser)
        {

            var friendship = new FriendRequests
            {
                SenderId = senderUser.Id,
                ReceiverId = receiverUser.Id,
                status = FriendRequestStatus.Pending.ToString()

            };

            context.FriendRequests.Add(friendship);
            context.SaveChanges();
        }
        public bool AreFriends(int userId1, int userId2)
        {
            return context.FriendRequests.Any(r =>
               (r.SenderId == userId1 && r.ReceiverId == userId2 || r.SenderId == userId2 && r.ReceiverId == userId1)
             && r.status == FriendRequestStatus.Pending.ToString());

        }
        public List<FriendRequest> GetPendingFriendRequests(int userId)
        {
            using (var context = new chatroomEntities())
            {
                var pendingRequests = context.FriendRequests
                    .Where(r => r.ReceiverId == userId && r.status == FriendRequestStatus.Pending.ToString())
                    .ToList();

                var result = new List<FriendRequest>();

                // store data in result
                foreach (var request in pendingRequests)
                {
                    var sender = GetUserById(request.SenderId);
                    if (sender != null)
                    {
                        var friendRequest = new FriendRequest
                        {
                            SenderId = request.SenderId,
                            SenderName = $"{sender.FirstName} {sender.LastName}",
                            ReceiverId = request.ReceiverId,
                            ProfilePicture = sender.ProfilePicture
                        };
                        result.Add(friendRequest);
                    }
                }
                return result;
            }
        }
        public bool AcceptFriendRequest(int requestId, int userId)
        {
            var request = context.FriendRequests.FirstOrDefault(x => x.SenderId == requestId
            && x.ReceiverId == userId && x.status == FriendRequestStatus.Pending.ToString());
            if (request != null)
            {
                request.status = FriendRequestStatus.Accepted.ToString();
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RejectFriendRequest(int requestId, int userId)
        {
            var request = context.FriendRequests.FirstOrDefault(y => y.SenderId == requestId
            && y.ReceiverId == userId && y.status == FriendRequestStatus.Pending.ToString());
            if (request != null)
            {
                request.status = FriendRequestStatus.Declined.ToString();
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public List<FriendRequest> FriendsList(int userId)
        {
            using (var context = new chatroomEntities())
            {
                var friends_list = context.FriendRequests
                    .Where(r => r.ReceiverId == userId && r.status == FriendRequestStatus.Accepted.ToString())
                    .ToList();

                var result = new List<FriendRequest>();

                foreach (var request in friends_list)
                {
                    var sender = GetUserById(request.SenderId);
                    if (sender != null)
                    {
                        var friendRequest = new FriendRequest
                        {
                            ProfilePicture = sender.ProfilePicture,
                            SenderId = request.SenderId,
                            SenderName = $"{sender.FirstName} {sender.LastName}",
                            ReceiverId = request.ReceiverId,
                        };
                        result.Add(friendRequest);
                    }
                }
                return result;
            }
        }
        public bool AcceptedFriendRequests(int requestId, int userId)
        {
            var request = context.FriendRequests.FirstOrDefault(y => y.SenderId == requestId
            && y.ReceiverId == userId && y.status == FriendRequestStatus.Accepted.ToString());
            if (request != null)
            {
                request.status = FriendRequestStatus.Declined.ToString();
                context.SaveChanges();
                return true;
            }
            return false;
        }

       
        public List<Models.Message> Chats(int userId)
        {
            using (var context = new chatroomEntities())
            {
                var latestMessages = context.Messages
                    .Where(m => m.SenderId == userId || m.RecieverId == userId)
                    .GroupBy(m => new { m.SenderId, m.RecieverId })
                    .Select(g => g.OrderByDescending(m => m.TimeStamp).FirstOrDefault())
                    .ToList();

                var result = new List<Models.Message>();
                foreach (var message in latestMessages)
                {
                    var sender = GetUserById(message.SenderId);
                    var receiver = context.UserData.FirstOrDefault(u => u.id == message.RecieverId);
                    if (sender != null && receiver != null)
                    {
                        var chat = new Message
                        {
                            SenderName = sender.FirstName + " " + sender.LastName,
                            ReceiverName = receiver.FirstName + " " + receiver.LastName,
                            Content = message.Content,
                            TimeStamp = message.TimeStamp,
                            RecieverId = message.RecieverId
                        };
                        result.Add(chat);
                    }
                }

                return result;
            }
        }

        public bool SendMessage(Messages message)
        { 
            context.Messages.Add(message);
            context.SaveChanges();
            return true;
        }
        public List<Models.Message> UserChat(int userId, int chatId)
        {
            using (var context = new chatroomEntities())
            {
                var chats = context.Messages
                    .Where(m => (m.SenderId == chatId && m.RecieverId == userId)||(m.SenderId == userId && m.RecieverId == chatId))
                    .ToList();

                var result = new List<Models.Message>();

                foreach (var chat in chats)
                {
                    var sender = GetUserById(chat.SenderId);
                    var receiver = GetUserById(chat.RecieverId);
                    bool isSent = chat.SenderId == userId;

                    if (sender != null && receiver != null)
                    {
                        var message = new Message
                        {
                            SenderName = isSent ? sender.FirstName + " " + sender.LastName : receiver.FirstName + " " + receiver.LastName,
                            ReceiverName = isSent ? receiver.FirstName + " " + receiver.LastName : sender.FirstName + " " + sender.LastName,
                            Content = chat.Content,
                            TimeStamp = chat.TimeStamp,
                            IsSent = isSent

                        };
                        result.Add(message);
                    }
                }

                return result;
            }

        }
    }
}