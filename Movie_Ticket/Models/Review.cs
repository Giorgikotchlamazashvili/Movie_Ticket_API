using System;
using System.Collections.Generic;

namespace Movie_Ticket.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? MovieId { get; set; }

    public int? UserId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual User? User { get; set; }
}
