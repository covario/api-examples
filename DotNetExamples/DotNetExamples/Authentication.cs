using System;
using System.IO;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace DotNetExamples
{
    public static class Authentication
    {
        public record AuthResult(
            [property: JsonProperty("access_token")] string AccessToken,
            [property: JsonProperty("expires_in")] int ExpiresIn,
            [property: JsonProperty("token_type")] string TokenType,
            [property: JsonProperty("scope")] string Scope);

        public static async Task<int> Authenticate(AuthOptions options)
        {
            var response = await Constants.AuthBaseUrl
                .AppendPathSegments("connect", "token")
                .AllowAnyHttpStatus()
                .PostUrlEncodedAsync(new
                {
                    client_id = options.ClientId,
                    client_secret = options.Secret,
                    grant_type = "client_credentials"
                });

            if (response.StatusCode != 200)
            {
                throw new Exception($"HTTP status code {response.StatusCode}\n" +
                                    $"Response body: {await response.GetStringAsync()}");
            }

            var authResponse = await response.GetJsonAsync<AuthResult>();

            Console.WriteLine("Successful auth response:");
            Console.WriteLine(authResponse);

            await using var file = File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), "token.txt"));
            await file.WriteLineAsync(authResponse.AccessToken);

            Console.WriteLine("Token written to file token.txt");

            return 0;
        }
    }
}
