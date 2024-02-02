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
    public class CountryController : Controller
    {
        private ICountryRepository _countryRepository;
        private IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]
        public IActionResult GetCountries()
        {
            List<CountryDto> countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(countries);
        }

        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }
            CountryDto country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(country);
        }

        [HttpGet("/{ownerId}/country")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            CountryDto country = _mapper.Map<CountryDto>(
                _countryRepository.GetCountryByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(country);
        }

        [HttpGet("/{countryId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetOwnersByCountry(int countryId)
        {
            List<OwnerDto> owners = _mapper.Map<List<OwnerDto>>(
                _countryRepository.GetOwnersByCountry(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CountryDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody] CountryDto countryDtoToCreate)
        {
            if (countryDtoToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (_countryRepository.CountryExists(countryDtoToCreate.Id))
            {
                ModelState.AddModelError("", $"Country with ID {countryDtoToCreate.Id} already exists!");
                return StatusCode(409, ModelState);
            }

            if (_countryRepository.CountryExists(countryDtoToCreate.Name))
            {
                ModelState.AddModelError("", $"Country with name {countryDtoToCreate.Name} already exists!");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country countryEntity = _mapper.Map<Country>(countryDtoToCreate);

            if (!_countryRepository.CreateCountry(countryEntity))
            {
                ModelState.AddModelError("", $"Something went wrong saving {countryDtoToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            CountryDto createdCountryDto = _mapper.Map<CountryDto>(countryEntity);

            return CreatedAtRoute("GetCountry", new { countryId = createdCountryDto.Id }, createdCountryDto);
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto countryDto)
        {
            if (countryDto == null || countryId != countryDto.Id)
            {
                return BadRequest("Invalid request.");
            }

            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound($"Country with ID {countryId} not found.");
            }

            Country countryToUpdate = _mapper.Map<Country>(countryDto);

            // Assuming UpdateCategory returns a boolean indicating success/failure
            if (!_countryRepository.UpdateCountry(countryToUpdate))
            {
                return StatusCode(500, "Error updating the country.");
            }

            // Optionally, re-fetch the updated category from the repository if you need to ensure
            // you're returning the latest state including any changes that might have been applied
            // automatically by the database or ORM (like computed fields, timestamps, etc.)
            Country updatedCountry = _countryRepository.GetCountry(countryId);
            CountryDto updatedCountryDto = _mapper.Map<CountryDto>(updatedCountry);

            // Return the updated resource
            // You could use Ok(updatedCountryDto) if you just want to return the updated data
            // Or use CreatedAtRoute if you want to also indicate the location of the updated resource
            return CreatedAtRoute("GetCountry", new { countryId = updatedCountryDto.Id }, updatedCountryDto);
        }

    }
}
