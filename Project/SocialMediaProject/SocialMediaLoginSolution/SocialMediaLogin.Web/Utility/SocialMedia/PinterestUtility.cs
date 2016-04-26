using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace SocialMediaLogin.Web
{
    public class PinterestUtility
    {

        #region [Method]

        public string GetAuthorizationLink()
        {
            return string.Format("{0}/?client_id={1}&redirect_uri={2}&response_type=code&scope=read_public",
                WebConfigurationManagement.PinterestAuthorizeBaseUrl,
                WebConfigurationManagement.PinterestConsumerKey,
                HttpUtility.UrlEncode(WebConfigurationManagement.PinterestCallbackUrl));
        }

        public string GetAccessToken(string authorizationCode)
        {
            // Use the authorization code to request an access token.
            string postData = string.Format("grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}",
                                        WebConfigurationManagement.PinterestConsumerKey, WebConfigurationManagement.PinterestSecretKey, authorizationCode);

            var webClient = new WebClient();
            string responseData;
            try
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                responseData = webClient.UploadString(WebConfigurationManagement.PinterestAccessTokenUrl, postData);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("OAuth Web Request Failed", ex);
            }

            if (responseData.Length > 0)
            {
                //Store the returned access_token
                dynamic accessResponse = new JavaScriptSerializer().DeserializeObject(responseData);

                if (accessResponse["access_token"] != null)
                    return accessResponse["access_token"];
            }
            return null;
        }

        public ProfileInfo GetUserData(string token)
        {
            //// Use Pinterest API to get details about the user
            string pinterestUrl = string.Format("https://api.pinterest.com/v1/me?access_token={0}", token);

            var webClient = new WebClient();
            string responseData;
            try
            {
                responseData = webClient.DownloadString(pinterestUrl);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("OAuth Web Request Failed", ex);
            }

            if (responseData.Length > 0)
            {
                //Store the returned access_token
                PinterestUserDetails userDetails = new JavaScriptSerializer().Deserialize<PinterestUserDetails>(responseData);

                var userData = new ProfileInfo()
                {
                    FirstName = userDetails.data.first_name,
                    LastName = userDetails.data.last_name,
                    ProfileURL = userDetails.data.url,
                    ProfilePicture = userDetails.data.image
                };

                return userData;
            }
            return null;
        }

        #endregion

    }
}