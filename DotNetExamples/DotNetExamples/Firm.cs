using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DotNetExamples
{
    public static class Firm
    {
        public record FirmModel(
            string Name,
            string Description,
            IEnumerable<Fund> Funds);

        public record Fund(
            Guid Id,
            string Name,
            string Description,
            string Currency,
            IEnumerable<Account> Accounts);

        public record Account(
            Guid Id,
            string Type);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        public static async Task<int> GetFirmInfo()
        {
            var token = Program.ReadToken();
            var response = await Constants.ApiBaseUrl
                .AppendPathSegments("api", "firm")
                .WithHeaders(new
                {
                    Authorization = $"Bearer {token}"
                })
                .GetAsync();

            var firm = JsonSerializer.Deserialize<FirmModel>(await response.GetStringAsync(), JsonOptions);
            Console.WriteLine(JsonSerializer.Serialize(firm, JsonOptions));

            return 0;
        }
    }
}
