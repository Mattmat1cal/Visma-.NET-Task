using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShortageSystem.Model.Enums;

namespace ShortageSystem.Model
{
    internal class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType? UserType { get; set; }
    }
}
