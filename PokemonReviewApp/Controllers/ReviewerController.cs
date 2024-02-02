using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repositories;
using System.Collections;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private IReviewerRepository _reviewerRepository;
        private IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        // GET api/reviewer
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewers()
        {
            List<ReviewerDto> reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}/reviewer", Name = "GetReviewer")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            ReviewerDto reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(List<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            List<ReviewDto> reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreateReviewerDto))]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody] CreateReviewerDto reviewerToCreateDto)
        {
            if (reviewerToCreateDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_reviewerRepository.ReviewerExists(reviewerToCreateDto.Id))
            {
                ModelState.AddModelError("", $"Reviewer with ID {reviewerToCreateDto.Id} already exists.");
                return StatusCode(409, ModelState);
            }

            // Assuming OwnerDto has FirstName and LastName properties
            if (_reviewerRepository.ReviewerExists(reviewerToCreateDto.FirstName, reviewerToCreateDto.LastName))
            {
                ModelState.AddModelError("", $"Reviewer with name {reviewerToCreateDto.FirstName} {reviewerToCreateDto.LastName} already exists!");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            Reviewer reviewer = _mapper.Map<Reviewer>(reviewerToCreateDto);

            if (!_reviewerRepository.CreateReviewer(reviewer))
            {
                ModelState.AddModelError("", $"Something went wrong saving the reviewer {reviewerToCreateDto.Id}");
                return StatusCode(500, ModelState);
            }

            ReviewerDto createdReviewerDto = _mapper.Map<ReviewerDto>(reviewer);

            return CreatedAtRoute("GetReviewer", new { reviewerId = createdReviewerDto.Id }, createdReviewerDto);
        }
    }
}

