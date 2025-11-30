using System;
using System.Collections.Generic;

namespace Movie_Ticket.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int UserId { get; set; }

    public int ShowId { get; set; }

    public int SeatId { get; set; }

    public string BookingCode { get; set; } = null!;

    public DateTime? PurchaseDate { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Seat Seat { get; set; } = null!;

    public virtual Show Show { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
