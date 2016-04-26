using Facebook;
using LinkedIn.NET;
using LinkedIn.NET.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using DotNetOpenAuth.AspNet.Clients;

namespace SocialMediaLogin.Web
{
    public class GooglePlusUtility : OAuth2Client
    {
        #region [Declaration]

        private readonly string clientId;
        private readonly string clientSecret;

        #endregion

        #region [Method]

        /// <summary>
        /// Get Access to Google Plus API
        /// </summary>
        /// <param name="clientId">Google plus client Id</param>
        /// <param name="clientSecret">Google Plus API Client Scret Key</param>
        public GooglePlusUtility(string clientId, string clientSecret)
           : base("GooglePlus")
        {
            if (clientId == null)
                throw new ArgumentNullException("clientId");
            if (clientSecret == null)
                throw new ArgumentNullException("clientSecret");

            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        /// <summary>
        /// Get Service URL
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            UriBuilder uriBuilder = new UriBuilder(WebConfigurationManagement.GooglePlusAuthorizationEndpoint);
            uriBuilder.AppendQueryArgument("client_id", clientId);
            uriBuilder.AppendQueryArgument("redirect_uri", returnUrl.GetLeftPart(UriPartial.Path));
            uriBuilder.AppendQueryArgument("response_type", "code");
            uriBuilder.AppendQueryArgument("scope", "https://www.googleapis.com/auth/plus.login https://www.googleapis.com/auth/userinfo.email");
            uriBuilder.AppendQueryArgument("state", returnUrl.Query.Substring(1));

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Extract Google Plus response and get UserInfo
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            UriBuilder uriBuilder = new UriBuilder(WebConfigurationManagement.GooglePlusUserInfoEndpoint);
            uriBuilder.AppendQueryArgument("access_token", accessToken);

            WebRequest webRequest = WebRequest.Create(uriBuilder.Uri);
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null)
                            return null;

                        StreamReader streamReader = new StreamReader(responseStream);

                        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(streamReader.ReadToEnd());

                        // Add a username field in the dictionary
                        if (values.ContainsKey("email") && !values.ContainsKey("username"))
                            values.Add("username", values["email"]);

                        return values;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get Google Plus Access token
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            // Build up the form post data
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("code", authorizationCode);
            values.Add("client_id", clientId);
            values.Add("client_secret", clientSecret);
            values.Add("redirect_uri", returnUrl.GetLeftPart(UriPartial.Path));
            values.Add("grant_type", "authorization_code");
            string postData = String.Join("&",
                values.Select(x => Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value))
                      .ToArray());

            // Build up the request
            WebRequest webRequest = WebRequest.Create(WebConfigurationManagement.GooglePlusTokenEndpoint);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postData.Length;
            webRequest.Method = "POST";
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                StreamWriter streamWriter = new StreamWriter(requestStream);
                streamWriter.Write(postData);
                streamWriter.Flush();
            }

            // Process the response
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        StreamReader streamReader = new StreamReader(responseStream);

                        dynamic response = JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
                        return (string)response.access_token;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Re-write request to google plus
        /// </summary>
        public static void RewriteRequest()
        {
            var ctx = HttpContext.Current;

            var stateString = HttpUtility.UrlDecode(ctx.Request.QueryString["state"]);
            if (stateString == null || !stateString.Contains("__provider__=GooglePlus"))
                return;

            var q = HttpUtility.ParseQueryString(stateString);
            q.Add(ctx.Request.QueryString);
            q.Remove("state");

            ctx.RewritePath(ctx.Request.Path + "?" + q);
        }

        #endregion
    }
}