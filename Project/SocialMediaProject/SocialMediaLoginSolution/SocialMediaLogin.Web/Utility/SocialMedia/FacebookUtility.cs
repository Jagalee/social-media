using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SocialMediaLogin.Web
{
    public class FacebookUtility
    {
       
        private string FacebookAppAccessToken = string.Empty;
        private FacebookClient fb = new FacebookClient();

        public string GetLogin()
        {
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = WebConfigurationManagement.FacebookAppID,
                redirect_uri = WebConfigurationManagement.FacebookRedirect_uri,
                response_type = "code",
                scope = "email"
            });
            return loginUrl.AbsoluteUri;
        }

        public ProfileInfo GetUserInfo(string code)
        {
            var userProfilinfo = new ProfileInfo();
            try
            {
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = WebConfigurationManagement.FacebookAppID,
                    client_secret = WebConfigurationManagement.FacebookAppSecretKey,
                    redirect_uri = WebConfigurationManagement.FacebookRedirect_uri,
                    code = code
                });
                FacebookAppAccessToken = result.access_token;
                fb.AccessToken = FacebookAppAccessToken;
                dynamic GetData = fb.Get("me?fields=first_name,last_name,id,email,picture,link");
                userProfilinfo.ID = GetData.id;
                userProfilinfo.Email = GetData.email;
                userProfilinfo.FirstName = GetData.first_name;
                userProfilinfo.LastName = GetData.last_name;
                userProfilinfo.Phone = GetData.mobile_phone;
                userProfilinfo.ProfilePicture = GetData.picture.data.url; //"https://graph.facebook.com/" + me.id + "/picture";
                userProfilinfo.ProfileURL = GetData.link;
                userProfilinfo.IsSuccess = true;
            }
            catch (Exception ex)
            {
                userProfilinfo.IsSuccess = false;
                userProfilinfo.ErrorMessage = ex.Message;
            }
            return userProfilinfo;
        }
    }
}