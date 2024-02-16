using System.Text.Json.Serialization;

namespace OnlineShop.Domain.ValueObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FinancialOperationType
{
    [JsonPropertyName("REFUND")]
    Refund,
    [JsonPropertyName("WITHDRAW")]
    Withdraw
}