using System;
using System.Collections.Generic;

namespace Movie_Ticket.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public DateTime ReleaseDate { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
