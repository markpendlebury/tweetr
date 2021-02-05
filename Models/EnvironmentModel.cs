using System;

namespace tweetr.Models
{
    public class EnvironmentModel
    {
        public string CONSUMER_KEY
        {
            get { return Environment.GetEnvironmentVariable("CONSUMER_KEY"); }
        }

        public string CONSUMER_SECRET
        {
            get { return Environment.GetEnvironmentVariable("CONSUMER_SECRET"); }
        }

    }

}