using Facebook;
using LinkedIn.NET;
using LinkedIn.NET.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SocialMediaLogin.Web
{
    public class LinkedinUtility
    {

        #region [Declaration]

        private LinkedInClient linkedIn;

        #endregion

        #region [Method]

        /// <summary>
        /// Constractor
        /// </summary>
        public LinkedinUtility()
        {
            linkedIn = new LinkedInClient(WebConfigurationManagement.LinkedinAppID, WebConfigurationManagement.LinkedinAppSecretKey);
        }

        /// <summary>
        /// Get Linkedin URL
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizationUrl()
        {
            return linkedIn.GetAuthorizationUrl(new LinkedInAuthorizationOptions { RedirectUrl = WebConfigurationManagement.LinkedinRedirect_uri, State = Guid.NewGuid().ToString(), Permissions = LinkedInPermissions.EmailAddress });
        }

        /// <summary>
        /// Get User Info
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ProfileInfo GetUserInfo(string code, string state)
        {
            var userProfilinfo = new ProfileInfo();
            var Accesstoan = linkedIn.GetAccessToken(code, WebConfigurationManagement.LinkedinRedirect_uri);

            if (Accesstoan.Status == LinkedInResponseStatus.OK)
            {
                LinkedInGetMemberOptions lnoption = new LinkedInGetMemberOptions();
                lnoption.BasicProfileOptions.SelectAll();
                lnoption.EmailProfileOptions.SelectAll();
                lnoption.FullProfileOptions.SelectAll();
                var Userinfo = linkedIn.GetMember(lnoption);
                if (Userinfo.Status == LinkedInResponseStatus.OK)
                {
                    userProfilinfo.FirstName = linkedIn.CurrentUser.FirstName;
                    userProfilinfo.LastName = linkedIn.CurrentUser.LastName;
                    //  userProfilinfo.Phone = Userinfo.Result.BasicProfile.PhoneticFirstName;
                    userProfilinfo.ProfilePicture = ((LinkedIn.NET.Members.LinkedInPerson)(Userinfo.Result.BasicProfile)).PictureUrl;
                    userProfilinfo.ProfileURL = Userinfo.Result.BasicProfile.PublicProfileUrl;
                    userProfilinfo.Email = Userinfo.Result.EmailProfile.EmailAddress;
                    userProfilinfo.IsSuccess = true;
                }
                else
                {
                    userProfilinfo.IsSuccess = false;
                    userProfilinfo.ErrorMessage = Userinfo.Message;
                }
            }
            else
            {
                userProfilinfo.IsSuccess = false;
                userProfilinfo.ErrorMessage = Accesstoan.Message;
            }
            return userProfilinfo;
        }

        #endregion
       
    }
}