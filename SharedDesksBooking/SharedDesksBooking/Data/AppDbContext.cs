using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<Desk> Desks { get; set; } = default!;
    public DbSet<Reservation> Reservations { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Desk)
            .WithMany(d => d.Reservations)
            .HasForeignKey(r => r.DeskId);
    }
}
