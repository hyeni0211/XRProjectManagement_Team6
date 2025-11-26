using UnityEngine;
using UnityEngine.Networking;

namespace TwentyOz.VivenSDK.Scripts.Core.Common
{
    /// <exclude />
    /// <summary>
    /// Viven Web API
    /// </summary>
    public static class VivenAPI
    {
        public static UnityWebRequest Logout()
        {
            return Logout(VivenDomain.CurrentDomain);
        }
        
        private static UnityWebRequest Logout(LoginDomain domain)
        {
            return UnityWebRequest.Get($"https://{VivenDomain.WebURL.GetDomainWebURL(domain)}/login/log-out");
        }
        
        public static UnityWebRequest Upload(WWWForm form)
        {
            return Upload(VivenDomain.CurrentDomain, form);
        }

        private static UnityWebRequest Upload(LoginDomain domain, WWWForm form)
        {
            return UnityWebRequest.Post("http://localhost:3000/upload", form);
        }

        public static UnityWebRequest GetUserProfile(string userToken)
        {
            return GetUserProfile(VivenDomain.CurrentDomain, userToken);
        }
        
        private static UnityWebRequest GetUserProfile(LoginDomain domain, string userToken)
        {
            var request = UnityWebRequest.Get($"{VivenDomain.API.GetDomainAPI(domain)}/login/user-info/");
            request.SetRequestHeader("Authorization", $"Bearer {userToken}");
            return request;
        }

        public static UnityWebRequest GetLoginToken(WWWForm form)
        {
            return GetLoginToken(VivenDomain.CurrentDomain, form);
        }

        private static UnityWebRequest GetLoginToken(LoginDomain domain, WWWForm form)
        {
            return UnityWebRequest.Post($"{VivenDomain.API.GetDomainAPI(domain)}/login/get-token", form);
        }
    }
}