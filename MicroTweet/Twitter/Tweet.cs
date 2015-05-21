using System;
using Microsoft.SPOT;
using System.Collections;

namespace MicroTweet
{
    /// <summary>
    /// Represents a tweet (status update)
    /// </summary>
    public class Tweet
    {
        internal Tweet(Hashtable data)
        {
            // Fill property values with the data we received
            ID = (long)data["id"];
            Text = (string)data["text"];
            CreatedAt = Utility.ParseTwitterDateTime((string)data["created_at"]);
            Source = (string)data["source"];
            RetweetCount = (int)(long)data["retweet_count"];
            if (data.Contains("favorite_count"))
                FavoriteCount = (int)(long)data["favorite_count"];
            User = new User((Hashtable)data["user"]);
        }

        /// <summary>
        /// Gets the ID of the tweet.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the content of the tweet, e.g., "just setting up my twttr".
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the date/time the tweet was posted.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the source of the tweet as an HTML anchor tag, e.g., "&lt;a href=\"http://twitter.com\" rel=\"nofollow\"&gt;Twitter Web Client&lt;/a&gt;".
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets the number of times this tweet has been retweeted.
        /// </summary>
        public int RetweetCount { get; private set; }

        /// <summary>
        /// Gets the number of times this tweet has been favorited.
        /// </summary>
        public int FavoriteCount { get; private set; }

        /// <summary>
        /// Gets the user who posted this tweet.
        /// </summary>
        public User User { get; private set; }
    }
}
