using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repositories;
using System.Collections;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private IOwnerRepository _ownerRepository;
        private ICountryRepository _countryRepository;

        private IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepositpry,ICountryRepository countryRepository ,IMapper mapper)
        {
            _ownerRepository = ownerRepositpry;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _ownerRepository.GetOwners();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owners);
        }

        [HttpGet("{ownerId}", Name = "GetOwner")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            OwnerDto owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owner);
        }

        [HttpGet("/{ownerId}/ownerwithcountry")]
        [ProducesResponseType(200, Type = typeof(OwnerWithCountryDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetOwnerWithCountry(int ownerId)
        {
            var owner = _ownerRepository.GetOwnerWithCountry(ownerId);

            if (owner == null)
            {
                return NotFound();
            }

            OwnerWithCountryDto ownerWithCountryDto = _mapper.Map<OwnerWithCountryDto>(owner);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(ownerWithCountryDto);
        }

        [HttpGet("{ownerId}/pokemons")]
        [ProducesResponseType(200, Type = typeof(List<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            List<Pokemon> pokemons = _mapper.Map<List<Pokemon>> (_ownerRepository.GetPokemonsByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(OwnerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult CreateOwner([FromBody] OwnerDto ownerDtoToCreate)
        {
            if (ownerDtoToCreate == null)
            {
                return BadRequest(ModelState);
            }

            // Assuming OwnerDto has FirstName and LastName properties
            if (_ownerRepository.OwnerExists(ownerDtoToCreate.FirstName, ownerDtoToCreate.LastName))
            {
                ModelState.AddModelError("", $"Owner with name {ownerDtoToCreate.FirstName} {ownerDtoToCreate.LastName} already exists!");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            // Check if the country exists
            if (!_countryRepository.CountryExists(ownerDtoToCreate.CountryId))
            {
                ModelState.AddModelError("", $"Country with ID {ownerDtoToCreate.CountryId} does not exist!");
                return StatusCode(409, ModelState); // 409 Conflict
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Owner ownerEntity = _mapper.Map<Owner>(ownerDtoToCreate);

            if (!_ownerRepository.CreateOwner(ownerEntity))
            {
                ModelState.AddModelError("", $"Something went wrong saving {ownerDtoToCreate.FirstName} {ownerDtoToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            OwnerDto createdOwnerDto = _mapper.Map<OwnerDto>(ownerEntity);

            return CreatedAtRoute("GetOwner", new { ownerId = createdOwnerDto.Id }, createdOwnerDto);
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Owner ownerToDelete = _ownerRepository.GetOwner(ownerId);

            // Assuming DeleteOwner returns a boolean indicating success/failure
            if (!_ownerRepository.DeleteOwner(ownerToDelete))
            {
                return StatusCode(500, "Error deleting the owner.");
            }

            return Ok("Success");
        }
    }
}

