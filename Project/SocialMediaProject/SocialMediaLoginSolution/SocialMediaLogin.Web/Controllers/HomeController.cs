using DotNetOpenAuth.AspNet;
using InstaSharp;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SocialMediaLogin.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Private

        private FacebookUtility fbUtility = new FacebookUtility();
        private LinkedinUtility linkedinUtility = new LinkedinUtility();
        private TwitterUtility twitterUtility = new TwitterUtility();
        private PinterestUtility pinterestLogin = new PinterestUtility();
        private InstagramConfig instagramConfig = new InstagramConfig(WebConfigurationManagement.InstagramClientID, WebConfigurationManagement.InstagramClientSecret, WebConfigurationManagement.InstagramRedirect_uri, "");

        #endregion Private

        /// <summary>
        /// Index method
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(ProfileInfo profileInfo)
        {
            return View(profileInfo);
        }

        #region Facebook

        public ActionResult Facebook()
        {
            return Redirect(fbUtility.GetLogin());
        }

        public ActionResult FacebookCallback(string code)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code))
            {
                profileInfo = fbUtility.GetUserInfo(code);
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }

            return RedirectToAction("Index", "Home", profileInfo);
        }

        #endregion Facebook

        #region Linkedin

        public ActionResult Linkedin()
        {
            string url = linkedinUtility.GetAuthorizationUrl();
            return new RedirectResult(url);
        }

        public ActionResult LinkedinCallback(string code, string state)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code) || string.IsNullOrEmpty(code))
            {
                profileInfo = linkedinUtility.GetUserInfo(code, state);
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }
            return RedirectToAction("Index", "Home", profileInfo);
        }

        #endregion Linkedin

        #region Twitter

        public ActionResult Twitter()
        {
            return Redirect(twitterUtility.GetAuthorizationUrl());
        }

        public ActionResult TwitterCallback(string oauth_token, string oauth_verifier)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(oauth_token) || !string.IsNullOrEmpty(oauth_verifier))
            {
                profileInfo = twitterUtility.GetUserInfo(oauth_token, oauth_verifier);
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }

            return RedirectToAction("Index", "Home", profileInfo);
        }

        #endregion Twitter

        #region GooglePlus

        [HttpGet]
        public ActionResult GooglePlusExternalLogin(string provider)
        {
            return new ExternalLoginResult(provider, WebConfigurationManagement.GooglePlusRedirect_uri);
        }

        [HttpGet]
        public ActionResult GooglePlusCallback(string returnUrl)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            GooglePlusUtility.RewriteRequest();

            try
            {
                AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("GooglePlusCallback", new { ReturnUrl = returnUrl }));
                if (!result.IsSuccessful)
                {
                    profileInfo.IsSuccess = false;
                    profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";

                    return RedirectToAction("Index", "Home");
                }

                var list = result.ExtraData.ToList();

                foreach (var item in list)
                {
                    if (string.Compare(item.Key, "given_name", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        profileInfo.FirstName = item.Value.ToString();
                    }

                    if (string.Compare(item.Key, "family_name", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        profileInfo.LastName = item.Value.ToString();
                    }

                    if (string.Compare(item.Key, "email", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        profileInfo.Email = item.Value.ToString();
                    }

                    if (string.Compare(item.Key, "picture", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        profileInfo.ProfilePicture = item.Value.ToString();
                    }

                    if (string.Compare(item.Key, "link", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        profileInfo.ProfileURL = item.Value.ToString();
                    }
                }

                profileInfo.IsSuccess = true;
            }
            catch (Exception e)
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = e.Message;
            }
            return RedirectToAction("Index", "Home", profileInfo);
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #endregion GooglePlus

        #region Pinterest

        [HttpGet]
        public ActionResult Pinterest()
        {
            return Redirect(pinterestLogin.GetAuthorizationLink());
        }

        [HttpGet]
        public ActionResult PinterestCallBack(string code)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code))
            {
                profileInfo = pinterestLogin.GetUserData(pinterestLogin.GetAccessToken(code));
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }
            return RedirectToAction("Index", "Home", profileInfo);
        }

        #endregion Pinterest

        #region Instragram

        public ActionResult Instagram()
        {
            string a = string.Empty;
            var scopes = new List<OAuth.Scope>();
            scopes.Add(InstaSharp.OAuth.Scope.Basic);
            scopes.Add(InstaSharp.OAuth.Scope.Public_Content);
            var link = InstaSharp.OAuth.AuthLink(instagramConfig.OAuthUri + "authorize", instagramConfig.ClientId, instagramConfig.RedirectUri, scopes, InstaSharp.OAuth.ResponseType.Code);
            return Redirect(link);
        }

        public async Task<ActionResult> InstagramCallback(string code)
        {
            var userProfilinfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var auth = new OAuth(instagramConfig);
                    var oauthResponse = await auth.RequestToken(code);
                    string[] userName = oauthResponse.User.FullName.Split(' ');
                    userProfilinfo.FirstName = userName[0];
                    if (userName.Count() > 1)
                    {
                        userProfilinfo.LastName = userName[1];
                    }

                    userProfilinfo.ProfilePicture = oauthResponse.User.ProfilePicture;
                    userProfilinfo.ProfileURL = "https://www.instagram.com/" + oauthResponse.User.Username;
                    userProfilinfo.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    userProfilinfo.IsSuccess = false;
                    userProfilinfo.ErrorMessage = ex.Message;
                }
            }
            else
            {
                userProfilinfo.IsSuccess = false;
                userProfilinfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }
            return RedirectToAction("Index", "Home", userProfilinfo);
        }

        #endregion Instragram
    }
}