using System;
using Microsoft.SPOT;

namespace MicroTweet
{
    internal struct QueryParameter
    {
        public QueryParameter(string key, string value)
        {
            _key = key;
            _value = value;
        }

        private readonly string _key;
        public string Key { get { return _key; } }

        private readonly string _value;
        public string Value { get { return _value; } }
    }
}
