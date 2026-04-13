using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class ResumeTrigger
{
    private readonly ILogger _logger;
    private readonly HttpClient _client;

    private readonly string site1 = "https://resume.azure-api.net";
    private readonly string site2 = "https://bharathomes.azurewebsites.net";
    private readonly string? APIM_KEY = Environment.GetEnvironmentVariable("APIM_KEY");

    private readonly string? sessionId = Environment.GetEnvironmentVariable("X_SESSION_ID");

    public ResumeTrigger(ILoggerFactory loggerFactory, HttpClient client)
    {
        _logger = loggerFactory.CreateLogger<ResumeTrigger>();
        _client = client;
    }

    [Function("ResumeTrigger")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);


        var request = new HttpRequestMessage(
            HttpMethod.Get,
            site1
        );

        // ✅ REQUIRED HEADERS (same as Postman)
        request.Headers.Add("Ocp-Apim-Subscription-Key", APIM_KEY);
        request.Headers.Add("X-Session-Id", sessionId);

        HttpResponseMessage res1 = await _client.SendAsync(request);
        string content = await res1.Content.ReadAsStringAsync();
        System.Console.WriteLine($"Resume Trigger : {(int)res1.StatusCode} - {content}");

        request = new HttpRequestMessage(
            HttpMethod.Get,
            site2
        );
        var res2 = await _client.SendAsync(request);

        System.Console.WriteLine($"Bharathomes response Status : {(int)res2.StatusCode} - {res2.StatusCode}");

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }

    }
 


    
}