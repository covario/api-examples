using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace DotNetExamples
{
    public static class Constants
    {
        public const string AuthBaseUrl = "https://account-test.covar.io";

        public const string ApiBaseUrl = "https://api-test.covar.io";
    }

    [Verb("auth", HelpText = "Get authentication token")]
    public class AuthOptions
    {
        [Option('c', "client-id", Required = true, HelpText = "OAuth client id")]
        public string ClientId { get; set; }

        [Option('s', "secret", Required = true, HelpText = "OAuth client secret")]
        public string Secret { get; set; }
    }

    [Verb("firm", HelpText = "Get high-level firm information")]
    public class FirmOptions
    {
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<AuthOptions, FirmOptions>(args)
                    .MapResult(
                        (AuthOptions opts) => Authentication.Authenticate(opts).GetAwaiter().GetResult(),
                        (FirmOptions opts) => Firm.GetFirmInfo().GetAwaiter().GetResult(),
                        errs => 1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured:\n{e.Message}");
            }
        }

        public static string ReadToken()
        {
            return File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "token.txt")).First();
        }
    }
}
