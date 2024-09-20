namespace Project2.Models
{
    public class BookingPageView
    {
        public List<Booking>Bookings { get; set; }
        public int CurrentPage {  get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
