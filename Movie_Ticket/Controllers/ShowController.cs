using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly MovieTicketContext context;

        public ShowController(MovieTicketContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("shows")]
        public IActionResult GetShows()
        {
            var data =
        context.Shows
            .Join(
                context.Movies,
                show => show.MovieId,
                movie => movie.MovieId,
                (show, movie) => new
                {
                    ShowId = show.ShowId,
                    MovieName = movie.Title,
                    ShowTime = show.ShowTime,
                    Price = show.Price
                }
            )
            .ToList();

            return Ok(data);
        }

        [Authorize]
        [HttpGet("GetAvaliableShowByMovie")]
        public IActionResult GetShowByMovie(string movie)
        {
            var data = context.Shows.Join(context.Movies,
                s => s.MovieId,
                m => m.MovieId,
                (s, m) => new
                {
                    Shows = s,
                    Movies = m
                }).Select(s => new
                {
                    ShowId = s.Shows.ShowId,
                    Price = s.Shows.Price,
                    MovieID = s.Movies.MovieId,
                    MovieName = s.Movies.Title,
                    ShowTime = s.Shows.ShowTime
                }).Where(s => s.MovieName.Trim().ToLower() == movie.Trim().ToLower()).ToList();

            return Ok(data);

        }
    }
}
