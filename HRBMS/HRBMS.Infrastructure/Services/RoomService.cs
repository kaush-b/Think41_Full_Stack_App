using HRBMS.Core.Entities;
using HRBMS.Core.Services;
using HRBMS.Infrastructure.Repositories;

namespace HRBMS.Infrastructure.Services;

public class RoomService : IRoomService
{
    private readonly RoomRepository _roomRepository;

    public RoomService(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken)
    {
        return _roomRepository.AddAsync(room, cancellationToken);
    }

    public Task<Room?> UpdateRoomAsync(int id, Room updated, CancellationToken cancellationToken)
    {
        return _roomRepository.UpdateAsync(id, updated, cancellationToken);
    }

    public Task<bool> DeleteRoomAsync(int id, CancellationToken cancellationToken)
    {
        return _roomRepository.DeleteAsync(id, cancellationToken);
    }

    public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _roomRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken)
    {
        var list = await _roomRepository.GetAllAsync(cancellationToken);
        return list;
    }

    public async Task<IReadOnlyList<Room>> GetAvailableAsync(DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        if (checkOut <= checkIn) throw new ArgumentException("Check-out must be after check-in");
        var list = await _roomRepository.GetAvailableAsync(checkIn, checkOut, cancellationToken);
        return list;
    }
}