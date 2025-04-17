using Azure;
using Azure.Data.Tables;
using System;

public class SensorData : ITableEntity
{
    public string PartitionKey { get; set; } = "Sensor";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public double Temperature { get; set; }
    public double Humidity { get; set; }

    public ETag ETag { get; set; }
}
