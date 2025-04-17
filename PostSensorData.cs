using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class PostSensorData
{
    private readonly ILogger _logger;

    public PostSensorData(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PostSensorData>();
    }

    [Function("PostSensorData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("📥 Received sensor data.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonSerializer.Deserialize<SensorData>(requestBody);

        if (data is null || data.Temperature == 0 && data.Humidity == 0)
        {
            var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("❌ Invalid data.");
            return badResponse;
        }

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var tableClient = new TableClient(connectionString, "SensorData");
        await tableClient.CreateIfNotExistsAsync();

        data.Timestamp = DateTime.UtcNow;
        await tableClient.AddEntityAsync(data);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("✅ Data saved.");
        return response;
    }
}
