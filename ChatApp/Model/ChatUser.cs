using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class ChatUser : BaseUser
    {
        public Message LastMessage { get; set; }
        public string LastMessageTime { get; set; }
        public bool IsSentMessage { get; set; } = false;
    }
}
