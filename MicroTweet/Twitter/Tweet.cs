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
            bool isRetweet = false;
            Hashtable tweetData = data;
            if (data.Contains("retweeted_status"))
            {
                isRetweet = true;
                tweetData = (Hashtable)data["retweeted_status"];
            }

            // Fill property values with the data we received
            ID = (long)tweetData["id"];
            Text = (string)tweetData["text"];
            CreatedAt = Utility.ParseTwitterDateTime((string)tweetData["created_at"]);
            Source = (string)tweetData["source"];
            RetweetCount = (int)(long)tweetData["retweet_count"];
            if (data.Contains("favorite_count"))
                FavoriteCount = (int)(long)tweetData["favorite_count"];
            User = new User((Hashtable)tweetData["user"]);

            if (isRetweet)
            {
                IsRetweet = true;
                RetweetedAt = Utility.ParseTwitterDateTime((string)data["created_at"]);
                RetweetedByUser = new User((Hashtable)data["user"]);
            }
        }

        /// <summary>
        /// Gets the ID of the tweet.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether this is a retweet.
        /// </summary>
        public bool IsRetweet { get; private set; }

        /// <summary>
        /// Gets the content of the tweet, e.g., "just setting up my twttr".
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the date/time the tweet was posted.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// If this is a retweet, gets the date/time the tweet was retweeted.
        /// </summary>
        public DateTime RetweetedAt { get; private set; }

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

        /// <summary>
        /// If this is a retweet, gets the user who retweeted this tweet.
        /// </summary>
        public User RetweetedByUser { get; private set; }
    }
}
