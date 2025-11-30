using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket.Dtos;
using Movie_Ticket.Helpers;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketPaymentController : ControllerBase
    {
        private readonly MovieTicketContext context;
        public TicketPaymentController(MovieTicketContext context, BookingCodeGenerator generator)
        {
            this.context = context;
        }

        [Authorize]
        [HttpPost("BuyTicket")]
        public IActionResult BuyTicket(BuyTicketDto dto)
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

            var show = context.Shows.FirstOrDefault(s => s.ShowId == dto.ShowId);
            if (show == null)
            {
                return BadRequest("Show not found.");
            }

            var seat = context.Seats.FirstOrDefault(s => s.SeatId == dto.SeatId);
            if (seat == null)
            {
                return BadRequest("Seat not found.");
            }

            if (!seat.IsAvailable)
            {
                return BadRequest("Seat is already booked.");
            }

            var alreadyBooked = context.Tickets
                .Any(t => t.ShowId == dto.ShowId && t.SeatId == dto.SeatId);

            if (alreadyBooked)
            {
                return BadRequest("This seat is already booked for this show.");
            }


            var ticket = new Ticket
            {
                UserId = userId,
                ShowId = dto.ShowId,
                SeatId = dto.SeatId,
                BookingCode = BookingCodeGenerator.CodeGenerator(),
                PurchaseDate = DateTime.Now,
                Price = show.Price
            };

            context.Tickets.Add(ticket);
            context.SaveChanges();

            var payment = new Payment
            {
                TicketId = ticket.TicketId,
                Amount = show.Price,
                PaymentStatus = "Pending"
            };

            context.Payments.Add(payment);

            seat.IsAvailable = false;

            context.SaveChanges();

            return Ok(new
            {
                Message = "Ticket purchased successfully!",
                TicketId = ticket.TicketId,
                BookingCode = ticket.BookingCode,
                PaymentId = payment.PaymentId,
                Price = show.Price
            });
        }
    }


}

