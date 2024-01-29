using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public PokemonRepository(DataContext context) 
        {
            _context = context;
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
    }
}
