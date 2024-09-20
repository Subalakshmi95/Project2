using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>key):base(key)
        {
            
        }
        public DbSet<Booking>Bookings { get; set; }
    }
}
