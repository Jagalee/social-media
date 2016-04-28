using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using PinSharp;

namespace SocialMediaLogin.Web
{
    public class PinterestUtility
    {
        private string PinterestConsumerkey = ConfigurationManager.AppSettings["TwitterConsumerkey"].ToString();
        private string PinterestConsumersecret = ConfigurationManager.AppSettings["TwitterConsumersecret"].ToString();
        private string PinterestRedirect_uri = ConfigurationManager.AppSettings["TwitterRedirect_uri"].ToString();
        private string PinterestAccessTokan = "";
        private PinterestClient PinterestServices;

        public PinterestUtility()
        {
            PinterestServices = new PinterestClient();
        }

        public string GetAuthorizationUrl()
        {
            OAuthRequestToken requestToken = twitterService.GetRequestToken(TwitterRedirect_uri);
            var uri = twitterService.GetAuthenticationUrl(requestToken);
            return uri.AbsoluteUri;
        }

        public ProfileInfo GetUserInfo(string oauth_token, string oauth_verifier)
        {
            var userProfilinfo = new ProfileInfo();
            try
            {
                var requestToken = new OAuthRequestToken { Token = oauth_token };

                OAuthAccessToken accessToken = twitterService.GetAccessToken(requestToken, oauth_verifier);
                twitterService.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
                if (twitterService.Response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TwitterUser user = twitterService.GetUserProfile(new GetUserProfileOptions { IncludeEntities = false, SkipStatus = true });

                    string[] userName = user.Name.Split(' ');
                    userProfilinfo.FirstName = userName[0];
                    userProfilinfo.LastName = userName[1];
                    userProfilinfo.ProfilePicture = user.ProfileImageUrl;
                    userProfilinfo.ProfileURL = "https://twitter.com/" + user.ScreenName;
                    userProfilinfo.Email = "";
                    userProfilinfo.IsSuccess = true;
                }
                else
                {
                    userProfilinfo.IsSuccess = false;
                    userProfilinfo.ErrorMessage = twitterService.Response.Error.Message;
                }
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