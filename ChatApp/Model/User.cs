using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class User
    {
        public string Username { get; set; }
        public string _Id { get; set; }
        public Message LastMessage { get; set; }
    }
}
