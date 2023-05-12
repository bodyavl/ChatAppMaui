using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class Chat
    {
        public string Id { get; set; }

        public List<Message> messages { get; set; }
    }
}
