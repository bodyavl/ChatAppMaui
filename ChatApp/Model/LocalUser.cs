using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class LocalUser : BaseUser
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
