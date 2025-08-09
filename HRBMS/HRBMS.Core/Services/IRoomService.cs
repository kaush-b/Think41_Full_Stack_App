using HRBMS.Core.Entities;

namespace HRBMS.Core.Services;

public interface IRoomService
{
    Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken);
    Task<Room?> UpdateRoomAsync(int id, Room updated, CancellationToken cancellationToken);
    Task<bool> DeleteRoomAsync(int id, CancellationToken cancellationToken);
    Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Room>> GetAvailableAsync(DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken);
}