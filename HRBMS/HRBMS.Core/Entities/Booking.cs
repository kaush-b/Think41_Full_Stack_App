namespace HRBMS.Core.Entities;

public enum BookingStatus
{
    Active = 0,
    Cancelled = 1,
    CheckedIn = 2,
    CheckedOut = 3
}

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;

    public Room? Room { get; set; }
    public User? Guest { get; set; }
}