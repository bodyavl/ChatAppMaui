﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class Message
    {
        public bool FromSelf { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
