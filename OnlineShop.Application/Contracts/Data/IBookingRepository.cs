using OnlineShop.Domain.Models;

namespace OnlineShop.Application.Contracts.Data;

public interface IBookingRepository
{
    public Task AddBooking(IEnumerable<Booking> bookings);
    
    public Task Unbook(Guid bookingId);

    public Task<IEnumerable<Booking>> GetBookings(Guid bookingId);
}