using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repositories;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository,IOwnerRepository ownerRepository,ICategoryRepository categoryRepository, IReviewRepository reviewRepository,IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _ownerRepository = ownerRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        // GET api/pokemon
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemons()
        {
            List<PokemonDto> pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}", Name = "GetPokemon")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            PokemonDto pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            decimal rating = _pokemonRepository.GetPokemonRating(pokeId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreatePokemonDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult CreatePokemon([FromBody] CreatePokemonDto pokemonDtoToCreate)
        {
            if (pokemonDtoToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (_pokemonRepository.PokemonExists(pokemonDtoToCreate.Id))
            {
                ModelState.AddModelError("", "Pokemon Exists");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            if (_pokemonRepository.PokemonExists(pokemonDtoToCreate.Name))
            {
                ModelState.AddModelError("", "Pokemon Exists");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            if (!_ownerRepository.OwnerExists(pokemonDtoToCreate.OwnerId))
            {
                ModelState.AddModelError("", "Owner Doesn't Exists");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            if (!_categoryRepository.CategoryExists(pokemonDtoToCreate.CategoryId))
            {
                ModelState.AddModelError("", "Category Doesn't Exists");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Pokemon pokemonEntity = _mapper.Map<Pokemon>(pokemonDtoToCreate);

            bool created = _pokemonRepository.CreatePokemon(pokemonDtoToCreate.OwnerId, pokemonDtoToCreate.CategoryId, pokemonEntity);

            if (!created)
            {
                return StatusCode(500, "Unable to create Pokemon"); // Or a more specific error message
            }

            PokemonDto pokemonDtoCreated = _mapper.Map<PokemonDto>(pokemonEntity);
            return CreatedAtRoute("GetPokemon", new { pokeId = pokemonDtoCreated.Id }, pokemonDtoCreated);
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokemonId, [FromBody] PokemonDto pokemonDto)
        {
            if (pokemonDto == null || pokemonId != pokemonDto.Id)
            {
                return BadRequest("Invalid request.");
            }

            if (!_pokemonRepository.PokemonExists(pokemonId))
            {
                return NotFound($"Pokemon with ID {pokemonId} not found.");
            }

            Pokemon pokemonToUpdate = _mapper.Map<Pokemon>(pokemonDto);

            // Assuming UpdateCategory returns a boolean indicating success/failure
            if (!_pokemonRepository.UpdatePokemon(pokemonToUpdate))
            {
                return StatusCode(500, "Error updating the Pokemon.");
            }

            // Optionally, re-fetch the updated category from the repository if you need to ensure
            // you're returning the latest state including any changes that might have been applied
            // automatically by the database or ORM (like computed fields, timestamps, etc.)
            Pokemon updatedPokemon = _pokemonRepository.GetPokemon(pokemonId);
            PokemonDto updatedPokemonDto = _mapper.Map<PokemonDto>(updatedPokemon);

            // Return the updated resource
            // You could use Ok(updatedPokemonDto) if you just want to return the updated data
            // Or use CreatedAtRoute if you want to also indicate the location of the updated resource
            return CreatedAtRoute("GetPokemon", new { pokeId = updatedPokemonDto.Id }, updatedPokemonDto);
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound($"Pokemon with ID {pokeId} not found.");
            }

            Pokemon pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);
            ICollection<Review> reviews = _reviewRepository.GetReviewsOfPokemon(pokeId);

            bool result = _pokemonRepository.DeletePokemon(pokemonToDelete, reviews);

            if (!result)
            {
                // Log specific error or return a generic error message
                return StatusCode(500, "An error occurred while attempting to delete the Pokemon.");
            }

            // Return 204 No Content to indicate successful deletion without any content.
            return Ok("Success");
        }
    }
}
