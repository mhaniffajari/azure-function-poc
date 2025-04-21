using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace pocfunction
{
    public class SqlChangeTriggerFunction
    {
        private static readonly string httpEndpoint = Environment.GetEnvironmentVariable("CPI_ENDPOINT");
        private static readonly string username = Environment.GetEnvironmentVariable("CPI_USERNAME");
        private static readonly string password = Environment.GetEnvironmentVariable("CPI_PASSWORD");

        [FunctionName("SqlChangeTriggerFunction")]
        public async Task RunAsync(
            [SqlTrigger("[dbo].[poc_table]", "SqlConnectionString")]
            IReadOnlyList<SqlChange<SqlPocData>> changes,
            ILogger log)
        {
            foreach (var change in changes)
            {
                var jsonData = JsonConvert.SerializeObject(change);
                log.LogInformation($"Change detected: {jsonData}");

                await SendHttpRequestAsync(jsonData, log);
            }
        }

        private static async Task SendHttpRequestAsync(string jsonData, ILogger log)
        {
            try
            {
                string credentials = $"{username}:{password}";
                string base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

                string curlCommand = $"curl.exe --location \"{httpEndpoint}\" " +
                                     $"--header \"Authorization: Basic {base64Auth}\"";

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {curlCommand}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        string output = await process.StandardOutput.ReadToEndAsync();
                        log.LogInformation($"Curl command output: {output}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error executing curl command: {ex.Message}");
            }
        }

    }

    public class SqlPocData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
