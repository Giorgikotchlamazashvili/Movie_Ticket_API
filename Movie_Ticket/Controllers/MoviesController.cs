using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieTicketContext context;
        public MoviesController(MovieTicketContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("movies")]
        public IActionResult GetMovies()
        {
            var movies = context.Movies
                .Select(u => new
                {
                    u.MovieId,
                    u.Title,
                    u.Description,
                    u.DurationMinutes,
                    u.ReleaseDate
                })
                .ToList();

            return Ok(movies);
        }

        [Authorize]
        [HttpGet("moviesID")]
        public IActionResult GetMovie(int id)
        {
            var movie = context.Movies
                .Where(u => u.MovieId == id)
                .Select(u => new
                {
                    u.MovieId,
                    u.Title,
                    u.Description,
                    u.DurationMinutes,
                    u.ReleaseDate
                })
                .FirstOrDefault();

            if (movie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            return Ok(movie);
        }

        [Authorize]
        [HttpGet("moviesByName")]
        public IActionResult GetMovieByName(string name)
        {
            var movies = context.Movies
                .Where(u => u.Title.ToLower() == name.ToLower().Trim())
                .Select(u => new
                {
                    u.MovieId,
                    u.Title,
                    u.Description,
                    u.DurationMinutes,
                    u.ReleaseDate
                })
                .ToList();

            if (!movies.Any())
            {
                return NotFound(new { message = "No movies found with this name" });
            }

            return Ok(movies);
        }
    }
}
