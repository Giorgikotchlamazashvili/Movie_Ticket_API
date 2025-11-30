namespace Movie_Ticket.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public string SeatNumber { get; set; } = null!;
    public bool IsAvailable { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
