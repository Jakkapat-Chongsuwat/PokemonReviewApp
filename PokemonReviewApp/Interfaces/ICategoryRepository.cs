using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int Id);
        ICollection<Pokemon> GetPokemonsByCategory(int Id);
        bool CategoryExists(int Id);
        bool CategoryExists(string name);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
