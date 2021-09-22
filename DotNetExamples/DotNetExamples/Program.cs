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

        public const string ApiBaseUrl = "http://localhost:44371";

        public const string WebSocketBaseUrl = "ws://localhost:44371/ws";
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

    [Verb("otc-stream")]
    public class OtcQuoteOptions
    {
        [Option('i', "instrument", Required = true, HelpText = "Tradable instrument, e.g. BTC-USD")]
        public string Instrument { get; set; }

        [Option('q', "quantity", Required = true, HelpText = "Quantity in base currency")]
        public decimal Quantity { get; set; }

        [Option('f', "fundId", Required = true, HelpText = "Your fund id")]
        public string FundId { get; set; }

        [Option('a', "accountId", Required = true, HelpText = "Your otc account id")]
        public string AccountId { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<AuthOptions, FirmOptions, OtcQuoteOptions>(args)
                    .MapResult(
                        (AuthOptions opts) => Authentication.Authenticate(opts).GetAwaiter().GetResult(),
                        (FirmOptions opts) => Firm.GetFirmInfo().GetAwaiter().GetResult(),
                        (OtcQuoteOptions opts) => OtcQuote.StartStreamingQuote(opts).GetAwaiter().GetResult(),
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
