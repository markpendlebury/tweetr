using System;

namespace tweetr.Models
{
    public class EnvironmentModel
    {
        public string CONSUMER_API_KEY
        {
            get { return Environment.GetEnvironmentVariable("CONSUMER_API_KEY"); }
        }

        public string CONSUMER_API_SECRET
        {
            get { return Environment.GetEnvironmentVariable("CONSUMER_API_SECRET"); }
        }

    }

}