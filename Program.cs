using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pastel;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace tweetr
{
    class Program
    {
        static double scanDelay = 0.3; // Seconds
        static double searchDelay = 30; // Seconds
        static int initialPageSize = 10;

        static Color titleColor = Color.FromArgb(163, 80, 80);
        static Color subTitleColor = Color.FromArgb(97, 118, 145);
        static Color mainTextColor = Color.FromArgb(140, 140, 140);
        static Color highlightColor = Color.FromArgb(121, 165, 209);
        static Color seperatorColor = Color.FromArgb(87, 87, 87);
        static Color urlColor = Color.FromArgb(61, 48, 252);

        static string CONSUMER_KEY = Environment.GetEnvironmentVariable("CONSUMER_KEY");
        static string CONSUMER_SECRET = Environment.GetEnvironmentVariable("CONSUMER_SECRET");
        static string ACCESS_TOKEN = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
        static string ACCESS_SECRET = Environment.GetEnvironmentVariable("ACCESS_SECRET");

        static async Task Main(string[] args)
        {
            // DEBUGGING
            // var userClient = new TwitterClient(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_SECRET);
            // var user = await userClient.Users.GetAuthenticatedUserAsync();
            // await LoadLiveTimeline(userClient, user);
            // END DEBUGGING


            await MainMenu();

        }

        private static async Task MainMenu()
        {
            try
            {
                var userClient = new TwitterClient(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_SECRET);
                var user = await userClient.Users.GetAuthenticatedUserAsync();

                DisplayLogo(user);

                Console.WriteLine();
                Console.WriteLine("1. Send Tweet (Not Implemented)");
                Console.WriteLine("2. Stream timeline");
                Console.WriteLine("3. Exit");
                Console.WriteLine();
                Console.Write(":> ");
                ConsoleKey selection = Console.ReadKey().Key;
                Console.WriteLine();



                switch (selection)
                {
                    case ConsoleKey.D1:
                        {
                            // await SendTweet(userClient, user);
                            throw new NotImplementedException();
                        }
                    case ConsoleKey.D2:
                        {
                            await LoadLiveTimeline(userClient, user);
                            break;
                        }
                    case ConsoleKey.D3:
                        {
                            Console.WriteLine("Ok Byee!");
                            Environment.Exit(1);
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        static async Task SendTweet(TwitterClient userClient, IAuthenticatedUser user)
        {

            DisplayLogo(user);
            Console.WriteLine("Send a Tweet:");
            Console.WriteLine();
            Console.Write(":> ");
            string tweetBody = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Sending tweet...");
            await userClient.Tweets.PublishTweetAsync(tweetBody);
            Console.WriteLine("Sent!");
            Thread.Sleep(1);
            await MainMenu();
        }

        private static void DisplayLogo(IAuthenticatedUser user)
        {
            Console.Clear();
            string[] lines = File.ReadAllLines("./logo.txt");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("                by Mark Pendlebury");
            Console.WriteLine();
            // Console.WriteLine($"Logged in as {user.ScreenName}");
        }

        static async Task LoadLiveTimeline(TwitterClient userClient, IAuthenticatedUser user)
        {
            try
            {
                DisplayLogo(user);
                Console.WriteLine();
                Console.WriteLine("Retrieving timeline..");

                var homeTimelineTweets = await userClient.Timelines.GetHomeTimelineAsync(new GetHomeTimelineParameters() { PageSize = initialPageSize });
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------------".Pastel(seperatorColor));

                ITweet finalTweet = null;

                if (homeTimelineTweets.Length > 0)
                {
                    homeTimelineTweets = homeTimelineTweets.OrderBy(t => t.CreatedAt.UtcDateTime).ToArray();
                    foreach (var tweet in homeTimelineTweets)
                    {
                        DisplayTweet(tweet);
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
                            DisplayTweet(latestTweet.Last());
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

        private static void DisplayTweet(Tweetinvi.Models.ITweet tweet)
        {
            Console.WriteLine();

            Console.Write($"{tweet.CreatedBy.Name} ".Pastel(titleColor));
            if (tweet.IsRetweet)
            {
                Console.Write($"[RT] ".Pastel(subTitleColor));
            }
            Console.WriteLine($"[{getTimeElapsed(tweet.CreatedAt)}]".Pastel(subTitleColor));
            Console.WriteLine("---------------------------------------------------------".Pastel(seperatorColor));
            Console.WriteLine();
            if (tweet.IsRetweet)
            {
                Console.WriteLine(FormatTweetBody(tweet.RetweetedTweet.FullText.Pastel(mainTextColor)));
            }
            else
            {
                Console.WriteLine(FormatTweetBody(tweet.FullText.Pastel(mainTextColor)));
            }

            if(tweet.Media.Count > 0)
            {
                foreach(var image in tweet.Media)
                {
                    Console.WriteLine($"[{image.MediaURLHttps}]".Pastel(urlColor));
                }
            }
            string replyCount = tweet.RetweetCount.ToString() ?? "0";
            
            Console.WriteLine();
            Console.WriteLine($"💬 {replyCount}    🔁 {tweet.RetweetCount}     💙 {tweet.FavoriteCount}");
            Console.WriteLine("---------------------------------------------------------".Pastel(seperatorColor));
            Console.WriteLine();
        }

        private static string FormatTweetBody(string fullText)
        {
            string[] words = fullText.Split(' ');
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("@"))
                {
                    words[i] = words[i].Pastel(highlightColor);
                }
                if (words[i].Contains("#"))
                {
                    words[i] = words[i].Pastel(highlightColor);
                }
                if (words[i].Contains("http"))
                {
                    words[i] = words[i].Pastel(highlightColor);
                }
                output.Append(words[i]);
                output.Append(" ");
            }

            return output.ToString();

        }

        static string getTimeElapsed(DateTimeOffset createdAt)
        {
            DateTime created = DateTime.Parse($"{createdAt.Hour}:{createdAt.Minute}:{createdAt.Second} {createdAt.Day}/{createdAt.Month}/{createdAt.Year}");

            TimeSpan since = DateTime.Now.Subtract(created);

            string result = string.Empty;


            if (since <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", since.Seconds);
            }
            else if (since <= TimeSpan.FromMinutes(60))
            {
                result = since.Minutes > 1 ?
                    String.Format("{0} minutes ago", since.Minutes) :
                    "a minute ago";
            }
            else if (since <= TimeSpan.FromHours(24))
            {
                result = since.Hours > 1 ?
                    String.Format("{0} hours ago", since.Hours) :
                    "an hour ago";
            }
            else if (since <= TimeSpan.FromDays(30))
            {
                result = since.Days > 1 ?
                    String.Format("{0} days ago", since.Days) :
                    "yesterday";
            }
            else if (since <= TimeSpan.FromDays(365))
            {
                result = since.Days > 30 ?
                    String.Format("{0} months ago", since.Days / 30) :
                    "a month ago";
            }
            else
            {
                result = since.Days > 365 ?
                    String.Format("{0} years ago", since.Days / 365) :
                    "a year ago";
            }

            return result;


        }

    }
}
