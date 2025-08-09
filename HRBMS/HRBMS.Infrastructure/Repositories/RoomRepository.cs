using HRBMS.Core.Entities;
using HRBMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRBMS.Infrastructure.Repositories;

public class RoomRepository
{
    private readonly HrbmsDbContext _db;

    public RoomRepository(HrbmsDbContext db)
    {
        _db = db;
    }

    public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<List<Room>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _db.Rooms.ToListAsync(cancellationToken);
    }

    public async Task<Room> AddAsync(Room room, CancellationToken cancellationToken)
    {
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync(cancellationToken);
        return room;
    }

    public async Task<Room?> UpdateAsync(int id, Room updated, CancellationToken cancellationToken)
    {
        var existing = await _db.Rooms.FindAsync(new object[] { id }, cancellationToken);
        if (existing == null) return null;
        existing.RoomNumber = updated.RoomNumber;
        existing.Capacity = updated.Capacity;
        existing.PricePerNight = updated.PricePerNight;
        existing.IsAvailable = updated.IsAvailable;
        existing.AmenitiesCsv = updated.AmenitiesCsv;
        await _db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _db.Rooms.FindAsync(new object[] { id }, cancellationToken);
        if (existing == null) return false;
        _db.Rooms.Remove(existing);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<List<Room>> GetAvailableAsync(DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        return _db.Rooms
            .Where(r => r.IsAvailable)
            .Where(r => !_db.Bookings.Any(b => b.RoomId == r.Id && b.Status != BookingStatus.Cancelled &&
                                               ((checkIn < b.CheckOut) && (checkOut > b.CheckIn))))
            .ToListAsync(cancellationToken);
    }
}