using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly MovieTicketContext context;
        public PaymentController(MovieTicketContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpPut("ConfrimPayment")]
        public IActionResult ConfrimPayment()
        {
            var email = User.FindFirst("Email")?.Value;
            if (email == null)
            {

                return Unauthorized("Invalid user.");
            }

            var userId = context.Users
                .Where(u => u.Email.ToLower() == email.ToLower())
                .Select(u => u.UserId)
                .FirstOrDefault();
            if (userId == 0)
            {
                return Unauthorized("User not found.");
            }

            var Ticket = context.Tickets.Join(
                context.Payments,
                t => t.TicketId,
                p => p.TicketId,
                (t, p) => new { T = t, P = p }).Where(
                x => x.P.PaymentStatus.ToLower() == "pending" && x.T.UserId == userId).
                Select(x => x.P);

            if (!Ticket.Any())
            {
                return NotFound("No pending payments found");
            }

            foreach (var payment in Ticket)
            {
                payment.PaymentStatus = "Confirmed";
            }

            context.SaveChanges();

            return Ok("Payment confrimed");

        }
    }
}
