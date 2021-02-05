// using System;
// using System.Threading.Tasks;
// using Tweetinvi;
// using tweetr.Factories;
// using tweetr.Helpers;

// namespace tweetr.Workers
// {
//     public class MainWorker
//     {

//         private readonly TwitterFactory twitterFactory;
//         private readonly InternalHelpers internalHelpers;
//         public MainWorker(TwitterFactory twitterFactory, InternalHelpers internalHelpers)
//         {
//             this.twitterFactory = twitterFactory;
//             this.internalHelpers = internalHelpers;
//         }
//         internal async Task MainMenu(TwitterClient userClient)
//         {
//             try
//             {
//                 var user = await userClient.Users.GetAuthenticatedUserAsync();

//                 internalHelpers.DisplayLogo(user);

//                 Console.WriteLine();
//                 Console.WriteLine("1. Send Tweet (Not Implemented)");
//                 Console.WriteLine("2. Stream timeline");
//                 Console.WriteLine("3. Exit");
//                 Console.WriteLine();
//                 Console.Write(":> ");
//                 ConsoleKey selection = Console.ReadKey().Key;
//                 Console.WriteLine();



//                 switch (selection)
//                 {
//                     case ConsoleKey.D1:
//                         {
//                             await twitterFactory.SendTweet(userClient, user);
//                             break;
//                         }
//                     case ConsoleKey.D2:
//                         {
//                             await twitterFactory.LoadLiveTimeline(userClient, user);
//                             break;
//                         }
//                     case ConsoleKey.D3:
//                         {
//                             Console.WriteLine("Ok Byee!");
//                             Environment.Exit(1);
//                             break;
//                         }
//                 }

//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.ToString());
//                 throw;
//             }
//         }
//     }
// }