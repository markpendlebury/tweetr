using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;
using tweetr.Factories;
using tweetr.Helpers;

namespace tweetr
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var authFactory = serviceProvider.GetService<AuthFactory>();
                var internalHelpers = serviceProvider.GetService<InternalHelpers>();
                var twitterFactory = serviceProvider.GetService<TwitterFactory>();

                string authUrl = await authFactory.GetAuthUrl();

                Console.WriteLine(authUrl);
                TwitterClient userClient = await authFactory.StartListener();
                if (userClient != null)
                {
                    try
                    {
                        while (true)
                        {
                            var user = await userClient.Users.GetAuthenticatedUserAsync();

                            internalHelpers.DisplayLogo(user);

                            Console.WriteLine();
                            Console.WriteLine("1. Send Tweet");
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
                                        await twitterFactory.SendTweet(userClient, user);
                                        break;
                                    }
                                case ConsoleKey.D2:
                                    {
                                        await twitterFactory.LoadLiveTimeline(userClient, user);
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        private static void ConfigureServices(ServiceCollection serviceCollection)
        {
            // Factories
            serviceCollection.AddTransient<AuthFactory>();
            serviceCollection.AddTransient<TwitterFactory>();

            // Helpers
            serviceCollection.AddTransient<InternalHelpers>();
        }
    }
}
