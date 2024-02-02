using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonReviewApp.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateOwner(Owner owner)
        {
            _context.Add(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerOfPokemon(int pokeId)
        {
            return _context.PokemonOwners.Where(po => po.Pokemon.Id == pokeId).Select(o => o.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public Owner GetOwnerWithCountry(int ownerId)
        {
            return _context.Owners
                .Include(o => o.Country) // Ensure eager loading of Country
                .FirstOrDefault(o => o.Id == ownerId);
        }

        public ICollection<Pokemon> GetPokemonsByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(po => po.Owner.Id == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public bool OwnerExists(string firstName, string lastName)
        {
            // Check if firstName and lastName are null before calling Trim()
            string? trimmedFirstName = firstName?.Trim();
            string? trimmedLastName = lastName?.Trim();

            // Additionally, check if trimmedFirstName or trimmedLastName is null or empty before proceeding
            if (string.IsNullOrEmpty(trimmedFirstName) || string.IsNullOrEmpty(trimmedLastName))
            {
                return false;
            }

            // Check if there is any owner with the given first name and last name
            return _context.Owners.Any(o => o.FirstName.ToLower() == trimmedFirstName.ToLower() && o.LastName.ToLower() == trimmedLastName.ToLower());
        }

        public bool Save()
        {
            int saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
