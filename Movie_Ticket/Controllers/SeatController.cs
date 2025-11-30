using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly MovieTicketContext context;

        public SeatController(MovieTicketContext context)
        {
            this.context = context;
        }
        [Authorize]
        [HttpGet("Seats")]
        public IActionResult ShowSeats()
        {
            return Ok(
                new
                {
                    seatName = context.Seats.Select(u => u.SeatId),
                    seatId = context.Seats.Select(u => u.SeatId),
                    isAvaliable = context.Seats.Select(u => u.IsAvailable)
                });
        }

        [Authorize]
        [HttpGet("GetAvaliableSeatsByMovie")]
        public IActionResult GetAvailableSeatsByShow(string movie)
        {
            var seats = context.Shows
                .Where(s => s.Movie.Title == movie)
                .SelectMany(s => context.Seats
                    .Where(seat => seat.IsAvailable == true)
                    .Select(seat => new
                    {
                        MovieName = movie,
                        ShowId = s.ShowId,
                        seat.SeatNumber,
                        seat.SeatId
                    })
                )
                .ToList();

            return Ok(seats);
        }




    }
}
