using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chatroom.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public FriendRequestStatus status { get; set; }
        public UsersModel Sender { get; set; }
        public string SenderName { get; set; }
        public string ProfilePicture { get; set; }

    }
}