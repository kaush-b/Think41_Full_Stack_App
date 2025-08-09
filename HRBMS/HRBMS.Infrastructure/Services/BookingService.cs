using HRBMS.Core.Entities;
using HRBMS.Core.Services;
using HRBMS.Infrastructure.Repositories;

namespace HRBMS.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly BookingRepository _bookingRepository;
    private readonly RoomRepository _roomRepository;

    public BookingService(BookingRepository bookingRepository, RoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<Booking> CreateBookingAsync(int roomId, int guestId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("Check-out must be after check-in");
        }

        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken) ?? throw new InvalidOperationException("Room not found");
        if (!room.IsAvailable)
        {
            throw new InvalidOperationException("Room is not available for booking");
        }

        var hasOverlap = await _bookingRepository.HasOverlapAsync(roomId, checkIn, checkOut, cancellationToken);
        if (hasOverlap)
        {
            throw new InvalidOperationException("Room is already booked for the selected dates");
        }

        var booking = new Booking
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Status = BookingStatus.Active
        };

        return await _bookingRepository.AddAsync(booking, cancellationToken);
    }

    public async Task<bool> CancelBookingAsync(int bookingId, int requesterUserId, bool requesterIsStaffOrAdmin, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return false;
        if (!requesterIsStaffOrAdmin && booking.GuestId != requesterUserId)
        {
            throw new UnauthorizedAccessException("Only the owner or staff/admin can cancel this booking");
        }

        if (booking.Status == BookingStatus.Cancelled) return true;

        booking.Status = BookingStatus.Cancelled;
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        return true;
    }

    public async Task<Booking?> UpdateStatusAsync(int bookingId, BookingStatus status, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;
        booking.Status = status;
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        return booking;
    }

    public Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int userId, CancellationToken cancellationToken)
    {
        return _bookingRepository.GetForUserAsync(userId, cancellationToken).ContinueWith(t => (IReadOnlyList<Booking>)t.Result, cancellationToken);
    }

    public Task<IReadOnlyList<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken)
    {
        return _bookingRepository.GetAllAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Booking>)t.Result, cancellationToken);
    }
}