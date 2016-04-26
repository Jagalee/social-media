using DotNetOpenAuth.AspNet;
using InstaSharp;
using InstaSharp.Models.Responses;
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

        /// <summary>
        /// Index method
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var profileInfo = new ProfileInfo();

            if (Session["SocialMediaProfileInfo"] != null)
            {
                profileInfo = (ProfileInfo)Session["SocialMediaProfileInfo"];
            }

            return View(profileInfo);
        }

        #region Facebook

        public ActionResult Facebook()
        {
            var fbUtility = new FacebookUtility();
            return Redirect(fbUtility.GetLogin());
        }

        public ActionResult FacebookCallback(string code)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code))
            {
                var fbUtility = new FacebookUtility();
                profileInfo = fbUtility.GetUserInfo(code);
                Session["SocialMediaProfileInfo"] = profileInfo;
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion Facebook

        #region Linkedin

        public ActionResult Linkedin()
        {
            LinkedinUtility ln = new LinkedinUtility();
            string url = ln.GetAuthorizationUrl();
            return new RedirectResult(url);
        }

        public ActionResult LinkedinCallback(string code, string state)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code) || string.IsNullOrEmpty(code))
            {
                var lnUtility = new LinkedinUtility();
                profileInfo = lnUtility.GetUserInfo(code, state);
                Session["SocialMediaProfileInfo"] = profileInfo;
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion Linkedin

        #region Twitter

        public ActionResult Twitter()
        {
            var twitterUtility = new TwitterUtility();
            return Redirect(twitterUtility.GetAuthorizationUrl());
        }

        public ActionResult TwitterCallback(string oauth_token, string oauth_verifier)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(oauth_token) || !string.IsNullOrEmpty(oauth_verifier))
            {
                var twitterUtility = new TwitterUtility();
                profileInfo = twitterUtility.GetUserInfo(oauth_token, oauth_verifier);
                Session["SocialMediaProfileInfo"] = profileInfo;
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion twitter

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
                Session["SocialMediaProfileInfo"] = profileInfo;

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = e.Message;

                return RedirectToAction("Index", "Home");
            }
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
            var pinterestLogin = new PinterestUtility();
            return Redirect(pinterestLogin.GetAuthorizationLink());
        }

        [HttpGet]
        public ActionResult PinterestCallBack(string code)
        {
            ProfileInfo profileInfo = new ProfileInfo();
            if (!string.IsNullOrEmpty(code))
            {
                var pinterestLogin = new PinterestUtility();
                profileInfo = pinterestLogin.GetUserData(pinterestLogin.GetAccessToken(code));
                Session["SocialMediaProfileInfo"] = profileInfo;
            }
            else
            {
                profileInfo.IsSuccess = false;
                profileInfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion Pinterest

        #region Instragram

        private InstagramConfig instagramConfig = new InstagramConfig(WebConfigurationManagement.InstagramClientID, WebConfigurationManagement.InstagramClientSecret, WebConfigurationManagement.InstagramRedirect_uri, "");



        public ActionResult Instagram()
        {
            return RedirectToAction("InstgramLogin");
        }

        public ActionResult InstgramLogin()
        {
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
                Session["SocialMediaProfileInfo"] = userProfilinfo;
            }
            else
            {
                userProfilinfo.IsSuccess = false;
                userProfilinfo.ErrorMessage = "This app is still in development mode, and you don't have access to it. Switch to a registered test user or ask an app admin for permissions";
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}