using System;
using System.Collections.Generic;

namespace Movie_Ticket.Models;

public partial class Show
{
    public int ShowId { get; set; }

    public int MovieId { get; set; }

    public DateTime ShowTime { get; set; }

    public decimal Price { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
