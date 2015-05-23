using System;
using Microsoft.SPOT;
using System.Collections;

namespace MicroTweet
{
    /// <summary>
    /// Represents a Twitter user account.
    /// </summary>
    public class User
    {
        internal User(Hashtable data)
        {
            // Fill property values with the data we received
            ID = (long)data["id"];
            if (data.Contains("created_at"))
                CreatedAt = Utility.ParseTwitterDateTime((string)data["created_at"]);
            if (data.Contains("screen_name"))
                ScreenName = (string)data["screen_name"];
            if (data.Contains("name"))
                Name = (string)data["name"];
            if (data.Contains("description"))
                Description = (string)data["description"];
            if (data.Contains("location"))
                Location = (string)data["location"];
            if (data.Contains("url"))
                URL = (string)data["url"];
            if (data.Contains("followers_count"))
                FollowerCount = (int)(long)data["followers_count"];
            if (data.Contains("friends_count"))
                FollowingCount = (int)(long)data["friends_count"];
            if (data.Contains("statuses_count"))
                TweetCount = (int)(long)data["statuses_count"];
        }

        /// <summary>
        /// Gets the ID of the user account.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the date/time the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the screen name (username) for the account.
        /// </summary>
        public string ScreenName { get; private set; }

        /// <summary>
        /// Gets the user's name (e.g., "John Doe").
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the user's description (bio).
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the location provided by the user in association with their profile (e.g., "San Francisco, CA").
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Gets the URL provided by the user in association with their profile.
        /// </summary>
        public string URL { get; private set; }

        /// <summary>
        /// Gets the number of followers this account currently has.
        /// </summary>
        public int FollowerCount { get; private set; }

        /// <summary>
        /// Gets the number of users this account is following.
        /// </summary>
        public int FollowingCount { get; private set; }

        /// <summary>
        /// Gets the number of tweets (including retweets) issued by the user.
        /// </summary>
        public int TweetCount { get; private set; }
    }
}
