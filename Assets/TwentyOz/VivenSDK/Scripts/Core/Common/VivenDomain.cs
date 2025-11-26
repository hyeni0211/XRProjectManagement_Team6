using System;
using static System.String;

namespace TwentyOz.VivenSDK.Scripts.Core.Common
{
    /// <exclude />
    /// <summary>
    /// Viven Domain 정보
    /// </summary>
    public static class VivenDomain
    {
        public static LoginDomain CurrentDomain { get; private set; }

        /// <summary>
        /// API Domain
        /// </summary>
        public static class API
        {
            private static readonly string PublicAPI = "https://playapi.viven.app";
            private static readonly string DevAPI    = "https://playapi.dev.viven.app";  //TODO
            private static readonly string BetaAPI   = "https://playapi.beta.viven.app"; //TODO

            public static string GetDomainAPI(LoginDomain loginDomain)
            {
                return loginDomain switch
                {
                    LoginDomain.None   => Empty,
                    LoginDomain.Public => PublicAPI,
                    LoginDomain.Dev    => DevAPI,
                    LoginDomain.Beta   => BetaAPI,
                    _                  => throw new ArgumentOutOfRangeException(nameof(loginDomain), loginDomain, null)
                };
            }
        }

        public static class WebURL
        {
            private static readonly string PublicWebUrl = "play.viven.app";
            private static readonly string DevWebUrl    = "play.dev.viven.app";  //TODO
            private static readonly string BetaWebUrl   = "play.beta.viven.app"; //TODO

            public static string GetDomainWebURL(LoginDomain loginDomain)
            {
                return loginDomain switch
                {
                    LoginDomain.None   => Empty,
                    LoginDomain.Public => PublicWebUrl,
                    LoginDomain.Dev    => DevWebUrl,
                    LoginDomain.Beta   => BetaWebUrl,
                    _                  => throw new ArgumentOutOfRangeException(nameof(loginDomain), loginDomain, null)
                };
            }

            public static string GetDomainWebURL()
            {
                return GetDomainWebURL(CurrentDomain);
            }
        }

        public static class CDN
        {
            private static readonly string PublicCDN = "https://cdn.viven.app";
            private static readonly string DevCDN    = "https://cdn.dev.viven.app";  //TODO
            private static readonly string BetaCDN   = "https://cdn.beta.viven.app"; //TODO

            public static string GetDomainCDN(LoginDomain loginDomain)
            {
                return loginDomain switch
                {
                    LoginDomain.None   => Empty,
                    LoginDomain.Public => PublicCDN,
                    LoginDomain.Dev    => DevCDN,
                    LoginDomain.Beta   => BetaCDN,
                    _                  => throw new ArgumentOutOfRangeException(nameof(loginDomain), loginDomain, null)
                };
            }

            public static string GetDomainCDN()
            {
                return GetDomainCDN(CurrentDomain);
            }
        }

        /// <summary>
        /// Domain을 변경해줌.
        /// </summary>
        /// <param name="domain"></param>
        public static void SetDomain(LoginDomain domain)
        {
            CurrentDomain = domain;
        }

        public static class DTS
        {
            private static readonly string PublicDTS = "ZHRzOi8vMTI1LjEzMC4xMjUuMTA4OjgyODI="; //추후 추가 예정
            private static readonly string DevDTS    = "ZHRzOi8vMTI1LjEzMC4xMjUuMTA4OjIwMDcx"; //TODO
            private static readonly string BetaDTS   = "ZHRzOi8vMTI1LjEzMC4xMjUuMTA4OjIwMDcy"; //TODO


            public static string GetDomainDTS(LoginDomain loginDomain)
            {
                return loginDomain switch
                {
                    LoginDomain.None   => Empty,
                    LoginDomain.Public => PublicDTS,
                    LoginDomain.Dev    => DevDTS,
                    LoginDomain.Beta   => BetaDTS,
                    _                  => throw new ArgumentOutOfRangeException(nameof(loginDomain), loginDomain, null)
                };
            }

            public static string GetDomainDTS()
            {
                return GetDomainDTS(CurrentDomain);
            }
        }
    }
}