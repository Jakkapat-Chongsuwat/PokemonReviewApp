using PokemonReviewApp.Data;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext _context;

        public CategoryRepository(DataContext context) 
        {
            _context = context;
        }
        public bool CategoryExists(int Id)
        {
            return _context.Categories.Any(c => c.Id == Id);
        }
        public bool CategoryExists(string name)
        {
            // Check if categoryName is null before calling Trim()
            string? trimmedName = name?.Trim();

            // Additionally, check if trimmedName is null or empty before proceeding
            if (string.IsNullOrEmpty(trimmedName))
            {
                return false;
            }

            return _context.Categories.Any(c => c.Name.ToLower() == trimmedName.ToLower());
        }

        public bool CreateCategory(Category category)
          {
            _context.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _context.Remove(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.Id).ToList();
        }

        public Category GetCategory(int Id)
        {
           return _context.Categories.Where(c => c.Id == Id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonsByCategory(int categoryId)
        {
            return _context.PokemonCategories.Where(pc => pc.CategoryId == categoryId).Select(c => c.Pokemon).ToList();
        }

        public bool Save()
        {
            int saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _context.Update(category);
            return Save();
        }
    }
}
