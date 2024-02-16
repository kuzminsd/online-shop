using Microsoft.EntityFrameworkCore;
using OnlineShop.Application.Contracts.Data;
using OnlineShop.Domain.Models;

namespace OnlineShop.Persistence.Repositories;
/*
public class BookingRepository(OnlineShopDbContext dbContext) : IBookingRepository
{
   public async Task AddBooking(IEnumerable<Booking> bookings)
    {
        foreach (var booking in bookings)
        {
            await dbContext.Bookings.AddAsync(booking);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Unbook(Guid bookingId)
    {
        foreach (var booking in dbContext.Bookings.Where(x => x.Id == bookingId))
        {
            booking.Booked = false;
        }
        
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookings(Guid bookingId)
    {
        return await dbContext.Bookings.Where(x => x.Id == bookingId && x.Booked).ToListAsync();
    }
}*/