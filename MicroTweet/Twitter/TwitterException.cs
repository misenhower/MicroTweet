using System;
using Microsoft.SPOT;
using System.Collections;
using System.Net;

namespace MicroTweet
{
    /// <summary>
    /// The exception that is thrown when an error is received from Twitter's API.
    /// </summary>
    public class TwitterException : Exception
    {
        internal TwitterException(HttpStatusCode statusCode, string response)
        {
            _statusCode = statusCode;

            // Attempt to parse the JSON-encoded error message
            if (response != null)
            {
                try
                {
                    Hashtable data = (Hashtable)Json.Parse(response);
                    var errors = (ArrayList)data["errors"];
                    var error = (Hashtable)errors[0];
                    _message = (string)error["message"];
                    _errorCode = (int)(long)error["code"];
                }
                catch { }
            }
        }

        private readonly string _message;
        /// <summary>
        /// Gets the error message received from Twitter's API.
        /// </summary>
        public override string Message { get { return _message; } }

        private readonly HttpStatusCode _statusCode;
        /// <summary>
        /// Gets the HTTP status code received from Twitter's API.
        /// See the "HTTP Status Codes" section in Twitter's developer documentation for more information: https://dev.twitter.com/overview/api/response-codes
        /// </summary>
        public HttpStatusCode StatusCode { get { return _statusCode; } }

        private readonly int _errorCode;
        /// <summary>
        /// Gets the error code received from Twitter's API.
        /// See the "Error Codes" section in Twitter's developer documentation for more information: https://dev.twitter.com/overview/api/response-codes
        /// </summary>
        public int ErrorCode { get { return _errorCode; } }

        /// <summary>
        /// Returns the fully qualified name of this exception and possibly the error message.
        /// </summary>
        public override string ToString()
        {
            // Note: we have to override ToString because (unlike the desktop framework) NETMF's Exception class doesn't retrieve the message from the virtual Message property.

            string s = this.GetType().ToString();

            string message = Message;
            if (message != null && message.Length > 0)
                s += ": " + message;

            return s;
        }
    }
}
