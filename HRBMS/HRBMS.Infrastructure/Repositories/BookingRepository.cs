using HRBMS.Core.Entities;
using HRBMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRBMS.Infrastructure.Repositories;

public class BookingRepository
{
    private readonly HrbmsDbContext _db;

    public BookingRepository(HrbmsDbContext db)
    {
        _db = db;
    }

    public Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _db.Bookings.Include(b => b.Room).Include(b => b.Guest).ToListAsync(cancellationToken);
    }

    public Task<List<Booking>> GetForUserAsync(int userId, CancellationToken cancellationToken)
    {
        return _db.Bookings.Where(b => b.GuestId == userId).Include(b => b.Room).ToListAsync(cancellationToken);
    }

    public async Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(cancellationToken);
        return booking;
    }

    public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Booking booking, CancellationToken cancellationToken)
    {
        _db.Bookings.Update(booking);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasOverlapAsync(int roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        return _db.Bookings.AnyAsync(b => b.RoomId == roomId && b.Status != BookingStatus.Cancelled &&
                                          ((checkIn < b.CheckOut) && (checkOut > b.CheckIn)), cancellationToken);
    }
}