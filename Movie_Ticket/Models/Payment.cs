using System;
using System.Collections.Generic;

namespace Movie_Ticket.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int TicketId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
