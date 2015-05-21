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

            // Update the current time (since Twitter OAuth API requests require a valid timestamp)
            DateTime utcTime = Sntp.GetCurrentUtcTime();
            Microsoft.SPOT.Hardware.Utility.SetLocalTime(utcTime);

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

            try
            {
                // Send a tweet
                var tweet = twitter.SendTweet("Trying out MicroTweet!");
                Debug.Print("Posted tweet with ID: " + tweet.ID);
            }
            catch (Exception e)
            {
                Debug.Print("Could not send tweet.");
                Debug.Print(e.ToString());
            }
        }
    }
}
