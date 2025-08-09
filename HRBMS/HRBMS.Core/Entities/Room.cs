namespace HRBMS.Core.Entities;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; } = true; // Operational availability
    public string? AmenitiesCsv { get; set; } // Comma-separated list for simplicity

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}