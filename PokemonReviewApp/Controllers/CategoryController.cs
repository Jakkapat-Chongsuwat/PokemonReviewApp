using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            CategoryDto category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(category);
        }

        [HttpGet("/{categoryId}/pokemons")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetPokemonsByCategoryId(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            List<PokemonDto> pokemons = _mapper.Map<List<PokemonDto>>(
                _categoryRepository.GetPokemonsByCategory(categoryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoryDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDtoToCreate)
        {
            if (categoryDtoToCreate == null)
            {
                return BadRequest(ModelState);
            }

            // Check if a category with the same ID already exists
            if (_categoryRepository.CategoryExists(categoryDtoToCreate.Id))
            {
                ModelState.AddModelError("", $"Category with ID {categoryDtoToCreate.Id} already exists!");
                return StatusCode(404, ModelState);
            }

            // Check if a category with the same name already exists
            if (_categoryRepository.CategoryExists(categoryDtoToCreate.Name))
            {
                ModelState.AddModelError("", $"Category {categoryDtoToCreate.Name} already exists!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryEntity = _mapper.Map<Category>(categoryDtoToCreate);

            if (!_categoryRepository.CreateCategory(categoryEntity))
            {
                ModelState.AddModelError("", $"Something went wrong saving {categoryDtoToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            var createdCategoryDto = _mapper.Map<CategoryDto>(categoryEntity);

            return CreatedAtRoute("GetCategory", new { categoryId = createdCategoryDto.Id }, createdCategoryDto);
        }
    }
}
