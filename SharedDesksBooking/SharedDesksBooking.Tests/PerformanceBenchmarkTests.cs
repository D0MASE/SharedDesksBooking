using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Mappings;
using SharedDesksBooking.Models;
using SharedDesksBooking.Services;
using SharedDesksBooking.Models.Enums;
using Xunit.Abstractions;

namespace SharedDesksBooking.Tests;

public class PerformanceBenchmarkTests
{
    private readonly ITestOutputHelper _output;

    public PerformanceBenchmarkTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Benchmark_GetDesksWithAvailability_WithManyRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BenchmarkDb_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var deskService = new DeskService(context, mapper);

        // Seed many desks and reservations
        int deskCount = 100;
        int reservationCount = 5000;
        
        for (int i = 1; i <= deskCount; i++)
        {
            context.Desks.Add(new Desk { Id = i, Number = $"D{i}", Status = DeskStatus.Available });
        }
        
        var random = new Random();
        for (int i = 0; i < reservationCount; i++)
        {
            context.Reservations.Add(new Reservation
            {
                DeskId = random.Next(1, deskCount + 1),
                FirstName = "User",
                LastName = i.ToString(),
                StartDate = DateTime.Today.AddDays(random.Next(-30, 30)),
                EndDate = DateTime.Today.AddDays(random.Next(31, 60))
            });
        }
        await context.SaveChangesAsync();

        // Warm up
        await deskService.GetDesksWithAvailabilityAsync(DateTime.Today);

        // Act
        int iterations = 100;
        var sw = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            await deskService.GetDesksWithAvailabilityAsync(DateTime.Today);
        }
        
        sw.Stop();

        // Assert/Report
        double totalMs = sw.Elapsed.TotalMilliseconds;
        double avgMs = totalMs / iterations;
        
        _output.WriteLine($"--- PERFORMANCE BENCHMARK ---");
        _output.WriteLine($"Total desks: {deskCount}");
        _output.WriteLine($"Total reservations: {reservationCount}");
        _output.WriteLine($"Iterations: {iterations}");
        _output.WriteLine($"Total time: {totalMs:F2} ms");
        _output.WriteLine($"Average response time: {avgMs:F4} ms");
        _output.WriteLine($"-----------------------------");
        
        Assert.True(avgMs < 100, $"Performance too slow: {avgMs}ms");
    }
}
