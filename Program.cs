using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;
using tweetr.Factories;
using tweetr.Helpers;
using tweetr.Models;

namespace tweetr
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                CheckEnvironment();
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var authFactory = serviceProvider.GetService<AuthFactory>();
                var internalHelpers = serviceProvider.GetService<InternalHelpers>();
                var twitterFactory = serviceProvider.GetService<TwitterFactory>();

                string authUrl = await authFactory.GetAuthUrl();

                Console.WriteLine($"Open the following link in a browser to sign into twitter: {authUrl}");
                
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

        private static void CheckEnvironment()
        {
            Console.WriteLine("Starting up...");

            EnvironmentModel environment = new EnvironmentModel();

            bool envOK = true;

            foreach (PropertyInfo prop in environment.GetType().GetProperties())
            {

                var value = prop.GetValue(environment);
                if(value == null)
                {
                    envOK = false;
                    Console.WriteLine($"Environment variable {prop.Name} is empty");
                }
            }

            if(!envOK)
            {
                Console.WriteLine("Failed to configure environment, please fix the above errors and try again...");
                Environment.Exit(1);
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
