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
    }
}
