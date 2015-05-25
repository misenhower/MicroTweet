# MicroTweet
MicroTweet is a Twitter API Library for the .NET Micro Framework.
It can be used to post and retreive tweets directly from a NETMF device via Twitter's API.

MicroTweet requires NETMF v4.3 and a board that supports SSL.

## Getting Started
A sample application is included that shows a few of the primary functions of MicroTweet (including sending a tweet and retrieving your home timeline).
You can base your application off the included sample, or start a new application that references the MicroTweet library.

### Using MicroTweet in your own application
To reference the MicroTweet library, you can either add the MicroTweet project to your VS solution or simply install the library via NuGet.

To install it from NuGet, run the following command in the Package Manager Console:
```
Install-Package MicroTweet
```
Instructions for using NuGet and the Package Manager Console can be found [here](http://docs.nuget.org/consume/package-manager-console).

Once you've referenced the MicroTweet assembly from your application, add a `using` directive for the MicroTweet namespace at the top of your program file (e.g., Program.cs):
```cs
using MicroTweet;
```

### Setting the current time
**Important:** Twitter API requests must contain a valid (current) timestamp.
For your convenience, an SNTP class is included with MicroTweet to retrieve the current time from an NTP server.

You can update the current time on your board with the following code:
```cs
Microsoft.SPOT.Hardware.Utility.SetLocalTime(Sntp.GetCurrentUtcTime());
```

### Initializing TwitterClient
To use the Twitter API from your application you must first register it with Twitter to get a set of API keys.
Go to the [Twitter Application Manager](https://apps.twitter.com/) page to create a new application and get the necessary keys.

You will get two sets of keys: one for the application you created and one for your user account to access the API through that application.
These are known as the application credentials and the user credentials.

You can initialize a new instance of TwitterClient with these keys as follows:
```cs
// Set up application and user credentials
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
```

### Sending a Tweet
Use the `SendTweet` method to send a tweet:
```cs
try
{
    var tweet = twitter.SendTweet("Trying out MicroTweet!");
}
catch (Exception e)
{
    // Couldn't send the tweet, the exception may have more information
}
```
If an error is received from Twitter's API, a `TwitterException` will be thrown with further details.
The `Message` property of the exception will contain the actual error message received from Twitter.

### Retrieving your home timeline
Use the `GetHomeTimeline` method to retrieve the most recent tweets and retweets posted by people you follow:
```cs
try
{
    var tweets = twitter.GetHomeTimeline();
    foreach (Tweet tweet in tweets)
        Debug.Print("Tweet from @" + tweet.User.ScreenName + ": \"" + tweet.Text + "\"");
}
catch (Exception e)
{
    // Couldn't retrieve the timeline, the exception may have more information
}
```

### Retrieving a specific user's timeline
Use the `GetUserTimeline` method to view the tweets and retweets posted by a specific user:
```cs
try
{
    var tweets = twitter.GetUserTimeline("twitter");
    foreach (Tweet tweet in tweets)
        Debug.Print("Tweet from @" + tweet.User.ScreenName + ": \"" + tweet.Text + "\"");
}
catch (Exception e)
{
    // Couldn't retrieve the timeline, the exception may have more information
}
```

### Other features
The `TwitterClient` class has a few other useful methods:
- `GetCurrentUser` can be used to verify the supplied credentials (API keys). If the credentials are valid, a `User` object will be returned with details for the authenticating user.
- `GetUser` retrieves information for the user with the specified screen name or ID.
