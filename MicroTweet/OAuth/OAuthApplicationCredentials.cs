using System;
using Microsoft.SPOT;

namespace MicroTweet
{
    /// <summary>
    /// Application-specific credentials
    /// </summary>
    public struct OAuthApplicationCredentials
    {
        /// <summary>
        /// Consumer Key (API Key)
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Consumer Secret (API Secret)
        /// </summary>
        public string ConsumerSecret { get; set; }
    }
}
