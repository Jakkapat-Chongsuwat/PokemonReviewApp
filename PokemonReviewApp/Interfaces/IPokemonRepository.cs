using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int Id);
        Pokemon GetPokemon(string Name);
        decimal GetPokemonRating(int Id);
        bool PokemonExists(int Id);
        bool PokemonExists(string Name);
        bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool UpdatePokemon(Pokemon pokemon);
        bool DeletePokemon(Pokemon pokemon, ICollection<Review> reviews);
        bool Save();

    }
}
