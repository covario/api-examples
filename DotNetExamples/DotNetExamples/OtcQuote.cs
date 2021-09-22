using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetExamples
{
    public class OtcQuote
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        public static async Task<int> StartStreamingQuote(OtcQuoteOptions opts)
        {
            var client = new ClientWebSocket();
            client.Options.SetRequestHeader("Authorization", $"Bearer {Program.ReadToken()}");
            await client.ConnectAsync(new Uri(Constants.WebSocketBaseUrl), CancellationToken.None);

            while (client.State != WebSocketState.Open)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            await Task.Delay(TimeSpan.FromSeconds(1));

            var request = new
            {
                type = "otc-stream-start",
                data = new
                {
                    instrument = opts.Instrument,
                    quantity = opts.Quantity,
                    quantityCurrency = opts.Instrument.Split('-').First(),
                    fundId = opts.FundId,
                    accountId = opts.AccountId,
                    clientReference = Guid.NewGuid().ToString(),
                }
            };
            var requestText = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

            await client.SendAsync(
                new ReadOnlyMemory<byte>(requestText),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            var buffer = new byte[1024 * 2];

            while (client.State == WebSocketState.Open)
            {
                var res = await client.ReceiveAsync(new Memory<byte>(buffer), CancellationToken.None);

                var quote = await JsonSerializer.DeserializeAsync<QuoteMessage>(
                    new MemoryStream(buffer, 0, Array.IndexOf(buffer, (byte)0)),
                    JsonOptions);

                Console.WriteLine(JsonSerializer.Serialize(quote, JsonOptions));

                Array.Fill(buffer, (byte)0);
            }

            return 0;
        }

        public record QuoteMessage
        {
            public string Type { get; init; }

            public QuoteUpdate Data { get; init; }
        }

        public record QuoteUpdate
        {
            public string Instrument { get; init; }

            public decimal Quantity { get; init; }

            public decimal BuyPrice { get; init; }

            public decimal SellPrice { get; init; }
        }
    }
}
