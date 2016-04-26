using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMediaLogin.Web
{
    public class ProfileInfo
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfileURL { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}