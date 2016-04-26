using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using SocialMediaLogin.Web;
using System.Configuration;

namespace GooglePlusOAuthLogin
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            string clientId = ConfigurationManager.AppSettings["GooglePlusClientID"].ToString();
            string secretKey = ConfigurationManager.AppSettings["GooglePlusSecretKey"].ToString();

            OAuthWebSecurity.RegisterClient(new GooglePlusUtility(clientId, secretKey), "Google+", null);
        }
    }
}
