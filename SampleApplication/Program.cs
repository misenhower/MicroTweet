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

            // Verify the credentials and get the current user's account info
            try
            {
                var currentUser = twitter.VerifyCredentials();
                Debug.Print("Authenticated user account: @" + currentUser.ScreenName);
            }
            catch (Exception e)
            {
                Debug.Print("Could not verify account credentials.");
                Debug.Print(e.ToString());
            }

            // Send a tweet
            try
            {
                var tweet = twitter.SendTweet("Trying out MicroTweet!");
                Debug.Print("Posted a new tweet with ID: " + tweet.ID);
            }
            catch (Exception e)
            {
                Debug.Print("Could not send tweet.");
                Debug.Print(e.ToString());
            }

            // Get recent tweets from the home timeline
            try
            {
                var tweets = twitter.GetHomeTimeline(5);
                Debug.Print("Recent tweets from your timeline:");
                foreach (Tweet tweet in tweets)
                    Debug.Print("  Tweet from @" + tweet.User.ScreenName + ": \"" + tweet.Text + "\"");
            }
            catch (Exception e)
            {
                Debug.Print("Could not retrieve timeline.");
                Debug.Print(e.ToString());
            }

            // Get recent tweets from a specific user
            try
            {
                var tweets = twitter.GetUserTimeline("twitter");
                Debug.Print("Recent tweets from @twitter's timeline:");
                foreach (Tweet tweet in tweets)
                    Debug.Print("  Tweet from @" + tweet.User.ScreenName + ": \"" + tweet.Text + "\"");
            }
            catch (Exception e)
            {
                Debug.Print("Could not retrieve timeline.");
                Debug.Print(e.ToString());
            }
        }
    }
}
