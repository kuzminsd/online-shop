using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OnlineShop.Domain.ValueObjects;

public enum OrderStatus
{
    Collecting,
    BookingInProgress,
    Booked,
    DeliverySet,
    PaymentInProgress,
    Payed,
}