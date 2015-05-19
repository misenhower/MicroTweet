using System;
using Microsoft.SPOT;

namespace MicroTweet
{
    /// <summary>
    /// User-specific credentials
    /// </summary>
    public struct OAuthUserCredentials
    {
        /// <summary>
        /// Access Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Access Token Secret
        /// </summary>
        public string AccessTokenSecret { get; set; }
    }
}
