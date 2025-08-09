using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HRBMS.Core.Entities;
using HRBMS.Infrastructure.Data;
using HRBMS.Infrastructure.Repositories;
using HRBMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HRBMS.Tests;

public class BookingServiceTests
{
    private static HrbmsDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<HrbmsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new HrbmsDbContext(options);
    }

    [Fact]
    public async Task CreateBookingAsync_Should_Prevent_Overlaps()
    {
        using var db = CreateInMemoryDb();
        var roomRepo = new RoomRepository(db);
        var bookingRepo = new BookingRepository(db);
        var bookingService = new BookingService(bookingRepo, roomRepo);

        var room = await roomRepo.AddAsync(new Room { RoomNumber = "101", Capacity = 2, PricePerNight = 100, IsAvailable = true }, CancellationToken.None);

        var b1 = await bookingService.CreateBookingAsync(room.Id, 1, new DateTime(2025, 1, 10), new DateTime(2025, 1, 15), CancellationToken.None);
        b1.Should().NotBeNull();

        Func<Task> act = async () => await bookingService.CreateBookingAsync(room.Id, 2, new DateTime(2025, 1, 12), new DateTime(2025, 1, 16), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateStatus_Should_Change_Status()
    {
        using var db = CreateInMemoryDb();
        var roomRepo = new RoomRepository(db);
        var bookingRepo = new BookingRepository(db);
        var bookingService = new BookingService(bookingRepo, roomRepo);

        var room = await roomRepo.AddAsync(new Room { RoomNumber = "102", Capacity = 2, PricePerNight = 120, IsAvailable = true }, CancellationToken.None);
        var booking = await bookingService.CreateBookingAsync(room.Id, 1, new DateTime(2025, 2, 1), new DateTime(2025, 2, 5), CancellationToken.None);

        var updated = await bookingService.UpdateStatusAsync(booking.Id, BookingStatus.CheckedIn, CancellationToken.None);
        updated!.Status.Should().Be(BookingStatus.CheckedIn);

        updated = await bookingService.UpdateStatusAsync(booking.Id, BookingStatus.CheckedOut, CancellationToken.None);
        updated!.Status.Should().Be(BookingStatus.CheckedOut);
    }
}