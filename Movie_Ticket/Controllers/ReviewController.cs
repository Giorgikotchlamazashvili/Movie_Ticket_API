using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket.Models;

namespace Movie_Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly MovieTicketContext context;

        public ReviewController(MovieTicketContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpPost("CreateReview")]

        public IActionResult CreateReview(string movie, decimal rating, string comment)
        {
            var email = User.FindFirst("Email")?.Value;
            if (email == null)
            {
                return Unauthorized();
            }

            int movieId = context.Movies.Where(c => c.Title.Equals(movie.Trim().ToLower())).
                Select(c => c.MovieId).FirstOrDefault();
            if (movieId == 0)
            {
                return NotFound();
            }

            int userId = context.Users.Where(c => c.Email == email).
                Select(c => c.UserId).FirstOrDefault();
            if (userId == 0)
            {
                return BadRequest();
            }
            if (rating > 5)
            {
                return BadRequest("Rating Should Be Between 0-5");
            }
            var review = new Review
            {
                MovieId = movieId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            context.Add(review);
            context.SaveChanges();


            return Ok(new
            {
                ReviewID = review.ReviewId,
                MovieId = review.MovieId,
                MovieName = movie,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            });

        }
        [Authorize]
        [HttpGet("GetAllReviews")]
        public IActionResult GetAllReviews()
        {
            var reviews = context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .Select(r => new
                {
                    ReviewId = r.ReviewId,
                    MovieId = r.MovieId,
                    MovieName = r.Movie.Title,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return Ok(reviews);
        }
        [Authorize]
        [HttpGet("GetReviewsByMail")]

        public IActionResult GetReviewsByMail(string mail)
        {
            var reviews = context.Reviews.Join(
                context.Users,
                r => r.UserId,
                u => u.UserId,
                (r, u) => new
                {
                    Name = u.Name,
                    Email = u.Email,
                    Movie = r.Movie,
                    Comment = r.Comment,
                    Score = r.Rating
                }).Where(c => c.Email == mail.Trim()).ToList();

            return Ok(reviews);
        }
    }
}
