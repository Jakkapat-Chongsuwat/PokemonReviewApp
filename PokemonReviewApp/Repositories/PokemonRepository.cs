using Microsoft.Extensions.Logging;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<PokemonRepository> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public PokemonRepository(DataContext context, ILogger<PokemonRepository> logger) 
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all Pokemon order by Id
        /// </summary>
        /// <returns>All pokemon list order by id</returns>
        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int Id)
        {
            IQueryable<Review> review = _context.Reviews.Where(r => r.Pokemon.Id == Id);
            if (review.Count() <= 0)
            {
                return 0;
            }
            return ((decimal)review.Sum(r => r.Rating)/review.Count());
        }

        public bool PokemonExists(int Id)
        {
            return _context.Pokemon.Any(p => p.Id == Id);
        }

        public bool PokemonExists(string Name)
        {
            // Check if Name are null before calling Trim()
            string? trimmedName = Name?.Trim();

            // Additionally, check if trimmedName is null or empty before proceeding
            if (string.IsNullOrEmpty(trimmedName))
            {
                return false;
            }

            // Check if there is any pokemon with the same name
            return _context.Owners.Any(o => o.FirstName.ToLower() == trimmedName.ToLower());
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            // Ensure Owner and Category exist
            bool ownerExists = _context.Owners.Any(o => o.Id == ownerId);
            bool categoryExists = _context.Categories.Any(c => c.Id == categoryId);

            if (!ownerExists || !categoryExists)
            {
                return false; // Or handle this with an exception
            }

            // Create and add PokemonOwner and PokemonCategory
            PokemonOwner pokemonOwner = new PokemonOwner { OwnerId = ownerId, Pokemon = pokemon };
            PokemonCategory pokemonCategory = new PokemonCategory { CategoryId = categoryId, Pokemon = pokemon };

            _context.Add(pokemon);
            _context.Add(pokemonOwner);
            _context.Add(pokemonCategory);

            return Save();
        }

        public bool Save()
        {
            int saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(Pokemon pokemon)
        {
            _context.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon, ICollection<Review> reviews)
        {
            // Start a transaction
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // If there are related reviews, they need to be deleted first
                    // Assuming CASCADE DELETE is not set up at the database level
                    _context.RemoveRange(reviews);

                    // Now delete the pokemon
                    _context.Remove(pokemon);

                    // Save changes
                    _context.SaveChanges();

                    // Commit transaction if all operations were successful
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging and operational monitoring
                    _logger.LogError(ex, "Error deleting Pokemon with ID {PokeId}", pokemon.Id);

                    // Transaction will be rolled back due to the using statement's end
                    return false;
                }
            }
        }
    }
}
