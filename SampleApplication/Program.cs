using System;
using Microsoft.SPOT;
using MicroTweet;
using System.Net;
using System.Threading;

namespace SampleApplication
{
    public class Program
    {
        public static void Main()
        {
            // Wait for DHCP
            while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
                Thread.Sleep(50);

            // Set up application and user credentials
            // Visit https://apps.twitter.com/ to create a new Twitter application and get API keys/user access tokens
            var appCredentials = new OAuthApplicationCredentials()
            {
                ConsumerKey = "YOUR_CONSUMER_KEY_HERE",
                ConsumerSecret = "YOUR_CONSUMER_SECRET_HERE",
            };
            var userCredentials = new OAuthUserCredentials()
            {
                AccessToken = "YOUR_ACCESS_TOKEN",
                AccessTokenSecret = "YOUR_ACCESS_TOKEN_SECRET",
            };

            // Create new Twitter client with these credentials
            var twitter = new TwitterClient(appCredentials, userCredentials);

            // Send a tweet
            twitter.SendTweet("Trying out MicroTweet!");
        }
    }
}
