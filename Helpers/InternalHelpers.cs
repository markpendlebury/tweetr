using System;
using System.Drawing;
using System.IO;
using System.Text;
using Pastel;
using Tweetinvi.Models;
using tweetr.Models;

namespace tweetr.Helpers
{
    public class InternalHelpers
    {
       
        static Color urlColor = Color.FromArgb(61, 48, 252);

        private readonly Colors colors;

        public InternalHelpers()
        {
            this.colors = new Colors();
        }
        
        internal void DisplayLogo(IAuthenticatedUser user)
        {
            Console.Clear();
            string[] lines = File.ReadAllLines("./logo.txt");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("                by Mark Pendlebury");
            Console.WriteLine();
            Console.WriteLine($"Logged in as {user.Name}");
        }

        internal void DisplayTweet(Tweetinvi.Models.ITweet tweet)
        {
            Console.WriteLine();

            Console.Write($"{tweet.CreatedBy.Name} ".Pastel(colors.titleColor));
            if (tweet.IsRetweet)
            {
                Console.Write($"[RT] ".Pastel(colors.subTitleColor));
            }
            Console.WriteLine($"[{getTimeElapsed(tweet.CreatedAt)}]".Pastel(colors.subTitleColor));
            Console.WriteLine("---------------------------------------------------------".Pastel(colors.seperatorColor));
            Console.WriteLine();
            if (tweet.IsRetweet)
            {
                Console.WriteLine(FormatTweetBody(tweet.RetweetedTweet.FullText.Pastel(colors.mainTextColor)));
            }
            else
            {
                Console.WriteLine(FormatTweetBody(tweet.FullText.Pastel(colors.mainTextColor)));
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
            Console.WriteLine("---------------------------------------------------------".Pastel(colors.seperatorColor));
            Console.WriteLine();
        }

        private string FormatTweetBody(string fullText)
        {
            string[] words = fullText.Split(' ');
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("@"))
                {
                    words[i] = words[i].Pastel(colors.highlightColor);
                }
                if (words[i].Contains("#"))
                {
                    words[i] = words[i].Pastel(colors.highlightColor);
                }
                if (words[i].Contains("http"))
                {
                    words[i] = words[i].Pastel(colors.highlightColor);
                }
                output.Append(words[i]);
                output.Append(" ");
            }

            return output.ToString();

        }

        private string getTimeElapsed(DateTimeOffset createdAt)
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