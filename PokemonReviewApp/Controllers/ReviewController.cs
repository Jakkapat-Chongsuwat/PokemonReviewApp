using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IPokemonRepository _pokemonRepository;
        private IReviewerRepository _reviewerRepository;
        private IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository,IPokemonRepository pokemonRepository,IReviewerRepository reviewerRepository,IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        // GET api/review
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviews()
        {
            List<ReviewDto> reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpGet("{reviewId}", Name = ("GetReview"))]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }
            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(review);
        }

        [HttpGet("review/{pokemonId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsOfPokemon(int pokemonId)
        {
            try
            {
                var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfPokemon(pokemonId));

                // Check if ModelState is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (reviews == null || reviews.Count == 0)
                {
                    // You might choose to return NotFound even if the Pokémon exists but has no reviews
                    // Alternatively, return Ok with an empty list if you want to signify that the Pokémon exists but has no reviews
                    return NotFound($"No reviews found for Pokémon with ID {pokemonId}.");
                }

                return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                // Catch the exception thrown by the repository
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreateReviewDto))]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_reviewRepository.ReviewExists(reviewDto.Id))
            {
                ModelState.AddModelError("", $"Review with ID {reviewDto.Id} already exists.");
                return StatusCode(409, ModelState);
            }

            Review review = _mapper.Map<Review>(reviewDto);

            review.Pokemon = _pokemonRepository.GetPokemon(reviewDto.PokemonId);
            review.Reviewer = _reviewerRepository.GetReviewer(reviewDto.ReviewerId);

            if (!_reviewRepository.CreateReview(review))
            {
                ModelState.AddModelError("", $"Something went wrong saving the review {reviewDto.Id}");
                return StatusCode(500, ModelState);
            }

            ReviewDto createdReviewDto = _mapper.Map<ReviewDto>(review);

            return CreatedAtRoute("GetReview", new { reviewId = createdReviewDto.Id }, createdReviewDto);
        }
    }
}
