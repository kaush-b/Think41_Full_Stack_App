using HRBMS.Core.Entities;

namespace HRBMS.Core.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int roomId, int guestId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken);
    Task<bool> CancelBookingAsync(int bookingId, int requesterUserId, bool requesterIsStaffOrAdmin, CancellationToken cancellationToken);
    Task<Booking?> UpdateStatusAsync(int bookingId, BookingStatus status, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken);
}