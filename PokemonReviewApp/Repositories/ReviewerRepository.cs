using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repositories
{
    public class ReviewerRepository : IReviewerRepository
    {
        private DataContext _context;

        public ReviewerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _context.Reviewers.Add(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int reviewerId)
        {
            return _context.Reviewers
                           .Include(r => r.Reviews)
                           .FirstOrDefault(r => r.Id == reviewerId);
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.OrderBy(r => r.Id).ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers.Any(r => r.Id == reviewerId);
        }

        public bool ReviewerExists(string firstName, string lastName)
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
            return _context.Reviewers.Any(r => r.FirstName.ToLower() == trimmedFirstName.ToLower() && r.LastName.ToLower() == trimmedLastName.ToLower());
        }


        public bool Save()
        {
            int saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
