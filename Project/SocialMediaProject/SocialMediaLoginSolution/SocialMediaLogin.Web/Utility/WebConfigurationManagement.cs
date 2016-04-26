using System;
using System.Configuration;

namespace SocialMediaLogin.Web
{
    public static class WebConfigurationManagement
    {
        #region [Other]

        public static string ApplicationWebHTTPURL
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ApplicationWebHTTPURL"]);
            }
        }

        public static string ApplicationWebHTTPSURL
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ApplicationWebHTTPSURL"]);
            }
        }

        #endregion

        #region [Facebook]

        public static string FacebookAppID
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["FacebookAppID"]);
            }
        }
        public static string FacebookAppSecretKey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["FacebookAppSecretKey"]);
            }
        }
        public static string FacebookRedirect_uri
        {
            get
            {
                return string.Format("{0}Home/FacebookCallback", WebConfigurationManagement.ApplicationWebHTTPURL);
            }
        }

        #endregion

        #region [GooglePlus]

        public static string GooglePlusAuthorizationEndpoint
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["GooglePlusAuthorizationEndpoint"]);
            }
        }
        public static string GooglePlusTokenEndpoint
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["GooglePlusTokenEndpoint"]);
            }
        }
        public static string GooglePlusUserInfoEndpoint
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["GooglePlusUserInfoEndpoint"]);
            }
        }
        public static string GooglePlusRedirect_uri
        {
            get
            {
                return string.Format("{0}Home/GooglePlusCallback", WebConfigurationManagement.ApplicationWebHTTPURL);

            }
        }

        

        #endregion

        #region [Instagram]

        public static string InstagramClientID
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["InstagramClientID"]);
            }
        }
        public static string InstagramClientSecret
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["InstagramClientSecret"]);
            }
        }
        public static string InstagramRedirect_uri
        {
            get
            {
                return string.Format("{0}Home/InstagramCallback", WebConfigurationManagement.ApplicationWebHTTPURL);

            }
        }

        #endregion

        #region [Linkedin]

        public static string LinkedinAppID
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["LinkedinClientID"]);
            }
        }
        public static string LinkedinAppSecretKey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["LinkedinClientSecret"]);
            }
        }
        public static string LinkedinRedirect_uri
        {
            get
            {
                return string.Format("{0}Home/LinkedinCallback", WebConfigurationManagement.ApplicationWebHTTPURL);
            }
        }

        #endregion

        #region [Pinterest]

        public static string PinterestAuthorizeBaseUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["PinterestAuthorizeBaseUrl"]);
            }
        }
        public static string PinterestAccessTokenUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["PinterestAccessTokenUrl"]);
            }
        }
        public static string PinterestConsumerKey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["PinterestConsumerKey"]);
            }
        }
        public static string PinterestCallbackUrl
        {
            get
            {
                return string.Format("{0}Home/PinterestCallBack", WebConfigurationManagement.ApplicationWebHTTPURL);
            }
        }
        public static string PinterestSecretKey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["PinterestSecretKey"]);
            }
        }


        #endregion

        #region [Twitter]

        public static string TwitterConsumerkey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["TwitterConsumerkey"]);
            }
        }
        public static string TwitterConsumerSecret
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["TwitterConsumerSecret"]);
            }
        }
        public static string TwitterRedirect_uri
        {
            get
            {
                return string.Format("{0}Home/TwitterCallback", WebConfigurationManagement.ApplicationWebHTTPURL);
            }
        }

        #endregion
    }
}