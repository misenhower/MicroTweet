using System;
using Microsoft.SPOT;
using System.Net;
using System.Collections;
using System.Text;

namespace MicroTweet
{
    internal static class OAuth
    {
        /// <summary>
        /// Generates an OAuth Authorization header and adds it to the headers of the specified HttpWebRequest.
        /// </summary>
        /// <param name="webRequest">The HttpWebRequest object to sign.</param>
        /// <param name="applicationCredentials">The OAuth application credentials.</param>
        /// <param name="userCredentials">The OAuth user credentials.</param>
        /// <param name="parameters">An IEnumerable collection of QueryParameters. For GET requests, this should include all parameters in the query string. For POST requests, this should include all parameters in the request body.</param>
        public static void SignHttpWebRequest(HttpWebRequest webRequest, OAuthApplicationCredentials applicationCredentials, OAuthUserCredentials userCredentials, IEnumerable parameters = null)
        {
            // Get the OAuth parameters
            ArrayList oauthParameters = GetOAuthParameters(applicationCredentials, userCredentials);

            // Create a collection of all parameters (both OAuth authorization parameters and request parameters) for the OAuth signature
            ArrayList allParameters = new ArrayList();
            foreach (var p in oauthParameters)
                allParameters.Add(p);
            if (parameters != null)
            {
                foreach (var p in parameters)
                    allParameters.Add(p);
            }

            // Get the OAuth signature
            string oauthSignature = GenerateOAuthSignature(webRequest.Method, webRequest.RequestUri.AbsoluteUri, allParameters, applicationCredentials, userCredentials);

            // Add the signature to the OAuth parameters
            oauthParameters.Add(new QueryParameter("oauth_signature", oauthSignature));

            // Generate the Authorization header string
            // Format: OAuth key1="value1", key2="value2", key3="value3"
            string authorizationHeader = "OAuth ";
            for (int i = 0; i < oauthParameters.Count; i++)
            {
                if (i > 0)
                    authorizationHeader += ", ";

                QueryParameter parameter = (QueryParameter)oauthParameters[i];
                authorizationHeader += Utility.UriEncode(parameter.Key) + "=\"" + Utility.UriEncode(parameter.Value) + "\"";
            }

            // Add the Authorization header to the request
            webRequest.Headers.Add("Authorization", authorizationHeader);
        }

        private static ArrayList GetOAuthParameters(OAuthApplicationCredentials applicationCredentials, OAuthUserCredentials userCredentials)
        {
            ArrayList result = new ArrayList();

            result.Add(new QueryParameter("oauth_consumer_key", applicationCredentials.ConsumerKey));
            result.Add(new QueryParameter("oauth_nonce", Utility.GetRandomAlphanumericString(16)));
            result.Add(new QueryParameter("oauth_signature_method", "HMAC-SHA1"));
            result.Add(new QueryParameter("oauth_timestamp", DateTime.UtcNow.ToUnixTimestamp().ToString()));
            result.Add(new QueryParameter("oauth_version", "1.0"));
            result.Add(new QueryParameter("oauth_token", userCredentials.AccessToken));

            return result;
        }

        private static string GenerateOAuthSignature(string method, string uri, IList parameters, OAuthApplicationCredentials applicationCredentials, OAuthUserCredentials userCredentials)
        {
            // Percent-encode each parameter
            string[] encodedParameters = new string[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter parameter = (QueryParameter)parameters[i];
                encodedParameters[i] = Utility.UriEncode(parameter.Key) + "=" + Utility.UriEncode(parameter.Value);
            }

            // Sort the list of parameters alphabetically
            Utility.SortList(encodedParameters);

            // Join the parameters as a query string
            string joinedParameters = Utility.StringJoin("&", encodedParameters);

            // Generate the signature base
            string signatureBase = method + '&' + Utility.UriEncode(uri) + '&' + Utility.UriEncode(joinedParameters);

            // Generate the key
            string key = applicationCredentials.ConsumerSecret + '&' + userCredentials.AccessTokenSecret;

            // Compute the HMAC-SHA1 hash
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(signatureBase);
            byte[] hashBytes = SHA.ComputeHmacSHA1(keyBytes, messageBytes);

            // The final signature should be a Base64-encoded version of the hash
            return Utility.ConvertToRFC4648Base64String(hashBytes);
        }
    }
}
