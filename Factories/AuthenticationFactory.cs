using System;
using System.Net;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Auth;
using Tweetinvi.Parameters;

namespace tweetr.Factories
{
    public class AuthFactory
    {
        private static readonly IAuthenticationRequestStore _myAuthRequestStore = new LocalAuthenticationRequestStore();

        internal async Task<string> GetAuthUrl()
        {

            var appClient = new TwitterClient(Environment.GetEnvironmentVariable("CONSUMER_API_KEY"), Environment.GetEnvironmentVariable("CONSUMER_API_SECRET"));
            var authenticationRequestId = Guid.NewGuid().ToString();
            var redirectPath = "http://localhost:5001/validateTwitterAuth/";

            // Add the user identifier as a query parameters that will be received by `ValidateTwitterAuth`
            var redirectURL = _myAuthRequestStore.AppendAuthenticationRequestIdToCallbackUrl(redirectPath, authenticationRequestId);
            // Initialize the authentication process
            var authenticationRequestToken = await appClient.Auth.RequestAuthenticationUrlAsync(redirectURL);
            // Store the token information in the store
            await _myAuthRequestStore.AddAuthenticationTokenAsync(authenticationRequestId, authenticationRequestToken);

            // Return the authentication URL so we can display it to the user: 
            return authenticationRequestToken.AuthorizationURL;
        }

        internal async Task<TwitterClient> StartListener()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:5001/validateTwitterAuth/");

                listener.Start();

                for (; ; )
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;

                    using( HttpListenerResponse response = context.Response )
                    {
                        var appClient = new TwitterClient(Environment.GetEnvironmentVariable("CONSUMER_API_KEY"), Environment.GetEnvironmentVariable("CONSUMER_API_SECRET"));

                        // Extract the information from the redirection url
                        var requestParameters = await RequestCredentialsParameters.FromCallbackUrlAsync(context.Request.Url.Query, _myAuthRequestStore);
                        
                        // Request Twitter to generate the credentials.
                        var userCreds = await appClient.Auth.RequestCredentialsAsync(requestParameters);

                        // Congratulations the user is now authenticated!
                        var userClient = new TwitterClient(userCreds);

                        return userClient;
                    }
                }
            }
        }
    }
}