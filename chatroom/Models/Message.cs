using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chatroom.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int RecieverId { get; set; }
        public string Content { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string SenderName { get; set;}
        public string ReceiverName { get; set; }

    }
}