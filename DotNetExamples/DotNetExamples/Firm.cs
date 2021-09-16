using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

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

        public static async Task<int> GetFirmInfo()
        {
            var token = Program.ReadToken();
            var response = await Constants.ApiBaseUrl
                .AppendPathSegments("v2", "firm")
                .WithHeaders(new
                {
                    Authorization = $"Bearer {token}"
                })
                .PostAsync();

            var firm = JsonSerializer.Deserialize<FirmModel>(await response.GetStringAsync());

            Console.WriteLine(JsonSerializer.Serialize(firm, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            return 0;
        }
    }
}
