using System;
using System.Collections.Generic;

namespace Movie_Ticket.Models;

public partial class EmailVerification
{
    public int VerificationId { get; set; }

    public int UserId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime Expiration { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
