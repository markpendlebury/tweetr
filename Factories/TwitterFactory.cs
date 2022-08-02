using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pastel;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using tweetr.Helpers;
using tweetr.Models;

namespace tweetr.Factories
{
    public class TwitterFactory
    {
        private int initialPageSize = 10;
        private double scanDelay = 0.3; // Seconds
        private double searchDelay = 60; // Seconds

        private readonly InternalHelpers internalHelpers;
        private readonly Colors colors;

        public TwitterFactory(InternalHelpers internalHelpers)
        {
            this.internalHelpers = internalHelpers;
            this.colors = new Colors();
        }
        internal async Task SendTweet(TwitterClient userClient, IAuthenticatedUser user)
        {

            internalHelpers.DisplayLogo(user);
            Console.WriteLine("Send a Tweet:");
            Console.WriteLine();
            Console.Write(":> ");
            string tweetBody = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Sending tweet...");
            await userClient.Tweets.PublishTweetAsync(tweetBody);
            Console.WriteLine("Sent!");
            Thread.Sleep(1);
        }

        internal async Task LoadLiveTimeline(TwitterClient userClient, IAuthenticatedUser user)
        {
            try
            {
                internalHelpers.DisplayLogo(user);
                Console.WriteLine();
                Console.WriteLine("Retrieving timeline..");

                var homeTimelineTweets = await userClient.Timelines.GetHomeTimelineAsync(new GetHomeTimelineParameters() { PageSize = initialPageSize });
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------------".Pastel(colors.seperatorColor));

                ITweet finalTweet = null;

                if (homeTimelineTweets.Length > 0)
                {
                    homeTimelineTweets = homeTimelineTweets.OrderBy(t => t.CreatedAt.UtcDateTime).ToArray();
                    foreach (var tweet in homeTimelineTweets)
                    {
                        internalHelpers.DisplayTweet(tweet);
                        finalTweet = tweet;
                        Thread.Sleep(TimeSpan.FromSeconds(scanDelay));
                    }
                }

                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(searchDelay));
                    var parameters = new GetHomeTimelineParameters()
                    {
                        SinceId = finalTweet.Id,
                        PageSize = 1,
                    };

                    var latestTweet = await userClient.Timelines.GetHomeTimelineAsync(parameters);
                    if (latestTweet.Length > 0)
                    {
                        if (latestTweet[0] != finalTweet)
                        {
                            internalHelpers.DisplayTweet(latestTweet.Last());
                            finalTweet = latestTweet.Last();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}