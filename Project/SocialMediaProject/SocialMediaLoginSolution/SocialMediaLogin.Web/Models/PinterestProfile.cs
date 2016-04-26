using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMediaLogin.Web
{
    public class PinterestUserDetails
    {
        public PinterestProfile data;
    }

    public class PinterestProfile
    {
        public string url { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id { get; set; }
        public string image { get; set; }
    }
}